using FinanceApplication.financedtos;

namespace FinanceApplication.FinanceServiceAbs
{
    public interface IWalletService
    {
        Task<WalletDto> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);
        Task<IEnumerable<WalletDto>> GetAllAsync(Guid userId, CancellationToken ct = default);
        Task<WalletDto> CreateAsync(CreateWalletDto dto, Guid userId, CancellationToken ct = default);
        Task<WalletDto> UpdateAsync(Guid id, UpdateWalletDto dto, Guid userId, CancellationToken ct = default);
        Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
        Task<FinanceSummaryDto> GetSummaryAsync(Guid userId, string currency, CancellationToken ct = default);
    }
}
