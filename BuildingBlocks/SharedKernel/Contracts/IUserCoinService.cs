namespace SharedKernel.Contracts
{
    public interface IUserCoinService
    {
        Task<bool> TryDeductCoinsAsync(Guid userId, int amount, string reason, CancellationToken cancellationToken = default);
        Task RefundCoinsAsync(Guid userId, int amount, string reason, CancellationToken cancellationToken = default);
        Task<int> GetUserCoinBalanceAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
