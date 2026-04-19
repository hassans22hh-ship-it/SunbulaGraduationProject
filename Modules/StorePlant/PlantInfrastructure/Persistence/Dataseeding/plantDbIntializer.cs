using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PlantDomain.Entities;
using PlantDomain.Enums;
using PlantInfrastructure.Persistence.Data;

namespace PlantInfrastructure.Persistence.Dataseeding
{
    public static class plantDbIntializer
    {
        public static async Task SeedPlantsAsync(StorePlantDbContext context, ILogger logger)
        {
            try
            {
                // 1. Resolve path to plant.json
                var baseDir = AppContext.BaseDirectory;
                var filePath = Path.Combine(baseDir, "Persistence", "Dataseeding", "plant.json");

                // Fallback for local development if running from project root
                if (!File.Exists(filePath))
                {
                    // Try to find it in the source tree if not in output (common during initial dev runs)
                    var currentDir = Directory.GetCurrentDirectory();
                    filePath = Path.Combine(currentDir, "Modules", "StorePlant", "PlantInfrastructure", "Persistence", "Dataseeding", "plant.json");
                }

                if (!File.Exists(filePath))
                {
                    logger.LogWarning("Plant seeding file not found. Path: {FilePath}", filePath);
                    return;
                }

                // 2. Read and deserialize
                var jsonData = await File.ReadAllTextAsync(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var plantDtos = JsonSerializer.Deserialize<List<PlantSeedDto>>(jsonData, options);

                if (plantDtos == null || !plantDtos.Any())
                {
                    logger.LogInformation("No plants found in plant.json to seed.");
                    return;
                }

                // 3. Incremental Seeding: Check existing names
                // We use IgnoreQueryFilters to see even "deleted" plants if we want to avoid name collisions
                var existingNames = await context.Plants
                    .IgnoreQueryFilters() 
                    .Select(p => p.Name)
                    .ToListAsync();

                var newPlants = new List<Plant>();
                foreach (var dto in plantDtos)
                {
                    if (!existingNames.Contains(dto.Name))
                    {
                        try 
                        {
                            var imageUrl = dto.ImageUrl;

                            var plant = Plant.Create(
                                dto.Name,
                                dto.BotanicName,
                                dto.Description,
                                imageUrl,
                                dto.Price,
                                (PlantLevel)dto.Level,
                                dto.Decoration
                            );
                            newPlants.Add(plant);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to create plant entity for {PlantName}", dto.Name);
                        }
                    }
                }

                if (newPlants.Any())
                {
                    await context.Plants.AddRangeAsync(newPlants);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Plant Seeding: Successfully added {Count} new plants. {SkippedCount} already existed.", newPlants.Count, plantDtos.Count - newPlants.Count);
                }
                else
                {
                    logger.LogInformation("Plant Seeding: All {Count} plants from JSON already exist in the database.", plantDtos.Count);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Critical error during plant seeding process.");
                throw;
            }
        }

        private class PlantSeedDto
        {
            public string Name { get; set; } = string.Empty;
            public string BotanicName { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string ImageUrl { get; set; } = string.Empty;
            public int Price { get; set; }
            public int Level { get; set; }
            public string? Decoration { get; set; }
        }
    }
}
