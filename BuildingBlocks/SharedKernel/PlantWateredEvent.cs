using SharedKernel;
using System;

namespace SharedKernel
{
    /// <summary>
    /// Integration event raised when a user invests coins to grow a plant.
    /// This event is used to trigger coin deduction in the Identity/Finance modules.
    /// </summary>
    public sealed record PlantWateredEvent(
        Guid UserPlantId,
        Guid UserId,
        int CoinsSpent) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
