using Domain.Entities.TaskManagement;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskDomain.Contracts;
using TaskDomain.Entities.TaskManagement.Enums;
using TaskInfrastructure.Persistence.Data;
using TaskStatus = TaskDomain.Entities.TaskManagement.Enums.TaskStatus;

namespace TaskInfrastructure.Persistence.Repositories
{
    public sealed class TaskRepository:ITaskRepository
    {
        private readonly TaskManagementDbContext _context;
        private readonly DbSet<TaskItem> _dbSet;

        public TaskRepository(TaskManagementDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TaskItem>();
        }

        // ═══════════════════════════════════════════════════════════════
        // IRepository<T> Implementation
        // ═══════════════════════════════════════════════════════════════

        public async Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(t => !t.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TaskItem>> FindAsync(
            Expression<Func<TaskItem ,bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(predicate)
                .Where(t => !t.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<TaskItem?> FirstOrDefaultAsync(
            Expression<Func<TaskItem, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(t => !t.IsDeleted)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<bool> AnyAsync(
            Expression<Func<TaskItem, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(t => !t.IsDeleted)
                .AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> CountAsync(
            Expression<Func<TaskItem, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return await _dbSet.Where(t => !t.IsDeleted).CountAsync(cancellationToken);

            return await _dbSet
                .Where(t => !t.IsDeleted)
                .CountAsync(predicate, cancellationToken);
        }

        public async Task<TaskItem> AddAsync(TaskItem entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<TaskItem> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public void Update(TaskItem entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(TaskItem entity)
        {
            entity.MarkAsDeleted(); // Soft delete
        }

        public void DeleteRange(IEnumerable<TaskItem> entities)
        {
            foreach (var entity in entities)
            {
                entity.MarkAsDeleted();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // Custom Query Methods - Direct Implementation
        // ═══════════════════════════════════════════════════════════════

        public async Task<TaskItem?> GetByIdWithCategoriesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.TaskCategories)
                    .ThenInclude(tc => tc.Category)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<TaskItem?> GetByIdWithFolderAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(t => t.Folder)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken);
        }

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(t => t.UserId == userId && !t.IsDeleted).OrderByDescending(t => t.CreatedAt);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetActiveByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(t => t.UserId == userId && !t.IsArchived && t.Status == TaskStatus.Active && !t.IsDeleted).OrderBy(t => t.Title);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetArchivedByUserIdAsync(Guid userId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(t => t.UserId == userId && t.IsArchived && !t.IsDeleted).OrderByDescending(t => t.UpdatedAt);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetByFolderIdAsync(Guid folderId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(t => t.FolderId == folderId && !t.IsDeleted).OrderBy(t => t.Title);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetByCategoryIdAsync(Guid categoryId, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(t => t.TaskCategories.Any(tc => tc.CategoryId == categoryId) && !t.IsDeleted).OrderBy(t => t.Title);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> GetByBehaviorTypeAsync(
            Guid userId,
            BehaviorCategory behaviorType,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(t => t.UserId == userId && t.BehaviorType == behaviorType && !t.IsDeleted).OrderBy(t => t.Title);
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task<bool> TitleExistsAsync(
            Guid userId,
            string title,
            Guid? excludeTaskId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(t => t.UserId == userId && t.Title == title && !t.IsDeleted);

            if (excludeTaskId.HasValue)
            {
                query = query.Where(t => t.Id != excludeTaskId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<(IEnumerable<TaskItem> Items, int TotalCount)> SearchByTitleAsync(
            Guid userId,
            string query,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var lowercaseQuery = query.ToLower();
            var baseQuery = _dbSet.Where(t => t.UserId == userId && !t.IsDeleted && t.Title.ToLower().Contains(lowercaseQuery)).OrderBy(t => t.Title);
            var totalCount = await baseQuery.CountAsync(cancellationToken);
            var items = await baseQuery.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return (items, totalCount);
        }

        public async Task<IEnumerable<TaskItem>> GetRecentAsync(
            Guid userId,
            int count = 10,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(t => t.UserId == userId && !t.IsDeleted)
                .OrderByDescending(t => t.UpdatedAt ?? t.CreatedAt)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> CountByFolderIdAsync(Guid folderId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(t => t.FolderId == folderId && !t.IsDeleted)
                .CountAsync(cancellationToken);
        }

        public async Task HardDeleteByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            await _dbSet.Where(t => t.UserId == userId).ExecuteDeleteAsync(ct);
        }
    }
}
