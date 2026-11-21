using AutoMapper;
using PlantApplication.StorePlantDTOs;
using PlantApplication.StorePlantServiceAbstraction;
using PlantDomain.Contracts;
using PlantDomain.Entities;
using PlantDomain.Exceptions;
using SharedKernel.Contracts;

namespace PlantInfrastructure.StorePlantServices
{
    public class UserPlantService : IUserPlantService
    {
        private readonly PlantDomain.Contracts.IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserCoinService _userCoinService;

        public UserPlantService(
            PlantDomain.Contracts.IUnitOfWork unitOfWork, 
            IMapper mapper, 
            IUserCoinService userCoinService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userCoinService = userCoinService;
        }

        public async Task<UserPlantDto> GetByIdAsync(Guid userPlantId, Guid userId, CancellationToken cancellationToken = default)
        {
            var userPlant = await _unitOfWork.UserPlants.GetByIdWithDetailsAsync(userPlantId, cancellationToken)
                ?? throw new UserPlantNotFoundException(userPlantId);

            if (userPlant.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this plant.");

            return _mapper.Map<UserPlantDto>(userPlant);
        }

        public async Task<GardenSummaryDto> GetGardenAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var userPlants = await _unitOfWork.UserPlants.GetGardenByUserIdAsync(userId, cancellationToken);
            var firstPurchased = await _unitOfWork.UserPlants.GetFirstPurchasedAsync(userId, cancellationToken);
            var mostExpensive = await _unitOfWork.UserPlants.GetMostExpensiveAsync(userId, cancellationToken);
            var totalCoinsSpent = await _unitOfWork.UserPlants.GetTotalCoinsSpentAsync(userId, cancellationToken);

            return new GardenSummaryDto
            {
                TotalPlants = userPlants.Count(),
                TotalCoinsInvested = totalCoinsSpent,
                FirstPurchased = firstPurchased != null ? _mapper.Map<UserPlantDto>(firstPurchased) : null,
                MostExpensive = mostExpensive != null ? _mapper.Map<UserPlantDto>(mostExpensive) : null,
                Plants = _mapper.Map<IEnumerable<UserPlantDto>>(userPlants)
            };
        }

        public async Task<UserPlantDto> PurchasePlantAsync(
            PurchasePlantDto dto,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            // 1. Fetch plant and verify it exists and is available
            var plant = await _unitOfWork.Plants.GetByIdAsync(dto.PlantId, cancellationToken)
                ?? throw new PlantNotFoundException(dto.PlantId);

            if (!plant.IsCurrentlyAvailable())
                throw new InvalidOperationException($"Plant '{plant.Name}' is not currently available.");

            // 2. Business rule: no duplicate ownership
            var alreadyOwned = await _unitOfWork.UserPlants.UserOwnsPlantAsync(userId, dto.PlantId, cancellationToken);
            if (alreadyOwned)
                throw new DuplicatePlantPurchaseException(userId, dto.PlantId);

            // 3. Business rule: sufficient coin balance (Atomically deducted via clean module contract)
            var deductionSuccessful = await _userCoinService.TryDeductCoinsAsync(
                userId, 
                plant.Price, 
                $"Purchase Plant: {plant.Name}", 
                cancellationToken);
            
            if (!deductionSuccessful)
            {
                var currentBalance = await _userCoinService.GetUserCoinBalanceAsync(userId, cancellationToken);
                throw new InsufficientCoinsException(plant.Price, currentBalance);
            }

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                // 4. Create domain entity and persist to StorePlantDbContext
                var userPlant = UserPlant.Create(userId, dto.PlantId, plant.Price);

                await _unitOfWork.UserPlants.AddAsync(userPlant, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                // Re-fetch with navigation properties loaded
                var saved = await _unitOfWork.UserPlants.GetByIdWithDetailsAsync(userPlant.Id, cancellationToken)!;
                return _mapper.Map<UserPlantDto>(saved!);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                // Compensating transaction: refund deducted coins on failure to maintain consistency
                await _userCoinService.RefundCoinsAsync(
                    userId, 
                    plant.Price, 
                    $"Rollback: Purchase failed for plant {plant.Name}", 
                    cancellationToken);

                throw;
            }
        }

        public async Task<UserPlantDto> AddGrowthCoinsAsync(
            Guid userPlantId,
            Guid userId,
            int coins,
            CancellationToken cancellationToken = default)
        {
            var userPlant = await _unitOfWork.UserPlants.GetByIdWithDetailsAsync(userPlantId, cancellationToken)
                ?? throw new UserPlantNotFoundException(userPlantId);

            if (userPlant.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this plant.");

            // 1. Business Rule: Atomically deduct coins via clean module contract
            var deductionSuccessful = await _userCoinService.TryDeductCoinsAsync(
                userId, 
                coins, 
                $"Water Plant: {userPlantId}", 
                cancellationToken);

            if (!deductionSuccessful)
            {
                var currentBalance = await _userCoinService.GetUserCoinBalanceAsync(userId, cancellationToken);
                throw new InsufficientCoinsException(coins, currentBalance);
            }

            try
            {
                // 2. Perform growth logic
                userPlant.AddGrowthCoins(coins);

                _unitOfWork.UserPlants.Update(userPlant);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return _mapper.Map<UserPlantDto>(userPlant);
            }
            catch
            {
                // Compensating transaction: refund growth coins if persistence fails
                await _userCoinService.RefundCoinsAsync(
                    userId, 
                    coins, 
                    $"Rollback: AddGrowthCoins failed for {userPlantId}", 
                    cancellationToken);

                throw;
            }
        }

        public async Task DeleteUserDataAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            await _unitOfWork.UserPlants.HardDeleteByUserIdAsync(userId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
