using AutoMapper;
using FinanceApplication.financedtos;
using FinanceApplication.FinanceServiceAbs;
using FinanceDomain.contracts;
using FinanceDomain.Entities;
using FinanceDomain.Exceptions;

namespace FinanceInfrastructure.financeSService
{
    public sealed class FinancialCategoryService : IFinancialCategoryService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public FinancialCategoryService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<FinancialCategoryDto> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
        {
            var category = await _uow.FinancialCategories.GetByIdAsync(id, ct)
                ?? throw new FinancialCategoryNotFoundException(id);

            EnsureOwnership(category.UserId, userId);
            return _mapper.Map<FinancialCategoryDto>(category);
        }

        public async Task<IEnumerable<FinancialCategoryDto>> GetAllAsync(Guid userId, CancellationToken ct = default)
        {
            var categories = await _uow.FinancialCategories.GetByUserIdAsync(userId, ct);
            return _mapper.Map<IEnumerable<FinancialCategoryDto>>(categories);
        }

        public async Task<FinancialCategoryDto> CreateAsync(
            CreateFinancialCategoryDto dto, Guid userId, CancellationToken ct = default)
        {
            var exists = await _uow.FinancialCategories.NameExistsAsync(userId, dto.Name, ct);
            if (exists)
                throw new InvalidOperationException($"A category named '{dto.Name}' already exists.");

            var category = FinancialCategory.Create(userId, dto.Name);
            await _uow.FinancialCategories.AddAsync(category, ct);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<FinancialCategoryDto>(category);
        }

        public async Task<FinancialCategoryDto> RenameAsync(
            Guid id, string newName, Guid userId, CancellationToken ct = default)
        {
            var category = await _uow.FinancialCategories.GetByIdAsync(id, ct)
                ?? throw new FinancialCategoryNotFoundException(id);

            EnsureOwnership(category.UserId, userId);

            var exists = await _uow.FinancialCategories.NameExistsAsync(userId, newName, ct);
            if (exists && category.Name != newName)
                throw new InvalidOperationException($"A category named '{newName}' already exists.");

            category.Rename(newName);
            _uow.FinancialCategories.Update(category);
            await _uow.SaveChangesAsync(ct);

            return _mapper.Map<FinancialCategoryDto>(category);
        }

        public async Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
        {
            var category = await _uow.FinancialCategories.GetByIdAsync(id, ct)
                ?? throw new FinancialCategoryNotFoundException(id);

            EnsureOwnership(category.UserId, userId);

            // Soft-delete: transactions will have FK set to NULL (OnDelete.SetNull)
            _uow.FinancialCategories.Delete(category);
            await _uow.SaveChangesAsync(ct);
        }

        private static void EnsureOwnership(Guid ownerId, Guid requesterId)
        {
            if (ownerId != requesterId)
                throw new UnauthorizedAccessException("You do not have permission to access this category.");
        }
    }
}
