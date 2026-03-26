using TimeTrackingApplication.TimeDtos;

namespace TimeTrackingApplication.TimeServiceAbstraction
{
    /// Service for daily transaction queries and streak calculations.

    public interface IDailyTransactionService
    {
        Task<DailyTransactionDto?> GetByDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default);
        Task<IEnumerable<DailyTransactionDto>> GetByDateRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
        Task<DailySummaryDto> GetDailySummaryAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default);
        Task CheckAndAwardStreakBonusAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<int> GetCurrentStreakAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<DailyTransactionDto>> GetLastNDaysAsync(Guid userId, int days, CancellationToken cancellationToken = default);
    }
}

