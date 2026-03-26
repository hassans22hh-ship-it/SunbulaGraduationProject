using MediatR;
using SharedKernel;
using PlantApplication.StorePlantServiceAbstraction;

namespace PlantInfrastructure.IntegrationEvents
{
    public class UserAccountDeletedHandler : INotificationHandler<UserAccountDeletedEvent>
    {
        private readonly IUserPlantService _userPlantService;

        public UserAccountDeletedHandler(IUserPlantService userPlantService)
        {
            _userPlantService = userPlantService;
        }

        public async Task Handle(UserAccountDeletedEvent notification, CancellationToken cancellationToken)
        {
            await _userPlantService.DeleteUserDataAsync(notification.UserId, cancellationToken);
        }
    }
}
