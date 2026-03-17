using PlantDomain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlantApplication.StorePlantDTOs
{
    public sealed record GrowthHistoryDto
    {
        public required Guid Id { get; init; }
        public required Guid UserPlantId { get; init; }
        public required GrowthStage Stage { get; init; }
        public required string StageLabel { get; init; }
        public required Guid AchievementId { get; init; }
        public required DateTime GrowthDate { get; init; }
    }
}
