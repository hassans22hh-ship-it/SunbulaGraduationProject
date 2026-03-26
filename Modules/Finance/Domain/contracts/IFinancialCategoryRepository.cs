using FinanceDomain.Entities;
using SharedKernel;

namespace FinanceDomain.contracts
{
    /// Repository for FinancialCategory.
    public interface IFinancialCategoryRepository:IRepository<FinancialCategory>
    {
        Task<IEnumerable<FinancialCategory>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> NameExistsAsync(Guid userId, string name, CancellationToken cancellationToken = default);
        Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task HardDeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
