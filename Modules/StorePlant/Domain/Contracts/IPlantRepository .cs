using PlantDomain.Entities;
using PlantDomain.Enums;
using SharedKernel;
using System.Linq.Expressions;

namespace PlantDomain.Contracts
{
    public interface IPlantRepository:IRepository<Plant>
    {
        Task<Plant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Plant>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Plant>> FindAsync(Expression<Func<Plant, bool>> predicate, CancellationToken cancellationToken = default);

        // Custom queries
        Task<IEnumerable<Plant>> GetAvailablePlantsAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Plant>> GetByLevelAsync(PlantLevel level, CancellationToken cancellationToken = default);
        Task<IEnumerable<Plant>> GetSeasonalPlantsAsync(CancellationToken cancellationToken = default);
        Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default);
    }
}
