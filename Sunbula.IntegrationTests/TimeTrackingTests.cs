using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;

namespace Sunbula.IntegrationTests
{
    public class TimeTrackingTests : IntegrationTestBase
    {
        public TimeTrackingTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task StartAndGetActiveSession_ShouldSucceed()
        {
            // 1. Authenticate
            await AuthenticateAsync();

            // Cleanup any existing active session (for reliability)
            await Client.PostAsync("/api/v1/TimeSession/stop-active", null);

            // 2. Create a Task first (to satisfy TaskId requirement)
            var createTaskDto = new
            {
                Title = "Test Task for Time Session " + Guid.NewGuid().ToString().Substring(0, 8),
                Description = "Integration Test Description",
                Color = "#FF5733"
            };
            var taskResponse = await Client.PostAsJsonAsync("/api/v1/Tasks", createTaskDto);
            taskResponse.EnsureSuccessStatusCode();
            var createdTask = await taskResponse.Content.ReadFromJsonAsync<JsonElement>();
            var taskId = createdTask.GetProperty("id").GetGuid();

            // 3. Start Session
            var startDto = new
            {
                Title = "Integration Test Session",
                Category = "Work",
                TaskId = taskId,
                StartTime = DateTime.UtcNow
            };

            var startResponse = await Client.PostAsJsonAsync("/api/v1/TimeSession/start", startDto);
            
            // If already has active session, it might return 409, so we skip if 409
            if (startResponse.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                 // Potentially stop active then start again if we want pure test
                 await Client.PostAsync("/api/v1/TimeSession/stop-active", null);
                 startResponse = await Client.PostAsJsonAsync("/api/v1/TimeSession/start", startDto);
            }
            
            if (!startResponse.IsSuccessStatusCode)
            {
                var errorBody = await startResponse.Content.ReadAsStringAsync();
                throw new Exception($"Start Session failed with {startResponse.StatusCode}: {errorBody}");
            }

            // 3. Get Active Session
            var getActiveResponse = await Client.GetAsync("/api/v1/TimeSession/active");
            getActiveResponse.EnsureSuccessStatusCode();
            
            var sessionBody = await getActiveResponse.Content.ReadAsStringAsync();
            var session = JsonSerializer.Deserialize<JsonElement>(sessionBody);
            
            // Try both casings if needed, or just log
            if (!session.TryGetProperty("id", out var idProp) && !session.TryGetProperty("Id", out idProp))
            {
                 throw new Exception($"Could not find 'id' or 'Id' in response: {sessionBody}");
            }
            
            idProp.GetGuid().Should().NotBeEmpty();
            
            // 4. Stop Active Session (Cleanup)
            var stopResponse = await Client.PostAsync("/api/v1/TimeSession/stop-active", null);
            stopResponse.EnsureSuccessStatusCode();
        }
    }
}
