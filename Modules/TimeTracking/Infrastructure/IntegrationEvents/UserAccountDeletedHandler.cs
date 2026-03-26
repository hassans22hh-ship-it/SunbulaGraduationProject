using MediatR;
using SharedKernel;
using TimeTrackingApplication.TimeServiceAbstraction;

namespace TimeTrackingInfrastructure.IntegrationEvents
{
    public class UserAccountDeletedHandler : INotificationHandler<UserAccountDeletedEvent>
    {
        private readonly ITimeSessionService _timeTrackingService;

        public UserAccountDeletedHandler(ITimeSessionService timeTrackingService)
        {
            _timeTrackingService = timeTrackingService;
        }

        public async Task Handle(UserAccountDeletedEvent notification, CancellationToken cancellationToken)
        {
            await _timeTrackingService.DeleteUserDataAsync(notification.UserId, cancellationToken);
        }
    }
}
