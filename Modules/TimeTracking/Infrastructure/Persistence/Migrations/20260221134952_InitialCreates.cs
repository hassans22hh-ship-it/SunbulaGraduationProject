using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTrackingInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tracking");

            migrationBuilder.CreateTable(
                name: "DailyTransactions",
                schema: "tracking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalCoins = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    SessionCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTransactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeSessions",
                schema: "tracking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CoinsEarned = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    BehaviorType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    ManuallyAdded = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyTransactions_UserId",
                schema: "tracking",
                table: "DailyTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTransactions_UserId_Date",
                schema: "tracking",
                table: "DailyTransactions",
                columns: new[] { "UserId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TimeSessions_StartTime",
                schema: "tracking",
                table: "TimeSessions",
                column: "StartTime");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSessions_TaskId",
                schema: "tracking",
                table: "TimeSessions",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSessions_UserId",
                schema: "tracking",
                table: "TimeSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeSessions_UserId_IsActive",
                schema: "tracking",
                table: "TimeSessions",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_TimeSessions_UserId_StartTime",
                schema: "tracking",
                table: "TimeSessions",
                columns: new[] { "UserId", "StartTime" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyTransactions",
                schema: "tracking");

            migrationBuilder.DropTable(
                name: "TimeSessions",
                schema: "tracking");
        }
    }
}
