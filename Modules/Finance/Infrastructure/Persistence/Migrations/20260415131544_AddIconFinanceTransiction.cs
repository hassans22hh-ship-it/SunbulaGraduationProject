using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIconFinanceTransiction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Icon",
                schema: "Finance",
                table: "FinancialCategories",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                schema: "Finance",
                table: "FinancialCategories");
        }
    }
}
