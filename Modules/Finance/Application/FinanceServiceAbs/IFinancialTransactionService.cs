using FinanceApplication.financedtos;

namespace FinanceApplication.FinanceServiceAbs
{
    public interface IFinancialTransactionService
    {

        Task<FinancialTransactionDto> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);
        Task<IEnumerable<FinancialTransactionDto>> GetByWalletAsync(Guid walletId, Guid userId, CancellationToken ct = default);
        Task<IEnumerable<FinancialTransactionDto>> GetAllAsync(Guid userId, CancellationToken ct = default);
        Task<IEnumerable<FinancialTransactionDto>> GetByDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct = default);
        Task<FinancialTransactionDto> CreateAsync(CreateFinancialTransactionDto dto, Guid userId, CancellationToken ct = default);
        Task<FinancialTransactionDto> UpdateAsync(Guid id, UpdateFinancialTransactionDto dto, Guid userId, CancellationToken ct = default);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
        Task DeleteUserDataAsync(Guid userId, CancellationToken ct = default);
    }
}
