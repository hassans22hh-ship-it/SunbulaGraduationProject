using Domain.Entities.TaskManagement;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskDomain.Contracts;
using TaskInfrastructure.Persistence.Data;

namespace TaskInfrastructure.Persistence.Repositories
{
    public sealed class FolderRepository:IFolderRepository
    {
        private readonly TaskManagementDbContext _context;
        private readonly DbSet<Folder> _dbSet;

        public FolderRepository(TaskManagementDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Folder>();
        }

        public async Task<Folder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<Folder>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(f => !f.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Folder>> FindAsync(
            Expression<Func<Folder, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(predicate)
                .Where(f => !f.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<Folder?> FirstOrDefaultAsync(
            Expression<Func<Folder, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(f => !f.IsDeleted)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<bool> AnyAsync(
            Expression<Func<Folder, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(f => !f.IsDeleted)
                .AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> CountAsync(
            Expression<Func<Folder, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return await _dbSet.Where(f => !f.IsDeleted).CountAsync(cancellationToken);

            return await _dbSet
                .Where(f => !f.IsDeleted)
                .CountAsync(predicate, cancellationToken);
        }

        public async Task<Folder> AddAsync(Folder entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<Folder> entities, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public void Update(Folder entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Folder entity)
        {
            entity.MarkAsDeleted();
        }

        public void DeleteRange(IEnumerable<Folder> entities)
        {
            foreach (var entity in entities)
            {
                entity.MarkAsDeleted();
            }
        }

        // Custom queries
        public async Task<Folder?> GetByIdWithTasksAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(f => f.Tasks)
                .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<Folder>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(f => f.UserId == userId && !f.IsDeleted)
                .OrderBy(f => f.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Folder?> GetByNameAsync(Guid userId, string name, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(f => f.UserId == userId && f.Name == name && !f.IsDeleted, cancellationToken);
        }

        public async Task<bool> NameExistsAsync(
            Guid userId,
            string name,
            Guid? excludeFolderId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(f => f.UserId == userId && f.Name == name && !f.IsDeleted);

            if (excludeFolderId.HasValue)
            {
                query = query.Where(f => f.Id != excludeFolderId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }

        public async Task<int> GetTaskCountAsync(Guid folderId, CancellationToken cancellationToken = default)
        {
            var folder = await _dbSet
                .Include(f => f.Tasks)
                .FirstOrDefaultAsync(f => f.Id == folderId && !f.IsDeleted, cancellationToken);

            return folder?.Tasks.Count ?? 0;
        }

        public async Task HardDeleteByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            await _dbSet.Where(f => f.UserId == userId).ExecuteDeleteAsync(ct);
        }
    }
}
