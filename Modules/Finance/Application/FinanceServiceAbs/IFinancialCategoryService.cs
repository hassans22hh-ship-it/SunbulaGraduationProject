using FinanceApplication.financedtos;

namespace FinanceApplication.FinanceServiceAbs
{
    public interface IFinancialCategoryService
    {

        Task<FinancialCategoryDto> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);
        Task<IEnumerable<FinancialCategoryDto>> GetAllAsync(Guid userId, CancellationToken ct = default);
        Task<FinancialCategoryDto> CreateAsync(CreateFinancialCategoryDto dto, Guid userId, CancellationToken ct = default);
        Task<FinancialCategoryDto> RenameAsync(Guid id, string newName, Guid userId, CancellationToken ct = default);
        Task<FinancialCategoryDto> UpdateIconAsync(Guid id, string? icon, Guid userId, CancellationToken ct = default);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
    }
}
