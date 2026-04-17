using Application.Services.Abstraction;
using MediatR;
using SharedKernel;

namespace Application.EventHandlers
{
    /// <summary>
    /// Handles the PlantWateredEvent by deducting coins from the user's balance.
    /// This ensures that watering a plant actually costs the user coins in the database.
    /// </summary>
    public class PlantWateredEventHandler : INotificationHandler<PlantWateredEvent>
    {
        private readonly IUserIntegrationService _userIntegrationService;

        public PlantWateredEventHandler(IUserIntegrationService userIntegrationService)
        {
            _userIntegrationService = userIntegrationService;
        }

        public async Task Handle(PlantWateredEvent notification, CancellationToken cancellationToken)
        {
            // Business Rule: Deduct coins invested in plant growth from the user's identity profile.
            await _userIntegrationService.SpendCoinsAsync(
                notification.UserId, 
                notification.CoinsSpent, 
                "Plant Watering/Growth", 
                cancellationToken);
        }
    }
}
