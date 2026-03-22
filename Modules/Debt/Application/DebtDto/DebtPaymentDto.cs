namespace DebtApplication.Dtos
{
    /// Response DTO for DebtPayment.

    public sealed record DebtPaymentDto
    {
        public required Guid Id { get; init; }
        public required Guid DebtId { get; init; }
        public required decimal Amount { get; init; }
        public required DateTime PaymentDate { get; init; }
        public string? Notes { get; init; }
        public required DateTime CreatedAt { get; init; }
    }
}

