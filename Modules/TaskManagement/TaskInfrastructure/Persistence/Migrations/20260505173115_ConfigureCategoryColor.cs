using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureCategoryColor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                schema: "task",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                schema: "task",
                table: "Categories");
        }
    }
}
