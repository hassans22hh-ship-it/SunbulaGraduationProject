using DebtDomain.Enums;
namespace DebtApplication.Dtos

{
    /// Response DTO for Debt.

    public sealed record DebtDto
    {
        public required Guid Id { get; init; }
        public required string CreditorName { get; init; }
        public required decimal Amount { get; init; }
        public required decimal RemainingAmount { get; init; }
        public required DateTime DueDate { get; init; }
        public required bool IsPaid { get; init; }
        public required bool IsOverdue { get; init; }
        public required DebtType DebtType { get; init; }
        public string? Notes { get; init; }
        public required DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }
}

