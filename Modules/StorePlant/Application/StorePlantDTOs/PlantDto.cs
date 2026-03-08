using PlantDomain.Enums;

namespace PlantApplication.StorePlantDTOs
{
    public sealed record PlantDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string BotanicName { get; init; }
        public required string Description { get; init; }
        public required string ImageUrl { get; init; }
        public required int Price { get; init; }
        public required PlantLevel Level { get; init; }
        public required string LevelLabel { get; init; }
        public string? Decoration { get; init; }
        public required bool IsAvailable { get; init; }
        public required bool IsSeasonal { get; init; }
        public DateTime? SeasonStart { get; init; }
        public DateTime? SeasonEnd { get; init; }
        public required DateTime CreatedAt { get; init; }
    }
}
