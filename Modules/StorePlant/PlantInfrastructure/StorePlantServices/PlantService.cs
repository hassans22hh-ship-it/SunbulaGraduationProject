using AutoMapper;
using PlantApplication.StorePlantDTOs;
using PlantApplication.StorePlantServiceAbstraction;
using PlantDomain.Contracts;
using PlantDomain.Entities;
using PlantDomain.Enums;
using PlantDomain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlantInfrastructure.StorePlantServices
{
    public class PlantService : IPlantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlantService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PlantDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var plant = await _unitOfWork.Plants.GetByIdAsync(id, cancellationToken)
                ?? throw new PlantNotFoundException(id);

            return _mapper.Map<PlantDto>(plant);
        }

        public async Task<IEnumerable<PlantDto>> GetAllAvailableAsync(CancellationToken cancellationToken = default)
        {
            var plants = await _unitOfWork.Plants.GetAvailablePlantsAsync(cancellationToken);
            return _mapper.Map<IEnumerable<PlantDto>>(plants);
        }

        public async Task<IEnumerable<PlantDto>> GetByLevelAsync(PlantLevel level, CancellationToken cancellationToken = default)
        {
            var plants = await _unitOfWork.Plants.GetByLevelAsync(level, cancellationToken);
            return _mapper.Map<IEnumerable<PlantDto>>(plants);
        }

        public async Task<IEnumerable<PlantDto>> GetSeasonalPlantsAsync(CancellationToken cancellationToken = default)
        {
            var plants = await _unitOfWork.Plants.GetSeasonalPlantsAsync(cancellationToken);
            return _mapper.Map<IEnumerable<PlantDto>>(plants);
        }

        public async Task<PlantDto> CreateAsync(CreatePlantDto dto, CancellationToken cancellationToken = default)
        {
            var nameExists = await _unitOfWork.Plants.NameExistsAsync(dto.Name, cancellationToken);
            if (nameExists)
                throw new InvalidOperationException($"A plant with the name '{dto.Name}' already exists.");

            var plant = Plant.Create(dto.Name, dto.BotanicName, dto.Description,
                dto.ImageUrl, dto.Price, dto.Level, dto.Decoration);

            await _unitOfWork.Plants.AddAsync(plant, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<PlantDto>(plant);
        }

        public async Task<PlantDto> UpdateAsync(Guid id, UpdatePlantDto dto, CancellationToken cancellationToken = default)
        {
            var plant = await _unitOfWork.Plants.GetByIdAsync(id, cancellationToken)
                ?? throw new PlantNotFoundException(id);

            plant.Update(dto.Name, dto.BotanicName, dto.Description, dto.ImageUrl, dto.Price, dto.Decoration);

            _unitOfWork.Plants.Update(plant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<PlantDto>(plant);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var plant = await _unitOfWork.Plants.GetByIdAsync(id, cancellationToken)
                ?? throw new PlantNotFoundException(id);

            _unitOfWork.Plants.Delete(plant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<PlantDto> SetAvailabilityAsync(Guid id, bool isAvailable, CancellationToken cancellationToken = default)
        {
            var plant = await _unitOfWork.Plants.GetByIdAsync(id, cancellationToken)
                ?? throw new PlantNotFoundException(id);

            plant.SetAvailability(isAvailable);

            _unitOfWork.Plants.Update(plant);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return _mapper.Map<PlantDto>(plant);
        }
    }
}
