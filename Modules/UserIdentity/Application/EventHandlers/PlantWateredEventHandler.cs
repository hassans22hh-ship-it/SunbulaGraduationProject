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
            // Note: The coins are now atomically deducted in the Plant module to prevent race conditions.
            // We no longer call SpendCoinsAsync here to prevent double deduction.
            // If gamification events are needed, we can publish a notification here instead.
            await Task.CompletedTask;
        }
    }
}
