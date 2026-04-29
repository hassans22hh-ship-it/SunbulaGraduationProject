using DebtInfrastructure.Persistenece.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using DebtDomain.Entities;
using DebtDomain.Enums;
using DebtDomain.Contracts;
using SharedKernel;

namespace DebtInfrastructure.Persistenece.Repositories
{
    public class DebtRepository: IDebtRepository
    {
        private readonly DebtDbContext _context;
        private readonly DbSet<Debt> _dbSet;

        public DebtRepository(DebtDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Debt>();
        }

        // ═══════════════════════════════════════════════════════════════
        // IRepository<T> Implementation
        // ═══════════════════════════════════════════════════════════════

        public async Task<Debt?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<Debt>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Debt>> FindAsync(
            Expression<Func<Debt, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(predicate)
                .Where(e => !e.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<Debt?> FirstOrDefaultAsync(
            Expression<Func<Debt, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<bool> AnyAsync(
            Expression<Func<Debt, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(e => !e.IsDeleted)
                .AnyAsync(predicate, cancellationToken);
        }

        public async Task<int> CountAsync(
            Expression<Func<Debt, bool>>? predicate = null,
            CancellationToken cancellationToken = default)
        {
            if (predicate == null)
                return await _dbSet.Where(e => !e.IsDeleted).CountAsync(cancellationToken);

            return await _dbSet
                .Where(e => !e.IsDeleted)
                .CountAsync(predicate, cancellationToken);
        }

        public async Task<Debt> AddAsync(
            Debt entity,
            CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public async Task AddRangeAsync(
            IEnumerable<Debt> entities,
            CancellationToken cancellationToken = default)
        {
            await _dbSet.AddRangeAsync(entities, cancellationToken);
        }

        public void Update(Debt entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(Debt entity)
        {
            entity.MarkAsDeleted(); // Soft delete
        }

        public void DeleteRange(IEnumerable<Debt> entities)
        {
            foreach (var entity in entities)
            {
                entity.MarkAsDeleted();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // Custom Query Methods - Direct Implementation
        // ═══════════════════════════════════════════════════════════════

        public async Task<Debt?> GetByIdWithPaymentsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Payments.Where(p => !p.IsDeleted))
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);
        }

        public async Task<IEnumerable<Debt>> GetByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d => d.UserId == userId && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Debt>> GetUnpaidByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d => d.UserId == userId && !d.IsPaid && !d.IsDeleted)
                .OrderBy(d => d.DueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Debt>> GetOverdueByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;

            return await _dbSet
                .Where(d => d.UserId == userId
                         && !d.IsPaid
                         && d.DueDate < today
                         && !d.IsDeleted)
                .OrderBy(d => d.DueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Debt>> GetByTypeAsync(
            Guid userId,
            string debtType,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d => d.UserId == userId && d.DebtType == debtType && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<decimal> GetTotalDebtAmountAsync(
            Guid userId,
            string debtType,
            bool unpaidOnly = true,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet
                .Where(d => d.UserId == userId && d.DebtType == debtType && !d.IsDeleted);

            if (unpaidOnly)
                query = query.Where(d => !d.IsPaid);

            var total = await query.SumAsync(d =>(decimal) d.Amount , cancellationToken);

            return total;
        }

        public async Task<decimal> GetTotalRemainingAmountAsync(
            Guid userId,
            string debtType,
            CancellationToken cancellationToken = default)
            => await _dbSet
                .Where(d => d.UserId == userId && d.DebtType == debtType && !d.IsDeleted)
                .SumAsync(d => (decimal)d.RemainingAmount, cancellationToken);

        public async Task HardDeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _dbSet.Where(d => d.UserId == userId).ExecuteDeleteAsync(cancellationToken);
        }

        public async Task HardDeletePaymentsByDebtIdAsync(Guid debtId, CancellationToken cancellationToken = default)
        {
            await _context.DebtPayments.Where(p => p.DebtId == debtId).ExecuteDeleteAsync(cancellationToken);
        }
    }
}
