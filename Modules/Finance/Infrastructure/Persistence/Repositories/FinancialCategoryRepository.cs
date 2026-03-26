using FinanceDomain.contracts;
using FinanceDomain.Entities;
using FinanceInfrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinanceInfrastructure.Persistence.Repositories
{
    public class FinancialCategoryRepository: IFinancialCategoryRepository
    {
        private readonly FinanceDbContext _ctx;
        private readonly DbSet<FinancialCategory> _dbSet;

        public FinancialCategoryRepository(FinanceDbContext ctx)
        {
            _ctx = ctx;
            _dbSet = ctx.FinancialCategories;
        }

        public async Task<FinancialCategory?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await _dbSet.FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task<IEnumerable<FinancialCategory>> GetAllAsync(CancellationToken ct = default) =>
            await _dbSet.ToListAsync(ct);

        public async Task<IEnumerable<FinancialCategory>> FindAsync(
            Expression<Func<FinancialCategory, bool>> predicate, CancellationToken ct = default) =>
            await _dbSet.Where(predicate).ToListAsync(ct);

        public async Task<FinancialCategory?> FirstOrDefaultAsync(
            Expression<Func<FinancialCategory, bool>> predicate, CancellationToken ct = default) =>
            await _dbSet.FirstOrDefaultAsync(predicate, ct);

        public async Task<bool> AnyAsync(
            Expression<Func<FinancialCategory, bool>> predicate, CancellationToken ct = default) =>
            await _dbSet.AnyAsync(predicate, ct);

        public async Task<int> CountAsync(
            Expression<Func<FinancialCategory, bool>>? predicate = null, CancellationToken ct = default) =>
            predicate == null
                ? await _dbSet.CountAsync(ct)
                : await _dbSet.CountAsync(predicate, ct);

        public async Task<FinancialCategory> AddAsync(FinancialCategory entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<FinancialCategory> entities, CancellationToken ct = default) =>
            await _dbSet.AddRangeAsync(entities, ct);

        public void Update(FinancialCategory entity) => _dbSet.Update(entity);
        public void Delete(FinancialCategory entity) => entity.MarkAsDeleted();
        public void DeleteRange(IEnumerable<FinancialCategory> entities)
        {
            foreach (var e in entities) e.MarkAsDeleted();
        }

        // ─── Custom ─────────────────────────────────────────────────────────────

        public async Task<IEnumerable<FinancialCategory>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
            await _dbSet
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.Name)
                .ToListAsync(ct);

        public async Task<bool> NameExistsAsync(Guid userId, string name, CancellationToken ct = default) =>
            await _dbSet.AnyAsync(c => c.UserId == userId && c.Name == name, ct);

        public async Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken ct = default)
            => await _ctx.FinancialTransactions.AnyAsync(t => t.FinancialCategoryId == categoryId, ct);

        public async Task HardDeleteByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            await _dbSet.Where(c => c.UserId == userId).ExecuteDeleteAsync(ct);
        }
    }
}
