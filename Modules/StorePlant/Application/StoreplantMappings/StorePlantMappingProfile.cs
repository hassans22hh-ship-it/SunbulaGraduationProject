using AutoMapper;
using PlantApplication.StorePlantDTOs;
using PlantApplication.StorePlantServiceAbstraction;
using PlantDomain.Entities;
using PlantDomain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlantApplication.StoreplantMappings
{
    public sealed class StorePlantMappingProfile:Profile
    {
        public StorePlantMappingProfile()
        {
            CreatePlantMappings();
            CreateUserPlantMappings();
            CreateGrowthHistoryMappings();
        }

        // ── Plant ────────────────────────────────────────────────────────

        private void CreatePlantMappings()
        {
            CreateMap<Plant, PlantDto>()
                .ForMember(dest => dest.LevelLabel,
                    opt => opt.MapFrom(src => GetLevelLabel(src.Level)));
        }

        // ── UserPlant ────────────────────────────────────────────────────

        private void CreateUserPlantMappings()
        {
            CreateMap<UserPlant, UserPlantDto>()
                .ForMember(dest => dest.PlantName,
                    opt => opt.MapFrom(src => src.Plant.Name))
                .ForMember(dest => dest.PlantImageUrl,
                    opt => opt.MapFrom(src => src.Plant.ImageUrl))
                .ForMember(dest => dest.PlantBotanicName,
                    opt => opt.MapFrom(src => src.Plant.BotanicName))
                .ForMember(dest => dest.CurrentStageLabel,
                    opt => opt.MapFrom(src => GetStageLabel(src.CurrentStage)))
                .ForMember(dest => dest.CoinsToNextStage,
                    opt => opt.MapFrom(src => CalculateCoinsToNextStage(src)));
        }

        // ── GrowthHistory ────────────────────────────────────────────────

        private void CreateGrowthHistoryMappings()
        {
            CreateMap<GrowthHistory, GrowthHistoryDto>()
                .ForMember(dest => dest.StageLabel,
                    opt => opt.MapFrom(src => GetStageLabel(src.Stage)));
        }

        // ── Private Helpers ──────────────────────────────────────────────

        private static string GetLevelLabel(PlantLevel level) => level switch
        {
            PlantLevel.Beginner => "🌱 Beginner (20–50 coins)",
            PlantLevel.Medium => "🌿 Medium (50–300 coins)",
            PlantLevel.Advanced => "🌳 Advanced (1,000–2,000 coins)",
            PlantLevel.Rare => "🏆 Rare (5,000 coins)",
            _ => level.ToString()
        };

        private static string GetStageLabel(GrowthStage stage) => stage switch
        {
            GrowthStage.Seed => "🌰 Seed",
            GrowthStage.Seedling => "🌱 Seedling",
            GrowthStage.SmallPlant => "🌿 Small Plant",
            GrowthStage.LargePlant => "🌳 Large Plant",
            _ => stage.ToString()
        };

        private static int CalculateCoinsToNextStage(UserPlant userPlant)
        {
            if (userPlant.CurrentStage == GrowthStage.LargePlant)
                return 0; // Max stage reached

            const int CoinsPerStage = 10_000;
            return CoinsPerStage - userPlant.StageCoinsAccumulated;
        }
    }
}
