using FinanceDomain.Entities;
using FinanceDomain.Enums;
using SharedKernel;

namespace FinanceDomain.contracts
{
    /// Repository for FinancialTransaction.
    public interface IFinancialTransactionRepository:IRepository<FinancialTransaction>
    {
        Task<FinancialTransaction?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<FinancialTransaction>> GetByWalletIdAsync(Guid walletId, CancellationToken cancellationToken = default);
        Task<IEnumerable<FinancialTransaction>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<FinancialTransaction>> GetByUserIdAndDateRangeAsync(Guid userId, DateTime from, DateTime to, CancellationToken cancellationToken = default);
        Task<IEnumerable<FinancialTransaction>> GetByTypeAsync(Guid userId, TransactionType type, CancellationToken cancellationToken = default);
        Task<IEnumerable<FinancialTransaction>> GetByCategoryAsync(Guid userId, Guid categoryId, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalByTypeAsync(Guid userId, TransactionType type, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    }
}
