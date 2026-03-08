using Microsoft.EntityFrameworkCore.Storage;
using TaskDomain.Contracts;
using TaskInfrastructure.Persistence.Data;

namespace TaskInfrastructure.Persistence.Repositories
{
    public sealed class UnitOfWork: IUnitOfWork
    {
        private readonly TaskManagementDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(TaskManagementDbContext context)
        {
            _context = context;
            Tasks = new TaskRepository(context);
            Categories = new CategoryRepository(context);
            Folders = new FolderRepository(context);
        }

        public ITaskRepository Tasks { get; }
        public ICategoryRepository Categories { get; }
        public IFolderRepository Folders { get; }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

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
