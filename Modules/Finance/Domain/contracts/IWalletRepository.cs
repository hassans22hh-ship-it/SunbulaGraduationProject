using FinanceDomain.Entities;
using SharedKernel;

namespace FinanceDomain.contracts
{
    /// Repository for Wallet aggregate.
    public interface IWalletRepository:IRepository<Wallet>
    {
        Task<Wallet?> GetByIdWithTransactionsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Wallet>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> NameExistsAsync(Guid userId, string name, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalBalanceByUserIdAsync(Guid userId, string currency, CancellationToken cancellationToken = default);
        Task HardDeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
