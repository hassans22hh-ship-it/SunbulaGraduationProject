using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Sunbula; // Ensure this matches the namespace in Sunbula/Program.cs

namespace Sunbula.IntegrationTests
{
    public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthenticationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Login_WithTestUser_ShouldReturnOk()
        {
            // Arrange
            var loginDto = new
            {
                Email = "test_antigravity@test.com",
                Password = "any"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/Authentication/login", loginDto);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            
            var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            result.TryGetProperty("accessToken", out var token).Should().BeTrue();
            token.GetString().Should().NotBeNullOrEmpty();
        }
    }
}
