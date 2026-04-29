using FinanceDomain.contracts;
using FinanceDomain.Entities;
using FinanceDomain.Enums;
using FinanceInfrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FinanceInfrastructure.Persistence.Repositories
{
    public class FinancialTransactionRepository : IFinancialTransactionRepository

    {
        private readonly FinanceDbContext _ctx;
        private readonly DbSet<FinancialTransaction> _dbSet;

        public FinancialTransactionRepository(FinanceDbContext ctx)
        {
            _ctx = ctx;
            _dbSet = ctx.FinancialTransactions;
        }

        public async Task<FinancialTransaction?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await _dbSet.FirstOrDefaultAsync(t => t.Id == id, ct);

        public async Task<IEnumerable<FinancialTransaction>> GetAllAsync(CancellationToken ct = default) =>
            await _dbSet.ToListAsync(ct);

        public async Task<IEnumerable<FinancialTransaction>> FindAsync(
            Expression<Func<FinancialTransaction, bool>> predicate, CancellationToken ct = default) =>
            await _dbSet.Where(predicate).ToListAsync(ct);

        public async Task<FinancialTransaction?> FirstOrDefaultAsync(
            Expression<Func<FinancialTransaction, bool>> predicate, CancellationToken ct = default) =>
            await _dbSet.FirstOrDefaultAsync(predicate, ct);

        public async Task<bool> AnyAsync(
            Expression<Func<FinancialTransaction, bool>> predicate, CancellationToken ct = default) =>
            await _dbSet.AnyAsync(predicate, ct);

        public async Task<int> CountAsync(
            Expression<Func<FinancialTransaction, bool>>? predicate = null, CancellationToken ct = default) =>
            predicate == null
                ? await _dbSet.CountAsync(ct)
                : await _dbSet.CountAsync(predicate, ct);

        public async Task<FinancialTransaction> AddAsync(FinancialTransaction entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<FinancialTransaction> entities, CancellationToken ct = default) =>
            await _dbSet.AddRangeAsync(entities, ct);

        public void Update(FinancialTransaction entity) => _dbSet.Update(entity);
        public void Delete(FinancialTransaction entity) => entity.MarkAsDeleted();
        public void DeleteRange(IEnumerable<FinancialTransaction> entities)
        {
            foreach (var e in entities) e.MarkAsDeleted();
        }

        // ─── Custom Queries ──────────────────────────────────────────────────────

        public async Task<FinancialTransaction?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default) =>
            await _dbSet
                .Include(t => t.Wallet)
                .Include(t => t.FinancialCategory)
                .FirstOrDefaultAsync(t => t.Id == id, ct);

        public async Task<IEnumerable<FinancialTransaction>> GetByWalletIdAsync(
            Guid walletId, CancellationToken ct = default) =>
            await _dbSet
                .Include(t => t.FinancialCategory)
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(ct);

        public async Task<IEnumerable<FinancialTransaction>> GetByUserIdAsync(
            Guid userId, CancellationToken ct = default) =>
            await _dbSet
                .Include(t => t.Wallet)
                .Include(t => t.FinancialCategory)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(ct);

        public async Task<IEnumerable<FinancialTransaction>> GetByUserIdAndDateRangeAsync(
            Guid userId, DateTime from, DateTime to, CancellationToken ct = default) =>
            await _dbSet
                .Include(t => t.Wallet)
                .Include(t => t.FinancialCategory)
                .Where(t => t.UserId == userId
                         && t.TransactionDate >= from
                         && t.TransactionDate <= to)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(ct);

        public async Task<IEnumerable<FinancialTransaction>> GetByTypeAsync(
            Guid userId, TransactionType type, CancellationToken ct = default) =>
            await _dbSet
                .Include(t => t.Wallet)
                .Include(t => t.FinancialCategory)
                .Where(t => t.UserId == userId && t.Type == type)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(ct);

        public async Task<IEnumerable<FinancialTransaction>> GetByCategoryAsync(
            Guid userId, Guid categoryId, CancellationToken ct = default) =>
            await _dbSet
                .Include(t => t.Wallet)
                .Where(t => t.UserId == userId && t.FinancialCategoryId == categoryId)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync(ct);

        public async Task<decimal> GetTotalByTypeAsync(
            Guid userId, TransactionType type,
            DateTime? from = null, DateTime? to = null,
            CancellationToken ct = default)
        {
            var query = _dbSet.Where(t => t.UserId == userId && t.Type == type);

            if (from.HasValue) query = query.Where(t => t.TransactionDate >= from.Value);
            if (to.HasValue) query = query.Where(t => t.TransactionDate <= to.Value);

            return await query.SumAsync(t => t.Amount, ct);
        }

        public async Task HardDeleteByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            await _dbSet.Where(t => t.UserId == userId).ExecuteDeleteAsync(ct);
        }

        public async Task HardDeleteByWalletIdAsync(Guid walletId, CancellationToken ct = default)
        {
            await _dbSet.Where(t => t.WalletId == walletId).ExecuteDeleteAsync(ct);
        }
    }
}
