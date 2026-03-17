using SharedKernel;

namespace Domain.Entities.ValueOpjects
{
    public sealed record UserRegisteredEvent(Guid UserId, string Email) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();

        public DateTime OccurredOn { get; }=DateTime.UtcNow;
    }
}
