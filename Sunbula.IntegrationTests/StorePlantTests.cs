using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Sunbula.IntegrationTests
{
    public class StorePlantTests : IntegrationTestBase
    {
        public StorePlantTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAllAvailablePlants_ShouldSucceed()
        {
            // 1. Authenticate
            await AuthenticateAsync();

            // 2. Get All Plants
            var response = await Client.GetAsync("/api/v1/store/plants");
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"GetAllPlants failed with {response.StatusCode}: {errorBody}");
            }
            
            var plants = await response.Content.ReadFromJsonAsync<IEnumerable<JsonElement>>();
            plants.Should().NotBeNull();
            // Even if empty, it's successful 200 OK
        }

        [Fact]
        public async Task GetPlantsByLevel_ShouldSucceed()
        {
            // 1. Authenticate
            await AuthenticateAsync();

            // 2. Get Beginner Plants
            var response = await Client.GetAsync("/api/v1/store/plants/level/1"); // Beginner (1)
            response.EnsureSuccessStatusCode();
            
            var plants = await response.Content.ReadFromJsonAsync<IEnumerable<JsonElement>>();
            plants.Should().NotBeNull();
        }

        [Fact]
        public async Task PurchasePlant_Flow_ShouldSucceed()
        {
            // 1. Authenticate and get UserId
            await AuthenticateAsync();
            var userId = GetUserIdFromToken();

            // 2. Seed Coins for the test user directly in DB
            using (var scope = Factory.Services.CreateScope())
            {
                var identityContext = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.Data.UserIdentityDbContext>();
                var user = await identityContext.Users.FindAsync(userId);
                if (user != null)
                {
                    // Using reflection or a private method if AddCoins is not accessible, 
                    // but AddCoins is public in User.cs
                    user.AddCoins(1000, "Integration Test Seeding");
                    await identityContext.SaveChangesAsync();
                }
            }

            // 3. Ensure a plant exists and get its ID
            Guid plantId;
            using (var scope = Factory.Services.CreateScope())
            {
                var storeContext = scope.ServiceProvider.GetRequiredService<PlantInfrastructure.Persistence.Data.StorePlantDbContext>();
                var plant = await storeContext.Plants.FirstOrDefaultAsync(p => p.IsAvailable && !p.IsDeleted);
                if (plant == null)
                {
                    // Create a dummy plant if none exists
                    plant = PlantDomain.Entities.Plant.Create("Test Plant", "Test Desc", 100, PlantDomain.Enums.PlantLevel.Beginner);
                    await storeContext.Plants.AddAsync(plant);
                    await storeContext.SaveChangesAsync();
                }
                plantId = plant.Id;
            }

            // 4. Purchase the plant
            var purchaseDto = new { PlantId = plantId };
            var response = await Client.PostAsJsonAsync("/api/v1/Garden/purchase", purchaseDto);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                // If it's 409 Conflict, it means user already owns it (which is fine for repeat tests)
                if (response.StatusCode != System.Net.HttpStatusCode.Conflict)
                {
                    throw new Exception($"Purchase failed with {response.StatusCode}: {errorBody}");
                }
            }
            else
            {
                var purchasedPlant = await response.Content.ReadFromJsonAsync<JsonElement>();
                purchasedPlant.GetProperty("plantId").GetGuid().Should().Be(plantId);
            }

            // 5. Verify Garden
            var gardenResponse = await Client.GetAsync("/api/v1/Garden");
            gardenResponse.EnsureSuccessStatusCode();
            
            var garden = await gardenResponse.Content.ReadFromJsonAsync<JsonElement>();
            var plantsInGarden = garden.GetProperty("plants").EnumerateArray();
            plantsInGarden.Any(p => p.GetProperty("plantId").GetGuid() == plantId).Should().BeTrue();
        }
    }
}
