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
            // Note: The coins are now atomically deducted in the Plant module to prevent race conditions.
            // We no longer call SpendCoinsAsync here to prevent double deduction.
            // If gamification events are needed, we can publish a notification here instead.
            await Task.CompletedTask;
        }
    }
}
