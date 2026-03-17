
namespace PlantDomain.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        IPlantRepository Plants { get; }
        IUserPlantRepository UserPlants { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
