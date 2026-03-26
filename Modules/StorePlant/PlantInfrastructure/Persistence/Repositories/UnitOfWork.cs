using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using PlantDomain.Contracts;
using PlantInfrastructure.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlantInfrastructure.Persistence.Repositories
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly StorePlantDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(StorePlantDbContext context, ILogger<PlantRepository> plantRepositoryLogger)
        {
            _context = context;
            Plants = new PlantRepository(context, plantRepositoryLogger);
            UserPlants = new UserPlantRepository(context);
        }

        public IPlantRepository Plants { get; }
        public IUserPlantRepository UserPlants { get; }

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
            // _context.Dispose(); // DI container handles disposal of Scoped DbContext
        }
    }
}
