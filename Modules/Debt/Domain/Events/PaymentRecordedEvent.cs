using SharedKernel;

namespace DebtDomain.Events
{
    public sealed record  PaymentRecordedEvent(
    Guid DebtId,
    Guid PaymentId,
    decimal PaymentAmount,
    decimal RemainingAmount) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
