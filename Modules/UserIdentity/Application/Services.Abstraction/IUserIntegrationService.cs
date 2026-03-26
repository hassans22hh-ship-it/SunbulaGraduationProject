using Application.UserDTO;

namespace Application.Services.Abstraction
{
    public interface IUserIntegrationService
    {
        Task AwardStreakMilestoneAsync(Guid userId, int milestoneDays, int coins, CancellationToken cancellationToken = default);
    }
}
