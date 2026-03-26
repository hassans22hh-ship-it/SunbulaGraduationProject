using PlantDomain.Entities;
using SharedKernel;
using System.Linq.Expressions;

namespace PlantDomain.Contracts
{
    public interface IUserPlantRepository:IRepository<UserPlant>
    {
        Task<UserPlant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserPlant>> FindAsync(Expression<Func<UserPlant, bool>> predicate, CancellationToken cancellationToken = default);

        // Custom queries
        Task<IEnumerable<UserPlant>> GetGardenByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserPlant?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> UserOwnsPlantAsync(Guid userId, Guid plantId, CancellationToken cancellationToken = default);
        Task<int> CountByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserPlant?> GetFirstPurchasedAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserPlant?> GetMostExpensiveAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<int> GetTotalCoinsSpentAsync(Guid userId, CancellationToken cancellationToken = default);
        Task HardDeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
