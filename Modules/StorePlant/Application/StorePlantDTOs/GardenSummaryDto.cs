using System;
using System.Collections.Generic;
using System.Text;

namespace PlantApplication.StorePlantDTOs
{
    public sealed record GardenSummaryDto
    {
        public required int TotalPlants { get; init; }
        public required int TotalCoinsInvested { get; init; }
        public UserPlantDto? FirstPurchased { get; init; }
        public UserPlantDto? MostExpensive { get; init; }
        public required IEnumerable<UserPlantDto> Plants { get; init; }
    }
}
