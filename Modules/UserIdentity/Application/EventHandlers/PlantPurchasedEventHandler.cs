using Application.Services.Abstraction;
using MediatR;
using SharedKernel;
using System.Threading;
using System.Threading.Tasks;

namespace Application.EventHandlers
{
    /// <summary>
    /// Handles the PlantPurchasedEvent by deducting coins from the user's balance.
    /// This is an integration handler that reacts to events in the StorePlant module.
    /// </summary>
    public class PlantPurchasedEventHandler : INotificationHandler<PlantPurchasedEvent>
    {
        private readonly IUserIntegrationService _userIntegrationService;

        public PlantPurchasedEventHandler(IUserIntegrationService userIntegrationService)
        {
            _userIntegrationService = userIntegrationService;
        }

        public async Task Handle(PlantPurchasedEvent notification, CancellationToken cancellationToken)
        {
            // Business Rule: Deduct coins spent on the plant from the user's identity profile.
            await _userIntegrationService.SpendCoinsAsync(
                notification.UserId, 
                notification.CoinsSpent, 
                "Plant Purchase", 
                cancellationToken);
        }
    }
}
