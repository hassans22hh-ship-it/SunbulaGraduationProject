using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TimeTrackingDomain.Contracts;
using TimeTrackingDomain.Entities;
using TimeTrackingInfrastructure.Persistence.Data;

namespace TimeTrackingInfrastructure.Persistence.Repositories
{
    public sealed class TimeSessionRepository : ITimeSessionRepository
    {
        private readonly TimeTrackingDbContext _context;
        private readonly DbSet<TimeSession> _dbSet;

        public TimeSessionRepository(TimeTrackingDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TimeSession>();
        }

        public async Task<TimeSession?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);

        public async Task<IEnumerable<TimeSession>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => !e.IsDeleted).ToListAsync(cancellationToken);

        public async Task<IEnumerable<TimeSession>> FindAsync(Expression<Func<TimeSession, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync(cancellationToken);

        public async Task<TimeSession?> FirstOrDefaultAsync(Expression<Func<TimeSession, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => !e.IsDeleted).FirstOrDefaultAsync(predicate, cancellationToken);

        public async Task<bool> AnyAsync(Expression<Func<TimeSession, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => !e.IsDeleted).AnyAsync(predicate, cancellationToken);

        public async Task<int> CountAsync(Expression<Func<TimeSession, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(e => !e.IsDeleted);
            return predicate == null ? await query.CountAsync(cancellationToken) : await query.CountAsync(predicate, cancellationToken);
        }

        public async Task<TimeSession> AddAsync(TimeSession entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<TimeSession> entities, CancellationToken cancellationToken = default)
            => await _dbSet.AddRangeAsync(entities, cancellationToken);

        public void Update(TimeSession entity) => _dbSet.Update(entity);
        public void Delete(TimeSession entity) => entity.MarkAsDeleted();
        public void DeleteRange(IEnumerable<TimeSession> entities) { foreach (var entity in entities) entity.MarkAsDeleted(); }

        public async Task<int> GetTotalMinutesByUserIdAndDateAsync(Guid userId, DateOnly date, CancellationToken ct = default)
            => await _dbSet
                .Where(ts => ts.UserId == userId && !ts.IsDeleted && ts.Date == date)
                .SumAsync(ts => ts.DurationMinutes, ct);

        public async Task HardDeleteByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            await _dbSet.Where(ts => ts.UserId == userId).ExecuteDeleteAsync(ct);
        }

        public async Task<IEnumerable<TimeSession>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => e.UserId == userId && !e.IsDeleted).OrderByDescending(e => e.StartTime).ToListAsync(cancellationToken);

        public async Task<IEnumerable<TimeSession>> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => e.TaskId == taskId && !e.IsDeleted).OrderByDescending(e => e.StartTime).ToListAsync(cancellationToken);

        public async Task<IEnumerable<TimeSession>> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => e.UserId == userId && !e.IsDeleted && e.Date == date)
                .OrderBy(e => e.StartTime).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TimeSession>> GetByUserAndDateRangeAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => e.UserId == userId && !e.IsDeleted && e.Date >= from && e.Date <= to)
                .OrderBy(e => e.StartTime).ToListAsync(cancellationToken);
        }

        public async Task<TimeSession?> GetActiveSessionByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(e => e.UserId == userId && e.IsActive && !e.IsDeleted, cancellationToken);

        public async Task<TimeSession?> GetActiveSessionByUserAndTaskAsync(Guid userId, Guid taskId, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(e => e.UserId == userId && e.TaskId == taskId && e.IsActive && !e.IsDeleted, cancellationToken);

        public async Task<IEnumerable<TimeSession>> GetActiveSessionsByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet.Where(e => e.UserId == userId && e.IsActive && !e.IsDeleted).OrderBy(e => e.StartTime).ToListAsync(cancellationToken);

        public async Task<bool> HasActiveSessionAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(e => e.UserId == userId && e.IsActive && !e.IsDeleted, cancellationToken);

        public async Task<IEnumerable<TimeSession>> GetOverlappingSessionsAsync(Guid userId, DateTime startTime, DateTime endTime, Guid? taskId = null, Guid? excludeSessionId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(e => e.UserId == userId && !e.IsDeleted && !e.IsActive && e.EndTime != null && e.StartTime < endTime && e.EndTime > startTime);
            if (taskId.HasValue) query = query.Where(e => e.TaskId == taskId.Value);
            if (excludeSessionId.HasValue) query = query.Where(e => e.Id != excludeSessionId.Value);
            return await query.ToListAsync(cancellationToken);
        }

        public async Task<int> GetSessionCountByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
            => await _dbSet.CountAsync(e => e.TaskId == taskId && !e.IsDeleted, cancellationToken);

        public async Task<(IEnumerable<TimeSession> Sessions, int TotalCount)> GetPagedByUserIdAsync(Guid userId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(e => e.UserId == userId && !e.IsDeleted).OrderByDescending(e => e.StartTime);
            var total = await query.CountAsync(cancellationToken);
            var sessions = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return (sessions, total);
        }
    }
}
