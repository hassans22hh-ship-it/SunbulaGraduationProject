using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AllowDuplicateTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserId_Title",
                schema: "TaskManagement",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId_Title",
                schema: "TaskManagement",
                table: "Tasks",
                columns: new[] { "UserId", "Title" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tasks_UserId_Title",
                schema: "TaskManagement",
                table: "Tasks");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId_Title",
                schema: "TaskManagement",
                table: "Tasks",
                columns: new[] { "UserId", "Title" },
                unique: true);
        }
    }
}
