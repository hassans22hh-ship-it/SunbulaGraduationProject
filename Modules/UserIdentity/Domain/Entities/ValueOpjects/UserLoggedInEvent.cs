using SharedKernel;

namespace Domain.Entities.ValueOpjects
{
    public sealed record UserLoggedInEvent(Guid UserId, string Email, DateTime LoginAt) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
