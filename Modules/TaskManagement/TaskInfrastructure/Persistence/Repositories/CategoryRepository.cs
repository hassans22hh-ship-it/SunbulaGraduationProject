using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskDomain.Contracts;
using TaskDomain.Entities.TaskManagement;
using TaskInfrastructure.Persistence.Data;

namespace TaskInfrastructure.Persistence.Repositories
{
    public sealed class CategoryRepository :ICategoryRepository
    {
        private readonly TaskManagementDbContext _context;
        private readonly DbSet<Category> _dbSet;

        public CategoryRepository(TaskManagementDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Category>();
        }

        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => !c.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Category>> FindAsync(
            Expression<Func<Category, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(predicate)
                .Where(c => !c.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<Category?> FirstOrDefaultAsync(
            Expression<Func<Category, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => !c.IsDeleted)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<bool> AnyAsync(
            Expression<Func<Category, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => !c.IsDeleted)
                .AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> CountAsync(
            Expression<Func<Category, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return await _dbSet.Where(c => !c.IsDeleted).CountAsync(cancellationToken);

            return await _dbSet
                .Where(c => !c.IsDeleted)
                .CountAsync(predicate, cancellationToken);
        }

        public async Task<Category> AddAsync(Category entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<Category> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public void Update(Category entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Category entity)
        {
            entity.MarkAsDeleted();
        }

        public void DeleteRange(IEnumerable<Category> entities)
        {
            foreach (var entity in entities)
            {
                entity.MarkAsDeleted();
            }
        }

        // Custom queries
        public async Task<Category?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.TaskCategories)
                    .ThenInclude(tc => tc.Task)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<Category>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Category?> GetByNameAsync(Guid userId, string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Name == name && !c.IsDeleted, cancellationToken);
        }

        public async Task<bool> NameExistsAsync(
            Guid userId,
            string name,
            Guid? excludeCategoryId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(c => c.UserId == userId && c.Name == name && !c.IsDeleted);

            if (excludeCategoryId.HasValue)
            {
                query = query.Where(c => c.Id != excludeCategoryId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
    }
}
