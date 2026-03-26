using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;

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
    }
}
