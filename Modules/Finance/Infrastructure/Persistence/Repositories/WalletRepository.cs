using FinanceDomain.contracts;
using FinanceDomain.Entities;
using FinanceInfrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using System.Linq.Expressions;

namespace FinanceInfrastructure.Persistence.Repositories
{
    public class WalletRepository: IWalletRepository
    {
        private readonly FinanceDbContext _ctx;
        private readonly DbSet<Wallet> _dbSet;

        public WalletRepository(FinanceDbContext ctx)
        {
            _ctx = ctx;
            _dbSet = ctx.Wallets;
        }

        // ─── IRepository<Wallet> ────────────────────────────────────────────────

        public async Task<Wallet?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await _dbSet.FirstOrDefaultAsync(w => w.Id == id, ct);

        public async Task<IEnumerable<Wallet>> GetAllAsync(CancellationToken ct = default) =>
            await _dbSet.ToListAsync(ct);

        public async Task<IEnumerable<Wallet>> FindAsync(
            Expression<Func<Wallet, bool>> predicate, CancellationToken ct = default) =>
            await _dbSet.Where(predicate).ToListAsync(ct);

        public async Task<Wallet?> FirstOrDefaultAsync(
            Expression<Func<Wallet, bool>> predicate, CancellationToken ct = default) =>
            await _dbSet.FirstOrDefaultAsync(predicate, ct);

        public async Task<bool> AnyAsync(
            Expression<Func<Wallet, bool>> predicate, CancellationToken ct = default) =>
            await _dbSet.AnyAsync(predicate, ct);

        public async Task<int> CountAsync(
            Expression<Func<Wallet, bool>>? predicate = null, CancellationToken ct = default) =>
            predicate == null
                ? await _dbSet.CountAsync(ct)
                : await _dbSet.CountAsync(predicate, ct);

        public async Task<Wallet> AddAsync(Wallet entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<Wallet> entities, CancellationToken ct = default) =>
            await _dbSet.AddRangeAsync(entities, ct);

        public void Update(Wallet entity) => _dbSet.Update(entity);
        public void Delete(Wallet entity) => entity.MarkAsDeleted();
        public void DeleteRange(IEnumerable<Wallet> entities)
        {
            foreach (var e in entities) e.MarkAsDeleted();
        }

        // ─── Custom Queries ──────────────────────────────────────────────────────

        public async Task<Wallet?> GetByIdWithTransactionsAsync(Guid id, CancellationToken ct = default) =>
            await _dbSet
                .Include(w => w.Transactions)
                .FirstOrDefaultAsync(w => w.Id == id, ct);

        public async Task<IEnumerable<Wallet>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
            await _dbSet
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync(ct);

        public async Task<bool> NameExistsAsync(Guid userId, string name, CancellationToken ct = default) =>
            await _dbSet.AnyAsync(w => w.UserId == userId && w.Name == name, ct);

        public async Task<decimal> GetTotalBalanceByUserIdAsync(Guid userId, string currency, CancellationToken ct = default)
            => await _dbSet
                .Where(w => w.UserId == userId && w.Balance.Currency == currency && !w.IsDeleted)
                .SumAsync(w => w.Balance.Amount, ct);

        public async Task HardDeleteByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            await _dbSet.Where(w => w.UserId == userId).ExecuteDeleteAsync(ct);
        }
    }
}
