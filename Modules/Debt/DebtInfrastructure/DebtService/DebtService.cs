using DebtApplication.Dtos;
using DebtApplication.DebtService;
using DebtDomain.Enums;
using DebtDomain.Contracts;
using DebtDomain.Entities;
using System.Linq.Expressions;
using DebtDomain.Exceptions;
using Application.Services.Abstraction;

namespace DebtInfrastructure.DebtService
{
    public sealed class DebtService : IDebtService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserIntegrationService _userIntegrationService;

        public DebtService(IUnitOfWork unitOfWork, IUserIntegrationService userIntegrationService)
        {
            _unitOfWork = unitOfWork;
            _userIntegrationService = userIntegrationService;
        }

        public async Task<DebtDto> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var debt = await _unitOfWork.Debts.GetByIdAsync(id, cancellationToken);
            if (debt == null)
                throw new DebtNotFoundException(id);

            if (debt.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to access this debt");

            return MapToDto(debt);
        }

        public async Task<DebtWithPaymentsDto> GetByIdWithPaymentsAsync(
            Guid id,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var debt = await _unitOfWork.Debts.GetByIdWithPaymentsAsync(id, cancellationToken);
            if (debt == null)
                throw new DebtNotFoundException(id);

            if (debt.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to access this debt");

            return new DebtWithPaymentsDto
            {
                Debt = MapToDto(debt),
                Payments = debt.Payments.Select(MapToPaymentDto)
            };
        }

        public async Task<IEnumerable<DebtDto>> GetAllByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var debts = await _unitOfWork.Debts.GetByUserIdAsync(userId, cancellationToken);
            return debts.Select(MapToDto);
        }

        public async Task<IEnumerable<DebtDto>> GetUnpaidByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var debts = await _unitOfWork.Debts.GetUnpaidByUserIdAsync(userId, cancellationToken);
            return debts.Select(MapToDto);
        }

        public async Task<IEnumerable<DebtDto>> GetOverdueByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var debts = await _unitOfWork.Debts.GetOverdueByUserIdAsync(userId, cancellationToken);
            return debts.Select(MapToDto);
        }

        public async Task<IEnumerable<DebtDto>> GetByTypeAsync(
            Guid userId,
            string debtType,
            CancellationToken cancellationToken = default)
        {
            var debts = await _unitOfWork.Debts.GetByTypeAsync(userId, debtType, cancellationToken);
            return debts.Select(MapToDto);
        }

        public async Task<DebtSummaryDto> GetDebtSummaryAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var allDebts = await _unitOfWork.Debts.GetByUserIdAsync(userId, cancellationToken);
            var unpaidDebts = await _unitOfWork.Debts.GetUnpaidByUserIdAsync(userId, cancellationToken);
            var overdueDebts = await _unitOfWork.Debts.GetOverdueByUserIdAsync(userId, cancellationToken);

            var totalPayable = await _unitOfWork.Debts.GetTotalDebtAmountAsync(
                userId,
                "Payable",
                unpaidOnly: false,
                cancellationToken);

            var totalReceivable = await _unitOfWork.Debts.GetTotalDebtAmountAsync(
                userId,
                "Receivable",
                unpaidOnly: false,
                cancellationToken);

            var totalRemainingPayable = await _unitOfWork.Debts.GetTotalRemainingAmountAsync(
                userId,
                "Payable",
                cancellationToken);

            var totalRemainingReceivable = await _unitOfWork.Debts.GetTotalRemainingAmountAsync(
                userId,
                "Receivable",
                cancellationToken);

            return new DebtSummaryDto
            {
                TotalPayable = totalPayable,
                TotalReceivable = totalReceivable,
                TotalRemainingPayable = totalRemainingPayable,
                TotalRemainingReceivable = totalRemainingReceivable,
                TotalDebtsCount = allDebts.Count(),
                UnpaidDebtsCount = unpaidDebts.Count(),
                OverdueDebtsCount = overdueDebts.Count()
            };
        }

        public async Task<DebtDto> CreateAsync(
            CreateDebtDto dto,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            // Create using domain factory
            var debt = Debt.Create(
                userId,
                dto.CreditorName,
                dto.Amount,
                dto.DebtType,
                dto.DueDate,
                dto.Notes);

            // Add to repository
            await _unitOfWork.Debts.AddAsync(debt, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(debt);
        }

        public async Task<DebtDto> UpdateAsync(
            Guid id,
            UpdateDebtDto dto,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var debt = await _unitOfWork.Debts.GetByIdAsync(id, cancellationToken);
            if (debt == null)
                throw new DebtNotFoundException(id);

            // Authorization check
            if (debt.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to update this debt");

            // Use domain method
            debt.Update(dto.CreditorName, dto.DueDate, dto.Notes);

            _unitOfWork.Debts.Update(debt);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(debt);
        }

        public async Task<DebtPaymentDto> RecordPaymentAsync(
            Guid debtId,
            RecordPaymentDto dto,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var debt = await _unitOfWork.Debts.GetByIdAsync(debtId, cancellationToken);
            if (debt == null)
                throw new DebtNotFoundException(debtId);

            // Authorization check
            if (debt.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to record payment for this debt");

        
            // Use domain method
            var payment = debt.RecordPayment(dto.Amount, dto.PaymentDate, dto.Notes);

            _unitOfWork.Debts.Update(debt);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToPaymentDto(payment);
        }

        public async Task MarkAsPaidAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var debt = await _unitOfWork.Debts.GetByIdAsync(id, cancellationToken);
            if (debt == null)
                throw new DebtNotFoundException(id);

            if (debt.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to update this debt");

            debt.MarkAsPaid();

            _unitOfWork.Debts.Update(debt);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ReopenAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var debt = await _unitOfWork.Debts.GetByIdAsync(id, cancellationToken);
            if (debt == null)
                throw new DebtNotFoundException(id);

            if (debt.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to update this debt");

            debt.Reopen();

            _unitOfWork.Debts.Update(debt);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var debt = await _unitOfWork.Debts.GetByIdAsync(id, cancellationToken);
            if (debt == null)
                throw new DebtNotFoundException(id);

            if (debt.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to delete this debt");

            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                // Hard-delete all payments belonging to this debt
                await _unitOfWork.Debts.HardDeletePaymentsByDebtIdAsync(id, cancellationToken);

                _unitOfWork.Debts.Delete(debt);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }

        public async Task DeleteUserDataAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.Debts.HardDeleteByUserIdAsync(userId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // ═══════════════════════════════════════════════════════════════
        // MAPPING METHODS
        // ═══════════════════════════════════════════════════════════════

        private static DebtDto MapToDto(Debt debt)
        {
            return new DebtDto
            {
                Id = debt.Id,
                CreditorName = debt.CreditorName,
                Amount = debt.Amount.Value,
                RemainingAmount = debt.RemainingAmount.Value,
                DueDate = debt.DueDate,
                IsPaid = debt.IsPaid,
                IsOverdue = debt.IsOverdue,
                DebtType = debt.DebtType,
                Notes = debt.Notes,
                CreatedAt = debt.CreatedAt,
                UpdatedAt = debt.UpdatedAt
            };
        }

        private static DebtPaymentDto MapToPaymentDto(DebtPayment payment)
        {
            return new DebtPaymentDto
            {
                Id = payment.Id,
                DebtId = payment.DebtId,
                Amount = payment.Amount.Value,
                PaymentDate = payment.PaymentDate,
                Notes = payment.Notes,
                CreatedAt = payment.CreatedAt
            };
        }
    }
}

        
  

