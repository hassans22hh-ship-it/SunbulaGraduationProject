using SharedKernel;
using TimeTrackingDomain.Entities;

namespace TimeTrackingDomain.Contracts

{
    /// Repository for TimeSession aggregate.
    /// Direct query methods — NO Specification Pattern.
    public interface ITimeSessionRepository: IRepository<TimeSession>
    {
        Task<IEnumerable<TimeSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeSession>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeSession>> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeSession>> GetByUserAndDateRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
        Task<TimeSession?> GetActiveSessionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<bool> HasActiveSessionAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeSession>> GetOverlappingSessionsAsync(Guid userId, DateTime startTime, DateTime endTime, Guid? excludeSessionId = null, CancellationToken cancellationToken = default);
        Task<int> GetSessionCountByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
        Task<(IEnumerable<TimeSession> Sessions, int TotalCount)> GetPagedByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
    }
}

