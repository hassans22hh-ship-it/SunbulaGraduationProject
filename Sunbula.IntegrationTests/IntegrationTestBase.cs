using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Sunbula.IntegrationTests
{
    [Collection("IntegrationTests")]
    public abstract class IntegrationTestBase
    {
        protected readonly CustomWebApplicationFactory Factory;
        protected readonly HttpClient Client;

        protected IntegrationTestBase(CustomWebApplicationFactory factory)
        {
            Factory = factory;
            Client = factory.CreateClient();
        }

        protected string? CurrentToken { get; private set; }
        protected string? CurrentUserEmail { get; private set; }

        protected async Task AuthenticateAsync()
        {
            CurrentUserEmail = $"test_{Guid.NewGuid()}@test.com";
            var password = "Password123!";
            var loginDto = new
            {
                Email = CurrentUserEmail,
                Password = password
            };

            // 1. Register
            var registerDto = new
            {
                Email = CurrentUserEmail,
                FirstName = "Test",
                LastName = "User",
                Password = password,
                ConfirmPassword = password
            };
            var regResponse = await Client.PostAsJsonAsync("/api/v1/Authentication/register", registerDto);
            regResponse.EnsureSuccessStatusCode();

            // 2. Confirm email directly in database so RequireEmailConfirmedMiddleware passes
            using (var scope = Factory.Services.CreateScope())
            {
                var identityContext = scope.ServiceProvider.GetRequiredService<Infrastructure.Persistence.Data.UserIdentityDbContext>();
                var user = await identityContext.Users.FirstOrDefaultAsync(u => EF.Property<string>(u, "_email") == CurrentUserEmail);
                if (user != null)
                {
                    user.ConfirmEmail();
                    await identityContext.SaveChangesAsync();
                }
            }

            // 3. Login with confirmed user
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
