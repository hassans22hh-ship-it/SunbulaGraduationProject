using FinanceDomain.Enums;

namespace FinanceApplication.financedtos
{

    public sealed record WalletDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required WalletType Type { get; init; }
        public required decimal Balance { get; init; }
        public required string Currency { get; init; }
        public required DateTime CreatedAt { get; init; }
    }
}
