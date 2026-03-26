using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;

namespace Sunbula.IntegrationTests
{
    public class DebtTests : IntegrationTestBase
    {
        public DebtTests(WebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task CreateAndGetDebt_ShouldSucceed()
        {
            // 1. Authenticate
            await AuthenticateAsync();

            // 2. Create Debt
            var createDebtDto = new
            {
                CreditorName = "Creditor " + Guid.NewGuid().ToString().Substring(0, 8),
                Amount = 500.50m,
                DebtType = 0, // Assuming 0 is Payable
                DueDate = DateTime.UtcNow.AddMonths(1),
                Notes = "Integration Test Notes"
            };

            var createResponse = await Client.PostAsJsonAsync("/api/v1/Debt", createDebtDto);
            if (!createResponse.IsSuccessStatusCode)
            {
                var error = await createResponse.Content.ReadAsStringAsync();
                throw new Exception($"Create Debt failed with {createResponse.StatusCode}: {error}");
            }
            
            var createdDebt = await createResponse.Content.ReadFromJsonAsync<JsonElement>();
            var debtId = createdDebt.GetProperty("id").GetGuid();
            debtId.Should().NotBeEmpty();

            // 3. Get All Debts
            var getResponse = await Client.GetAsync("/api/v1/Debt");
            getResponse.EnsureSuccessStatusCode();
            
            var debts = await getResponse.Content.ReadFromJsonAsync<IEnumerable<JsonElement>>();
            debts.Should().Contain(d => d.GetProperty("id").GetGuid() == debtId);
        }
    }
}
