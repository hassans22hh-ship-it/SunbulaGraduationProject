namespace FinanceApplication.financedtos
{
    public sealed record FinancialCategoryDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public string? Icon { get; init; }
        public required DateTime CreatedAt { get; init; }
    }
}
