using PlantDomain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlantApplication.StorePlantDTOs
{
    public sealed record UserPlantDto
    {
        public required Guid Id { get; init; }
        public required Guid UserId { get; init; }
        public required Guid PlantId { get; init; }
        public required string PlantName { get; init; }
        public required string PlantImageUrl { get; init; }
        public required string PlantBotanicName { get; init; }
        public required int CoinsSpent { get; init; }
        public required DateTime PurchaseDate { get; init; }
        public required GrowthStage CurrentStage { get; init; }
        public required string CurrentStageLabel { get; init; }
        public required int StageCoinsAccumulated { get; init; }
        public required int CoinsToNextStage { get; init; }
        public IEnumerable<GrowthHistoryDto> GrowthHistories { get; init; } = [];
    }
}
