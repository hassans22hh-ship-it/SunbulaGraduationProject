namespace DebtDomain.Contracts
/// Unit of Work for Debt module.
{
    public interface IUnitOfWork: IDisposable
    {
        IDebtRepository Debts { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
