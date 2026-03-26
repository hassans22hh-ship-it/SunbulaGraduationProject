using Application.Services.Abstraction;
using Domain.Contracts;
using Domain.Exceptions;

namespace UserIdentityInfrastructure.Services
{
    public class UserIntegrationService : IUserIntegrationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserIntegrationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AwardStreakMilestoneAsync(Guid userId, int milestoneDays, int coins, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
                ?? throw new UserNotFoundException(userId);

            user.AwardStreakMilestone(milestoneDays, coins);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task SpendCoinsAsync(Guid userId, int amount, string reason, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
                ?? throw new UserNotFoundException(userId);

            user.SpendCoins(amount, reason);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
