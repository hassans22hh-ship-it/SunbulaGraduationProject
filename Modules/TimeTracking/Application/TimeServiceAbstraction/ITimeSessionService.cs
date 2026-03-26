using TimeTrackingApplication.TimeDtos;

namespace TimeTrackingApplication.TimeServiceAbstraction
{
    /// Service for time session operations including start, stop, manual create, and queries.

    public interface ITimeSessionService
    {
        Task<TimeSessionDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeSessionDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeSessionDto>> GetByDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default);
        Task<IEnumerable<TimeSessionDto>> GetByDateRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
        Task<(IEnumerable<TimeSessionDto> Sessions, int TotalCount)> GetPagedAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<TimeSessionDto?> GetActiveSessionAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>Starts a live tracking session. Throws if one already exists.</summary>
        Task<TimeSessionDto> StartAsync(StartSessionDto dto, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>Stops the active session and calculates coins.</summary>
        Task<TimeSessionDto> StopAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>Stops any active session for the user (for task switching).</summary>
        Task<TimeSessionDto?> StopActiveSessionAsync(Guid userId, CancellationToken cancellationToken = default);

        Task<TimeSessionDto> PauseAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);
        Task<TimeSessionDto> ResumeAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>Creates a manually added session.</summary>
        Task<TimeSessionDto> CreateManualAsync(CreateTimeSessionDto dto, Guid userId, CancellationToken cancellationToken = default);

        Task<TimeSessionDto> UpdateAsync(Guid id, UpdateTimeSessionDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

        Task<TimeSessionDto> RecoverSessionAsync(Guid sessionId, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteUserDataAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}

