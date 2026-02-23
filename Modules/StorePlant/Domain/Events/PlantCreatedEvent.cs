using SharedKernel;

namespace PlantDomain.Events
{
    public sealed  record PlantCreatedEvent(Guid PlantId) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
