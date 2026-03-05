using AutoMapper;
using FinanceApplication.financedtos;
using FinanceApplication.FinanceServiceAbs;
using FinanceDomain.contracts;
using FinanceDomain.Entities;
using FinanceDomain.Enums;
using FinanceDomain.Exceptions;

namespace FinanceInfrastructure.financeSService
{
    public sealed class FinancialTransactionService : IFinancialTransactionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public FinancialTransactionService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<FinancialTransactionDto> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
        {
            var tx = await _uow.Transactions.GetByIdWithDetailsAsync(id, ct)
                ?? throw new FinancialTransactionNotFoundException(id);

            EnsureOwnership(tx.UserId, userId);
            return await MapWithDestinationWalletAsync(tx, ct);
        }

        public async Task<IEnumerable<FinancialTransactionDto>> GetByWalletAsync(
            Guid walletId, Guid userId, CancellationToken ct = default)
        {
            var wallet = await _uow.Wallets.GetByIdAsync(walletId, ct)
                ?? throw new WalletNotFoundException(walletId);

            EnsureOwnership(wallet.UserId, userId);

            var transactions = await _uow.Transactions.GetByWalletIdAsync(walletId, ct);
            return _mapper.Map<IEnumerable<FinancialTransactionDto>>(transactions);
        }

        public async Task<IEnumerable<FinancialTransactionDto>> GetAllAsync(Guid userId, CancellationToken ct = default)
        {
            var transactions = await _uow.Transactions.GetByUserIdAsync(userId, ct);
            return _mapper.Map<IEnumerable<FinancialTransactionDto>>(transactions);
        }

        public async Task<IEnumerable<FinancialTransactionDto>> GetByDateRangeAsync(
            Guid userId, DateTime from, DateTime to, CancellationToken ct = default)
        {
            var transactions = await _uow.Transactions.GetByUserIdAndDateRangeAsync(userId, from, to, ct);
            return _mapper.Map<IEnumerable<FinancialTransactionDto>>(transactions);
        }

        public async Task<FinancialTransactionDto> CreateAsync(
            CreateFinancialTransactionDto dto, Guid userId, CancellationToken ct = default)
        {
            // Validate source wallet
            var wallet = await _uow.Wallets.GetByIdAsync(dto.WalletId, ct)
                ?? throw new WalletNotFoundException(dto.WalletId);

            EnsureOwnership(wallet.UserId, userId);

            // Validate destination wallet for transfers
            if (dto.Type == TransactionType.Transfer)
            {
                if (dto.DestinationWalletId is null)
                    throw new ArgumentException("Destination wallet is required for transfers.");

                var destWallet = await _uow.Wallets.GetByIdAsync(dto.DestinationWalletId.Value, ct)
                    ?? throw new WalletNotFoundException(dto.DestinationWalletId.Value);

                EnsureOwnership(destWallet.UserId, userId);
            }

            // Validate category if provided
            if (dto.FinancialCategoryId.HasValue)
            {
                var category = await _uow.FinancialCategories.GetByIdAsync(dto.FinancialCategoryId.Value, ct)
                    ?? throw new FinancialCategoryNotFoundException(dto.FinancialCategoryId.Value);

                EnsureOwnership(category.UserId, userId);
            }

            // Create transaction (domain validates business rules)
            var transaction = FinancialTransaction.Create(
                userId,
                dto.WalletId,
                dto.DestinationWalletId,
                dto.FinancialCategoryId,
                dto.Type,
                dto.Amount,
                wallet.Balance.Currency,
                dto.Description,
                dto.TransactionDate);

            // Apply balance changes within a DB transaction
            await _uow.BeginTransactionAsync(ct);
            try
            {
                wallet.ApplyTransaction(dto.Amount, dto.Type);

                if (dto.Type == TransactionType.Transfer && dto.DestinationWalletId.HasValue)
                {
                    var destWallet = await _uow.Wallets.GetByIdAsync(dto.DestinationWalletId.Value, ct);
                    destWallet!.ReceiveTransfer(dto.Amount);
                    _uow.Wallets.Update(destWallet);
                }

                _uow.Wallets.Update(wallet);
                await _uow.Transactions.AddAsync(transaction, ct);
                await _uow.SaveChangesAsync(ct);
                await _uow.CommitTransactionAsync(ct);
            }
            catch
            {
                await _uow.RollbackTransactionAsync(ct);
                throw;
            }

            return _mapper.Map<FinancialTransactionDto>(transaction);
        }

