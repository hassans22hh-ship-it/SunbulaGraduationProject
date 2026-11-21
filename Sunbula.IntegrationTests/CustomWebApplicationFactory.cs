using Application.Services.Abstraction;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using Xunit;

namespace Sunbula.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:SunbulaDb"] = _dbContainer.GetConnectionString(),
                    ["Database:AutoMigrate"] = "true",
                    ["Jwt:SecretKey"] = "SunbulaDummyIntegrationTestSecretKey_NotReal_AtLeast256BitsLongForHmacSha256Security!",
                    ["Jwt:Issuer"] = "https://localhost:7217;http://localhost:5142",
                    ["Jwt:Audience"] = "SunbulaIntegrationTests",
                    ["Jwt:ExpiryMinutes"] = "60",
                    ["Jwt:RefreshTokenExpiryDays"] = "7"
                });
            });

            builder.ConfigureTestServices(services =>
            {
                // Replace external SMTP email service with a mock/no-op service for integration tests
                services.RemoveAll<IEmailService>();
                services.AddScoped<IEmailService, TestNoOpEmailService>();
            });
        }
    }

    public class TestNoOpEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    [CollectionDefinition("IntegrationTests")]
    public class IntegrationTestCollection : ICollectionFixture<CustomWebApplicationFactory>
    {
    }
}
