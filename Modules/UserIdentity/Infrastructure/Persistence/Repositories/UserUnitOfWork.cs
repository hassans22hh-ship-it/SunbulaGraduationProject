using Domain.Contracts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace UserIdentityInfrastructure.Persistence.Repositories
{
    public sealed class UserUnitOfWork : IUnitOfWork
    {
        private readonly UserIdentityDbContext _context;
        private IDbContextTransaction? _transaction;
        public UserUnitOfWork(UserIdentityDbContext context, IUserRepository userRepository)
        {
            _context = context;
            Users = userRepository;
        }
        public IUserRepository Users { get; }

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
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
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

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
