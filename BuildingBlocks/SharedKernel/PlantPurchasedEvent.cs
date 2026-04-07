using MediatR;
using SharedKernel;
using System;

namespace SharedKernel
{
    /// <summary>
    /// Integration event published when a user successfully purchases a plant.
    /// Consumed by UserIdentity module to deduct coins from balance.
    /// </summary>
    public record PlantPurchasedEvent(
        Guid UserPlantId,
        Guid UserId,
        Guid PlantId,
        int CoinsSpent) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
