using SharedKernel.Contracts;
using Domain.Contracts;
using Application.Services.Abstraction;
using Domain.Entities.ValueObjects;

namespace UserIdentityInfrastructure.Services
{
    public class UserCoinService : IUserCoinService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICoinStreamManager _coinStreamManager;

        public UserCoinService(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            ICoinStreamManager coinStreamManager)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _coinStreamManager = coinStreamManager;
        }

        public async Task<bool> TryDeductCoinsAsync(Guid userId, int amount, string reason, CancellationToken cancellationToken = default)
        {
            if (amount <= 0) return true;

            var success = await _userRepository.TryDeductCoinsAtomicAsync(userId, amount, cancellationToken);
            if (success)
            {
                var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
                if (user != null)
                {
                    await _coinStreamManager.NotifyCoinChangeAsync(
                        new CoinBalanceChangedEvent(userId, user.CoinBalance + amount, user.CoinBalance, -amount, reason));
                }
            }
            return success;
        }

        public async Task RefundCoinsAsync(Guid userId, int amount, string reason, CancellationToken cancellationToken = default)
        {
            if (amount <= 0) return;

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                user.AddCoins(amount, reason);
                _userRepository.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<int> GetUserCoinBalanceAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            return user?.CoinBalance ?? 0;
        }
    }
}
