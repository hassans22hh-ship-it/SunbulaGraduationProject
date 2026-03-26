using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;

namespace Sunbula.IntegrationTests
{
    public class FinanceTests : IntegrationTestBase
    {
        public FinanceTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateAndGetWallet_ShouldSucceed()
        {
            // 1. Authenticate
            await AuthenticateAsync();

            // 2. Create Wallet
            var createWalletDto = new
            {
                Name = "Wallet " + Guid.NewGuid().ToString().Substring(0, 8),
                Type = 0, // Assuming 0 is Cash
                Currency = "SAR",
                OpeningBalance = 1000m
            };

            var createResponse = await Client.PostAsJsonAsync("/api/v1/Wallets", createWalletDto);
            createResponse.EnsureSuccessStatusCode();
            
            var createdWallet = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
            var walletId = createdWallet.GetProperty("id").GetGuid();
            walletId.Should().NotBeEmpty();

            // 3. Get All Wallets
            var getResponse = await Client.GetAsync("/api/v1/Wallets");
            getResponse.EnsureSuccessStatusCode();
            
            var wallets = await getResponse.Content.ReadFromJsonAsync<IEnumerable<JsonElement>>();
            wallets.Should().Contain(w => w.GetProperty("id").GetGuid() == walletId);
        }
    }
}
