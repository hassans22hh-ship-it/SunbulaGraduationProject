using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlantDomain.Contracts;
using PlantDomain.Entities;
using PlantDomain.Enums;
using PlantInfrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace PlantInfrastructure.Persistence.Repositories
{
    public class PlantRepository : IPlantRepository
    {
        private readonly StorePlantDbContext _context;
        private readonly DbSet<Plant> _dbSet;
        private readonly ILogger<PlantRepository> _logger;

        public PlantRepository(StorePlantDbContext context, ILogger<PlantRepository> logger)
        {
            _context = context;
            _dbSet = context.Set<Plant>();
            _logger = logger;
        }

        // ── IRepository<Plant> ────────────────────────────────────────────

        public async Task<Plant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => await _dbSet.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

        public async Task<IEnumerable<Plant>> GetAllAsync(CancellationToken cancellationToken = default)
            => await _dbSet.Where(p => !p.IsDeleted).OrderBy(p => p.Level).ThenBy(p => p.Price).ToListAsync(cancellationToken);

        public async Task<IEnumerable<Plant>> FindAsync(Expression<Func<Plant, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(p => !p.IsDeleted).Where(predicate).ToListAsync(cancellationToken);

        public async Task<Plant?> FirstOrDefaultAsync(Expression<Func<Plant, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(p => !p.IsDeleted).FirstOrDefaultAsync(predicate, cancellationToken);

        public async Task<bool> AnyAsync(Expression<Func<Plant, bool>> predicate, CancellationToken cancellationToken = default)
            => await _dbSet.Where(p => !p.IsDeleted).AnyAsync(predicate, cancellationToken);

        public async Task<int> CountAsync(Expression<Func<Plant, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return await _dbSet.Where(p => !p.IsDeleted).CountAsync(cancellationToken);
            return await _dbSet.Where(p => !p.IsDeleted).CountAsync(predicate, cancellationToken);
        }

        public async Task<Plant> AddAsync(Plant entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<Plant> entities, CancellationToken cancellationToken = default)
            => await _dbSet.AddRangeAsync(entities, cancellationToken);

        public void Update(Plant entity) => _dbSet.Update(entity);

        public void Delete(Plant entity) => entity.MarkAsDeleted();

        public void DeleteRange(IEnumerable<Plant> entities)
        {
            foreach (var e in entities) e.MarkAsDeleted();
        }

        // ── Custom Queries ────────────────────────────────────────────────

        public async Task<IEnumerable<Plant>> GetAvailablePlantsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbSet
                    .Where(p => p.IsAvailable && !p.IsDeleted)
                    .OrderBy(p => p.Level)
                    .ThenBy(p => p.Price)
                    .ToListAsync(cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                // Senior Eng: Capture details when opening connection fails
                _logger.LogError(ex, "Timeout fetching plants. Context: {db}", _context.Database.GetConnectionString());
                throw; 
            }
            catch (Exception ex)
            {
                // Catch potential SqlException timeouts that manifest as TaskCanceledException
                _logger.LogError(ex, "Exception fetching plants. Context: {db}", _context.Database.GetConnectionString());
                throw;
            }
        }

        public async Task<IEnumerable<Plant>> GetByLevelAsync(PlantLevel level, CancellationToken cancellationToken = default)
            => await _dbSet
                .Where(p => p.Level == level && p.IsAvailable && !p.IsDeleted)
                .OrderBy(p => p.Price)
                .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Plant>> GetSeasonalPlantsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(p => p.IsSeasonal && p.IsAvailable && !p.IsDeleted &&
                            p.SeasonStart <= now && p.SeasonEnd >= now)
                .OrderBy(p => p.SeasonEnd)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
            => await _dbSet.AnyAsync(p => p.Name == name && !p.IsDeleted, cancellationToken);
    }
}
