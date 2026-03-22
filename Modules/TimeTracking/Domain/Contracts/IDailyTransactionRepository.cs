using SharedKernel;
using TimeTrackingDomain.Entities;

namespace TimeTrackingDomain.Contracts
{
    public interface IDailyTransactionRepository: IRepository<DailyTransaction>
    {
        Task<DailyTransaction?> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default);
        Task<IEnumerable<DailyTransaction>> GetByUserAndDateRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default);
        Task<int> GetCurrentStreakAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<DailyTransaction>> GetLastNDaysAsync(Guid userId, int days, CancellationToken cancellationToken = default);
    }
}