        public async Task<FinancialTransactionDto> UpdateAsync(
            Guid id, UpdateFinancialTransactionDto dto, Guid userId, CancellationToken ct = default)
        {
            var tx = await _uow.Transactions.GetByIdWithDetailsAsync(id, ct)
                ?? throw new FinancialTransactionNotFoundException(id);

            EnsureOwnership(tx.UserId, userId);

            var wallet = await _uow.Wallets.GetByIdAsync(tx.WalletId, ct)
                ?? throw new WalletNotFoundException(tx.WalletId);

            await _uow.BeginTransactionAsync(ct);
            try
            {
                // Reverse old amount on wallet
                wallet.ReverseTransaction(tx.Amount, tx.Type);

                // Apply new amount
                wallet.ApplyTransaction(dto.Amount, tx.Type);

                tx.Update(dto.FinancialCategoryId, dto.Amount, dto.Description, dto.TransactionDate);

                _uow.Wallets.Update(wallet);
                _uow.Transactions.Update(tx);
                await _uow.SaveChangesAsync(ct);
                await _uow.CommitTransactionAsync(ct);
            }
            catch
            {
                await _uow.RollbackTransactionAsync(ct);
                throw;
            }

            return _mapper.Map<FinancialTransactionDto>(tx);
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
        {
            var tx = await _uow.Transactions.GetByIdAsync(id, ct)
                ?? throw new FinancialTransactionNotFoundException(id);

            EnsureOwnership(tx.UserId, userId);

            var wallet = await _uow.Wallets.GetByIdAsync(tx.WalletId, ct)
                ?? throw new WalletNotFoundException(tx.WalletId);

            await _uow.BeginTransactionAsync(ct);
            try
            {
                // Reverse balance on source wallet
                wallet.ReverseTransaction(tx.Amount, tx.Type);

                // Reverse on destination wallet for transfers
                if (tx.Type == TransactionType.Transfer && tx.DestinationWalletId.HasValue)
                {
                    var destWallet = await _uow.Wallets.GetByIdAsync(tx.DestinationWalletId.Value, ct);
                    if (destWallet is not null)
                    {
                        destWallet.ReverseTransaction(tx.Amount, TransactionType.Income);
                        _uow.Wallets.Update(destWallet);
                    }
                }

                _uow.Wallets.Update(wallet);
                _uow.Transactions.Delete(tx);
                await _uow.SaveChangesAsync(ct);
                await _uow.CommitTransactionAsync(ct);
            }
            catch
            {
                await _uow.RollbackTransactionAsync(ct);
                throw;
            }
        }

        // ─── Helpers ─────────────────────────────────────────────────────────────

        private async Task<FinancialTransactionDto> MapWithDestinationWalletAsync(
            FinancialTransaction tx, CancellationToken ct)
        {
            var dto = _mapper.Map<FinancialTransactionDto>(tx);

            if (tx.DestinationWalletId.HasValue)
            {
                var destWallet = await _uow.Wallets.GetByIdAsync(tx.DestinationWalletId.Value, ct);
                // Return record with destination name filled (records are immutable, use `with`)
                return dto with { DestinationWalletName = destWallet?.Name };
            }

            return dto;
        }

        private static void EnsureOwnership(Guid ownerId, Guid requesterId)
        {
            if (ownerId != requesterId)
                throw new UnauthorizedAccessException("You do not have permission to access this resource.");
        }
    }
}
