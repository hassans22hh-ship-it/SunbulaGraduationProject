using MediatR;
using SharedKernel;
using FinanceApplication.FinanceServiceAbs;

namespace FinanceInfrastructure.IntegrationEvents
{
    public class UserAccountDeletedHandler : INotificationHandler<UserAccountDeletedEvent>
    {
        private readonly IFinancialTransactionService _financialService;

        public UserAccountDeletedHandler(IFinancialTransactionService financialService)
        {
            _financialService = financialService;
        }

        public async Task Handle(UserAccountDeletedEvent notification, CancellationToken cancellationToken)
        {
            await _financialService.DeleteUserDataAsync(notification.UserId, cancellationToken);
        }
    }
}
