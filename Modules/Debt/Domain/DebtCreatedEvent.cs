using DebtDomain.Enums;
using SharedKernel;


namespace DebtDomain
{
    public sealed record DebtCreatedEvent
    (Guid DebtId,
    Guid UserId,
    decimal Amount,
    DebtType DebtType) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
