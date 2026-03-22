using Application.ServiceAbstraction;
using Application.TaskManagmentDTOS;
using TaskDomain.Contracts;
using TaskDomain.Entities.TaskManagement;
using TaskDomain.Exceptions;

namespace TaskInfrastructure.Services
{
    public sealed class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
            if (category == null)
                throw new CategoryNotFoundException(id);

            return MapToDto(category);
        }

        public async Task<IEnumerable<CategoryDto>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var categories = await _unitOfWork.Categories.GetByUserIdAsync(userId, cancellationToken);
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            var nameExists = await _unitOfWork.Categories.NameExistsAsync(userId, dto.Name, cancellationToken: cancellationToken);
            if (nameExists)
                throw new InvalidOperationException($"Category with name '{dto.Name}' already exists");

            var category = Category.Create(userId, dto.Name, dto.Color);

            await _unitOfWork.Categories.AddAsync(category, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(category);
        }

        public async Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
            if (category == null)
                throw new CategoryNotFoundException(id);

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to update this category");

            if (category.Name != dto.Name)
            {
                var nameExists = await _unitOfWork.Categories.NameExistsAsync(userId, dto.Name, id, cancellationToken);
                if (nameExists)
                    throw new InvalidOperationException($"Category with name '{dto.Name}' already exists");
            }

            category.Update(dto.Name, dto.Color);

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(category);
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id, cancellationToken);
            if (category == null)
                throw new CategoryNotFoundException(id);

            if (category.UserId != userId)
                throw new UnauthorizedAccessException("You don't have permission to delete this category");

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color.Value,
                CreatedAt = category.CreatedAt,
                TaskCount = category.GetTaskCount()
            };
        }
    }
}
