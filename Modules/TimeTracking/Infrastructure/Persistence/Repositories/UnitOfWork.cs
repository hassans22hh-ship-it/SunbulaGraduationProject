using Microsoft.EntityFrameworkCore.Storage;
using TimeTrackingDomain.Contracts;
using TimeTrackingInfrastructure.Persistence.Data;

namespace TimeTrackingInfrastructure.Persistence.Repositories
{
    public class UnitOfWork: IUnitOfWork
    {
        private readonly TimeTrackingDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(TimeTrackingDbContext context)
        {
            _context = context;
            TimeSessions = new TimeSessionRepository(context);
            DailyTransactions = new DailyTransactionRepository(context);
        }

        public ITimeSessionRepository TimeSessions { get; }
        public IDailyTransactionRepository DailyTransactions { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
            => _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}

