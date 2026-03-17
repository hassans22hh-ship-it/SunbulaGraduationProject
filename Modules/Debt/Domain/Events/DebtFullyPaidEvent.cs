using SharedKernel;

namespace DebtDomain.Events
{
    public sealed record DebtFullyPaidEvent(
    Guid DebtId,
    Guid UserId) : IDomainEvent
      {
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;

    }
}
