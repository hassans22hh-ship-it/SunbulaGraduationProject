using Application.UserDTO;

namespace Application.Services.Abstraction
{
    public interface IUserIntegrationService
    {
        Task AwardStreakMilestoneAsync(Guid userId, int milestoneDays, int coins, CancellationToken cancellationToken = default);
        Task AddCoinsAsync(Guid userId, int amount, string reason, CancellationToken cancellationToken = default);
        Task SpendCoinsAsync(Guid userId, int amount, string reason, CancellationToken cancellationToken = default);
    }
}
