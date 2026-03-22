namespace TimeTrackingDomain.Contracts
{
    /// Unit of Work for TimeTracking module.

    public interface IUnitOfWork: IDisposable
    {
        ITimeSessionRepository TimeSessions { get; }
        IDailyTransactionRepository DailyTransactions { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}

