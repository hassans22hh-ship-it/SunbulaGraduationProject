using PlantApplication.StorePlantDTOs;
using PlantDomain.Enums;


namespace PlantApplication.StorePlantServiceAbstraction
{
    public interface IPlantService
    {
        Task<PlantDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<PlantDto>> GetAllAvailableAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<PlantDto>> GetByLevelAsync(PlantLevel level, CancellationToken cancellationToken = default);
        Task<IEnumerable<PlantDto>> GetSeasonalPlantsAsync(CancellationToken cancellationToken = default);
        Task<PlantDto> CreateAsync(CreatePlantDto dto, CancellationToken cancellationToken = default);
        Task<PlantDto> UpdateAsync(Guid id, UpdatePlantDto dto, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PlantDto> SetAvailabilityAsync(Guid id, bool isAvailable, CancellationToken cancellationToken = default);
    }
}
