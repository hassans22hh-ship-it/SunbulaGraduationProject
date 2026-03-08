using FinanceDomain.contracts;
using FinanceInfrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace FinanceInfrastructure.Persistence.Repositories
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly FinanceDbContext _ctx;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(FinanceDbContext ctx)
        {
            _ctx = ctx;
            Wallets = new WalletRepository(ctx);
            Transactions = new FinancialTransactionRepository(ctx);
            FinancialCategories = new FinancialCategoryRepository(ctx);
        }

        public IWalletRepository Wallets { get; }
        public IFinancialTransactionRepository Transactions { get; }
        public IFinancialCategoryRepository FinancialCategories { get; }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
            await _ctx.SaveChangesAsync(ct);

        public async Task BeginTransactionAsync(CancellationToken ct = default) =>
            _transaction = await _ctx.Database.BeginTransactionAsync(ct);

        public async Task CommitTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction is not null)
            {
                await _transaction.CommitAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync(ct);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _ctx.Dispose();
        }
    }


}
