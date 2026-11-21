using MediatR;
using SharedKernel;
using System.Threading;
using System.Threading.Tasks;

namespace Application.EventHandlers
{
    /// <summary>
    /// Integration event handler for PlantWateredEvent.
    /// In this Modular Monolith architecture, coin deduction is coordinated synchronously via IUserCoinService
    /// with compensating rollback on failure to maintain consistency across DbContexts without distributed transactions.
    /// This event handler serves as an extension point for audit logging, achievements, or analytics.
    /// </summary>
    public class PlantWateredEventHandler : INotificationHandler<PlantWateredEvent>
    {
        public Task Handle(PlantWateredEvent notification, CancellationToken cancellationToken)
        {
            // Extension point for cross-module gamification metrics / audit logging.
            return Task.CompletedTask;
        }
    }
}
