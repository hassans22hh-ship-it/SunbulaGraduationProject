using SharedKernel;

namespace FinanceDomain.contracts

{
    /// <summary>Unit of Work for Finance module.</summary>
    public interface IUnitOfWork : IDisposable
    {
        IWalletRepository Wallets { get; }
        IFinancialTransactionRepository Transactions { get; }
        IFinancialCategoryRepository FinancialCategories { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
