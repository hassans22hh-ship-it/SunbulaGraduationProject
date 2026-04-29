using DebtApplication.Dtos;
using DebtDomain.Enums;

namespace DebtApplication.DebtService
{
    public interface IDebtService
    {
        Task<DebtDto> GetByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<DebtWithPaymentsDto> GetByIdWithPaymentsAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<DebtDto>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<DebtDto>> GetUnpaidByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<DebtDto>> GetOverdueByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<DebtDto>> GetByTypeAsync(Guid userId, string debtType, CancellationToken cancellationToken = default);
        Task<DebtSummaryDto> GetDebtSummaryAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<DebtDto> CreateAsync(CreateDebtDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task<DebtDto> UpdateAsync(Guid id, UpdateDebtDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task<DebtPaymentDto> RecordPaymentAsync(Guid debtId, RecordPaymentDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task MarkAsPaidAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task ReopenAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteUserDataAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
