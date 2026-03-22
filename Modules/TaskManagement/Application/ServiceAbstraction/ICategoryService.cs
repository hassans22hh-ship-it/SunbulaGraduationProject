using Application.TaskManagmentDTOS;


namespace Application.ServiceAbstraction
{
    public interface ICategoryService
    {

        Task<CategoryDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<CategoryDto>> GetAllByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<CategoryDto> CreateAsync(CreateCategoryDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryDto dto, Guid userId, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
    }
}
