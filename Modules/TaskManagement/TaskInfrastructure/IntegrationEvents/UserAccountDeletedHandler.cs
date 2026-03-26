using MediatR;
using SharedKernel;
using Application.ServiceAbstraction;

namespace TaskInfrastructure.IntegrationEvents
{
    public class UserAccountDeletedHandler : INotificationHandler<UserAccountDeletedEvent>
    {
        private readonly ITaskService _taskService;

        public UserAccountDeletedHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task Handle(UserAccountDeletedEvent notification, CancellationToken cancellationToken)
        {
            await _taskService.DeleteUserDataAsync(notification.UserId, cancellationToken);
        }
    }
}
