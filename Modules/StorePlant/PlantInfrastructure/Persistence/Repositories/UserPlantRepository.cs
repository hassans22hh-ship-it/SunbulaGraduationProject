using Microsoft.EntityFrameworkCore;
using PlantDomain.Contracts;
using PlantDomain.Entities;
using PlantInfrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PlantInfrastructure.Persistence.Repositories
{
    public class UserPlantRepository : IUserPlantRepository
    {
        private readonly StorePlantDbContext _context;
        private readonly DbSet<UserPlant> _dbSet;

        public UserPlantRepository(StorePlantDbContext context)
        {
            _context = context;
            _dbSet = context.Set<UserPlant>();
        }

        // ── IRepository<UserPlant> ────────────────────────────────────────

        public async Task<UserPlant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(up => up.Id == id && !up.IsDeleted, cancellationToken);

        public async Task<IEnumerable<UserPlant>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.Where(up => !up.IsDeleted).ToListAsync(cancellationToken);

        public async Task<IEnumerable<UserPlant>> FindAsync(Expression<Func<UserPlant, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(up => !up.IsDeleted).Where(predicate).ToListAsync(cancellationToken);

        public async Task<UserPlant?> FirstOrDefaultAsync(Expression<Func<UserPlant, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(up => !up.IsDeleted).FirstOrDefaultAsync(predicate, cancellationToken);

        public async Task<bool> AnyAsync(Expression<Func<UserPlant, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(up => !up.IsDeleted).AnyAsync(predicate, cancellationToken);

        public async Task<int> CountAsync(Expression<Func<UserPlant, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return await _dbSet.Where(up => !up.IsDeleted).CountAsync(cancellationToken);
            return await _dbSet.Where(up => !up.IsDeleted).CountAsync(predicate, cancellationToken);
        }

        public async Task<UserPlant> AddAsync(UserPlant entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<UserPlant> entities, CancellationToken cancellationToken = default)
            => await _dbSet.AddRangeAsync(entities, cancellationToken);

        public void Update(UserPlant entity) => _dbSet.Update(entity);

        public void Delete(UserPlant entity) => entity.MarkAsDeleted();

        public void DeleteRange(IEnumerable<UserPlant> entities)
        {
            foreach (var e in entities) e.MarkAsDeleted();
        }

        // ── Custom Queries ────────────────────────────────────────────────

        public async Task<IEnumerable<UserPlant>> GetGardenByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet
                .Include(up => up.Plant)
                .Include(up => up.GrowthHistories)
                .Where(up => up.UserId == userId && !up.IsDeleted)
                .OrderByDescending(up => up.PurchaseDate)
                .ToListAsync(cancellationToken);

        public async Task<UserPlant?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbSet
                .Include(up => up.Plant)
                .Include(up => up.GrowthHistories)
                .FirstOrDefaultAsync(up => up.Id == id && !up.IsDeleted, cancellationToken);

        public async Task<bool> UserOwnsPlantAsync(Guid userId, Guid plantId, CancellationToken cancellationToken = default)
            => await _dbSet
                .AnyAsync(up => up.UserId == userId && up.PlantId == plantId && !up.IsDeleted, cancellationToken);

        public async Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet
                .CountAsync(up => up.UserId == userId && !up.IsDeleted, cancellationToken);

        public async Task<UserPlant?> GetFirstPurchasedAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet
                .Include(up => up.Plant)
                .Where(up => up.UserId == userId && !up.IsDeleted)
                .OrderBy(up => up.PurchaseDate)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<UserPlant?> GetMostExpensiveAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet
                .Include(up => up.Plant)
                .Where(up => up.UserId == userId && !up.IsDeleted)
                .OrderByDescending(up => up.CoinsSpent)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<int> GetTotalCoinsSpentAsync(Guid userId, CancellationToken cancellationToken = default)
            => await _dbSet
                .Where(up => up.UserId == userId && !up.IsDeleted)
                .SumAsync(up => up.CoinsSpent, cancellationToken);

        public async Task HardDeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _dbSet.Where(up => up.UserId == userId).ExecuteDeleteAsync(cancellationToken);
        }
    }
}
