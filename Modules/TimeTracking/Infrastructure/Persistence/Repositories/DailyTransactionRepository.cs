using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeTrackingDomain.Contracts;
using TimeTrackingDomain.Entities;
using TimeTrackingInfrastructure.Persistence.Data;

namespace TimeTrackingInfrastructure.Persistence.Repositories
{
    public class DailyTransactionRepository: IDailyTransactionRepository
    {
        private readonly TimeTrackingDbContext _context;
        private readonly DbSet<DailyTransaction> _dbSet;

        public DailyTransactionRepository(TimeTrackingDbContext context)
        {
            _context = context;
            _dbSet = context.Set<DailyTransaction>();
        }

        public async Task<DailyTransaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);

        public async Task<IEnumerable<DailyTransaction>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => !e.IsDeleted).ToListAsync(cancellationToken);

        public async Task<IEnumerable<DailyTransaction>> FindAsync(Expression<Func<DailyTransaction, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync(cancellationToken);

        public async Task<DailyTransaction?> FirstOrDefaultAsync(Expression<Func<DailyTransaction, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => !e.IsDeleted).FirstOrDefaultAsync(predicate, cancellationToken);

        public async Task<bool> AnyAsync(Expression<Func<DailyTransaction, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => !e.IsDeleted).AnyAsync(predicate, cancellationToken);

        public async Task<int> CountAsync(Expression<Func<DailyTransaction, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(e => !e.IsDeleted);
            return predicate == null ? await query.CountAsync(cancellationToken) : await query.CountAsync(predicate, cancellationToken);
        }

        public async Task<DailyTransaction> AddAsync(DailyTransaction entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<DailyTransaction> entities, CancellationToken cancellationToken = default)
            => await _dbSet.AddRangeAsync(entities, cancellationToken);

        public void Update(DailyTransaction entity) => _dbSet.Update(entity);
        public void Delete(DailyTransaction entity) => entity.MarkAsDeleted();
        public void DeleteRange(IEnumerable<DailyTransaction> entities) { foreach (var entity in entities) entity.MarkAsDeleted(); }

        public async Task<IEnumerable<DailyTransaction>> GetRecentByUserIdAsync(Guid userId, int days, CancellationToken ct = default) =>
            await _dbSet
                .Where(dt => dt.UserId == userId && !dt.IsDeleted)
                .OrderByDescending(dt => dt.Date)
                .Take(days)
                .ToListAsync(ct);

        public async Task HardDeleteByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            await _dbSet.Where(dt => dt.UserId == userId).ExecuteDeleteAsync(ct);
        }

        public async Task<DailyTransaction?> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(e => e.UserId == userId && e.Date == date && !e.IsDeleted, cancellationToken);

        public async Task<IEnumerable<DailyTransaction>> GetByUserAndDateRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => e.UserId == userId && !e.IsDeleted && e.Date >= from && e.Date <= to).OrderBy(e => e.Date).ToListAsync(cancellationToken);

        public async Task<int> GetCurrentStreakAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var localNow = DateTime.UtcNow;
            try { localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")); } catch { localNow = localNow.AddHours(3); }
            var today = DateOnly.FromDateTime(localNow);
            var recentDays = await _dbSet
                .Where(e => e.UserId == userId && !e.IsDeleted && e.TotalMinutes >= 30 && e.Date <= today)
                .OrderByDescending(e => e.Date).Select(e => e.Date).Take(60)
                .ToListAsync(cancellationToken);

            if (!recentDays.Any()) return 0;

            var firstDay = recentDays.First();
            if (firstDay < today.AddDays(-1)) return 0;

            int streak = 1;
            for (int i = 1; i < recentDays.Count; i++)
            {
                if (recentDays[i - 1].DayNumber - recentDays[i].DayNumber == 1) streak++;
                else break;
            }
            return streak;
        }

        public async Task<IEnumerable<DailyTransaction>> GetLastNDaysAsync(Guid userId, int days, CancellationToken cancellationToken = default)
        {
            var localNow = DateTime.UtcNow;
            try { localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time")); } catch { localNow = localNow.AddHours(3); }
            var from = DateOnly.FromDateTime(localNow.AddDays(-days));
            return await _dbSet.Where(e => e.UserId == userId && !e.IsDeleted && e.Date >= from).OrderBy(e => e.Date).ToListAsync(cancellationToken);
        }
    }
}

