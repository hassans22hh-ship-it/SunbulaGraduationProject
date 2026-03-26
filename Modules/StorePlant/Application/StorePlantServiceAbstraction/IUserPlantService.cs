using PlantApplication.StorePlantDTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlantApplication.StorePlantServiceAbstraction
{
    public interface IUserPlantService
    {
        Task<UserPlantDto> GetByIdAsync(Guid userPlantId, Guid userId, CancellationToken cancellationToken = default);
        Task<GardenSummaryDto> GetGardenAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<UserPlantDto> PurchasePlantAsync(PurchasePlantDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task<UserPlantDto> AddGrowthCoinsAsync(Guid userPlantId, Guid userId, int coins, CancellationToken cancellationToken = default);
        Task DeleteUserDataAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
