namespace DebtApplication.Dtos
{
    /// Debt with payment history.

    public sealed record DebtWithPaymentsDto
    {
        public required DebtDto Debt { get; init; }
        public required IEnumerable<DebtPaymentDto> Payments { get; init; }
    }
}

