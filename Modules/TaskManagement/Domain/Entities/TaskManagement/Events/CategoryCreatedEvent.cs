using SharedKernel;

namespace TaskDomain.Entities.TaskManagement.Events
{
    public sealed class CategoryCreatedEvent(Guid CategoryId, Guid UserId) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public Guid CategoryId { get; } = CategoryId;
        public Guid UserId { get; } = UserId;
    }
}
