using AutoMapper;
using FinanceApplication.financedtos;
using FinanceApplication.FinanceServiceAbs;
using FinanceDomain.contracts;
using FinanceDomain.Entities;
using FinanceDomain.Enums;
using FinanceDomain.Exceptions;

namespace FinanceInfrastructure.financeSService
{
    public sealed class WalletService : IWalletService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public WalletService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<WalletDto> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
        {
            var wallet = await _uow.Wallets.GetByIdAsync(id, ct)
                ?? throw new WalletNotFoundException(id);

            EnsureOwnership(wallet.UserId, userId);
            return _mapper.Map<WalletDto>(wallet);
        }

        public async Task<IEnumerable<WalletDto>> GetAllAsync(Guid userId, CancellationToken ct = default)
        {
            var wallets = await _uow.Wallets.GetByUserIdAsync(userId, ct);
            return _mapper.Map<IEnumerable<WalletDto>>(wallets);
        }

        public async Task<WalletDto> CreateAsync(
            CreateWalletDto dto, Guid userId, CancellationToken ct = default)
        {
            var nameExists = await _uow.Wallets.NameExistsAsync(userId, dto.Name, ct);
            if (nameExists)
                throw new InvalidOperationException($"A wallet named '{dto.Name}' already exists.");

            var wallet = Wallet.Create(userId, dto.Name, dto.Type, dto.OpeningBalance, dto.Currency);

            await _uow.Wallets.AddAsync(wallet, ct);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<WalletDto>(wallet);
        }

        public async Task<WalletDto> UpdateAsync(
            Guid id, UpdateWalletDto dto, Guid userId, CancellationToken ct = default)
        {
            var wallet = await _uow.Wallets.GetByIdAsync(id, ct)
                ?? throw new WalletNotFoundException(id);

            EnsureOwnership(wallet.UserId, userId);

            wallet.Update(dto.Name, dto.Type);
            _uow.Wallets.Update(wallet);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<WalletDto>(wallet);
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
        {
            var wallet = await _uow.Wallets.GetByIdAsync(id, ct)
                ?? throw new WalletNotFoundException(id);

            EnsureOwnership(wallet.UserId, userId);

            await _uow.BeginTransactionAsync(ct);
            try
            {
                // Hard-delete all transactions belonging to this wallet
                await _uow.Transactions.HardDeleteByWalletIdAsync(id, ct);

                _uow.Wallets.Delete(wallet);
                await _uow.SaveChangesAsync(ct);
                await _uow.CommitTransactionAsync(ct);
            }
            catch
            {
                await _uow.RollbackTransactionAsync(ct);
                throw;
            }
        }

        public async Task<FinanceSummaryDto> GetSummaryAsync(
            Guid userId, string currency, CancellationToken ct = default)
        {
            var wallets = await _uow.Wallets.GetByUserIdAsync(userId, ct);
            var totalBalance = await _uow.Wallets.GetTotalBalanceByUserIdAsync(userId, currency, ct);

            var totalIncome = await _uow.Transactions.GetTotalByTypeAsync(userId, TransactionType.Income, cancellationToken: ct);
            var totalExpenses = await _uow.Transactions.GetTotalByTypeAsync(userId, TransactionType.Expense, cancellationToken: ct);
            var txCount = await _uow.Transactions.CountAsync(t => t.UserId == userId, ct);

            return new FinanceSummaryDto
            {
                TotalBalance = totalBalance,
                Currency = currency,
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                WalletCount = wallets.Count(),
                TransactionCount = txCount
            };
        }

        private static void EnsureOwnership(Guid ownerId, Guid requesterId)
        {
            if (ownerId != requesterId)
                throw new UnauthorizedAccessException("You do not have permission to access this wallet.");
        }
    }
}
