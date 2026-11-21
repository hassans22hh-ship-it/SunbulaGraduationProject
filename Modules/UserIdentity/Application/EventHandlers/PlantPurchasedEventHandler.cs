using MediatR;
using SharedKernel;
using System.Threading;
using System.Threading.Tasks;

namespace Application.EventHandlers
{
    /// <summary>
    /// Integration event handler for PlantPurchasedEvent.
    /// In this Modular Monolith architecture, coin deduction is coordinated synchronously via IUserCoinService
    /// with compensating rollback on failure to maintain consistency across DbContexts without distributed transactions.
    /// This event handler serves as an extension point for audit logging, achievements, or analytics.
    /// </summary>
    public class PlantPurchasedEventHandler : INotificationHandler<PlantPurchasedEvent>
    {
        public Task Handle(PlantPurchasedEvent notification, CancellationToken cancellationToken)
        {
            // Extension point for cross-module gamification metrics / audit logging.
            return Task.CompletedTask;
        }
    }
}
