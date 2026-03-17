using PlantDomain.Enums;
using SharedKernel;

namespace PlantDomain.Events
{
    /// Raised when a UserPlant advances to a new growth stage.

    public sealed record PlantGrownEvent(
    Guid UserPlantId,
    Guid UserId,
    GrowthStage NewStage) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

    }
}
