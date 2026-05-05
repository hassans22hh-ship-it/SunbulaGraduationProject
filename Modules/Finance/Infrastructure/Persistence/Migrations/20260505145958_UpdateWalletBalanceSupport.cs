using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWalletBalanceSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserId_Name",
                schema: "Finance",
                table: "Wallets");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId_Name",
                schema: "Finance",
                table: "Wallets",
                columns: new[] { "UserId", "Name" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_UserId_Name",
                schema: "Finance",
                table: "Wallets");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId_Name",
                schema: "Finance",
                table: "Wallets",
                columns: new[] { "UserId", "Name" },
                unique: true);
        }
    }
}
