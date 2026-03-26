using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;

namespace Sunbula.IntegrationTests
{
    public class TaskManagementTests : IntegrationTestBase
    {
        public TaskManagementTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateAndGetTask_ShouldSucceed()
        {
            // 1. Authenticate
            await AuthenticateAsync();

            // 2. Create Task
            var createTaskDto = new
            {
                Title = "Integration Test Task " + Guid.NewGuid().ToString().Substring(0, 8),
                Color = "#FF5733",
                BehaviorType = 0, // Assuming 0 is a valid enum value (e.g. Normal)
                Emoji = "🚀"
            };

            var createResponse = await Client.PostAsJsonAsync("/api/v1/Tasks", createTaskDto);
            createResponse.EnsureSuccessStatusCode();
            
            var createdTask = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
            var taskId = createdTask.GetProperty("id").GetGuid();
            taskId.Should().NotBeEmpty();

            // 3. Get All Tasks
            var getResponse = await Client.GetAsync("/api/v1/Tasks");
            getResponse.EnsureSuccessStatusCode();
            
            var result = await getResponse.Content.ReadFromJsonAsync<JsonElement>();
            var tasks = result.GetProperty("items").EnumerateArray();
            
            tasks.Should().Contain(t => t.GetProperty("id").GetGuid() == taskId);
        }
        
        [Fact]
        public async Task GetProfile_ShouldReturnCorrectEmail()
        {
            // 1. Authenticate
            await AuthenticateAsync();

            // 2. Get Profile
            var response = await Client.GetAsync("/api/v1/Authentication/profile");
            response.EnsureSuccessStatusCode();

            var user = await response.Content.ReadFromJsonAsync<JsonElement>();
            user.GetProperty("email").GetString().Should().Be("test_antigravity@test.com");
        }
    }
}
