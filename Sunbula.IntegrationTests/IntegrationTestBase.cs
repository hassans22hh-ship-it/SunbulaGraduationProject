using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;

namespace Sunbula.IntegrationTests
{
    public abstract class IntegrationTestBase : IClassFixture<WebApplicationFactory<Program>>
    {
        protected readonly WebApplicationFactory<Program> Factory;
        protected readonly HttpClient Client;

        protected IntegrationTestBase(WebApplicationFactory<Program> factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
        }

        protected string? CurrentToken { get; private set; }

        protected async Task AuthenticateAsync()
        {
            var userEmail = $"test_{Guid.NewGuid()}@test.com";
            var loginDto = new
            {
                Email = userEmail,
                Password = "Password123!"
            };

            // 1. Register
            var registerDto = new
            {
                Email = userEmail,
                FirstName = "Test",
                LastName = "User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };
            var regResponse = await Client.PostAsJsonAsync("/api/v1/Authentication/register", registerDto);
            // Ignore if already exists (depends on how 409 is handled)
            
            // 2. Login
            var response = await Client.PostAsJsonAsync("/api/v1/Authentication/login", loginDto);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Auth failed: {error}");
            }

            var result = await response.Content.ReadFromJsonAsync<JsonElement>();
            CurrentToken = result.GetProperty("accessToken").GetString();

            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CurrentToken);
        }

        protected Guid GetUserIdFromToken()
        {
            if (string.IsNullOrEmpty(CurrentToken))
                throw new InvalidOperationException("User is not authenticated. Call AuthenticateAsync() first.");

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(CurrentToken);
            var userIdClaim = jwtToken.Claims.First(claim => claim.Type == "sub" || claim.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdClaim.Value);
        }
    }
}
