using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Sunbula.IntegrationTests
{
    public class AuthenticationTests : IntegrationTestBase
    {
        public AuthenticationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task Login_WithRegisteredUser_ShouldReturnOkAndToken()
        {
            // Arrange
            var email = $"test_login_{Guid.NewGuid()}@test.com";
            var password = "Password123!";

            var registerDto = new
            {
                Email = email,
                FirstName = "Integration",
                LastName = "Tester",
                Password = password,
                ConfirmPassword = password
            };

            var regResponse = await Client.PostAsJsonAsync("/api/v1/Authentication/register", registerDto);
            regResponse.EnsureSuccessStatusCode();

            var loginDto = new
            {
                Email = email,
                Password = password
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/Authentication/login", loginDto);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            result.TryGetProperty("accessToken", out var token).Should().BeTrue();
            token.GetString().Should().NotBeNullOrEmpty();
        }
    }
}
