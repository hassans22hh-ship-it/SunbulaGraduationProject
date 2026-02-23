using SharedKernel;

namespace PlantDomain.Events
{
    /// Raised when a user successfully purchases a plant.
    /// Integration: consumed by UserIdentity module to deduct coins from balance.
    
    public sealed record PlantPurchasedEvent(
    Guid UserPlantId,
    Guid UserId,
    Guid PlantId,
    int CoinsSpent) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
