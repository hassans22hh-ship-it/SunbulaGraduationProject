using MediatR;
using SharedKernel;
using DebtApplication.DebtService;

namespace DebtInfrastructure.IntegrationEvents
{
    public class UserAccountDeletedHandler : INotificationHandler<UserAccountDeletedEvent>
    {
        private readonly IDebtService _debtService;

        public UserAccountDeletedHandler(IDebtService debtService)
        {
            _debtService = debtService;
        }

        public async Task Handle(UserAccountDeletedEvent notification, CancellationToken cancellationToken)
        {
            await _debtService.DeleteUserDataAsync(notification.UserId, cancellationToken);
        }
    }
}
