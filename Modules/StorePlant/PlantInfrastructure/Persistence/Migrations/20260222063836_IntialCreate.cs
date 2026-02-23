using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PlantInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class IntialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "store");

            migrationBuilder.CreateTable(
                name: "Plants",
                schema: "store",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BotanicName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Price = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Decoration = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsSeasonal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SeasonStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SeasonEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserPlants",
                schema: "store",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoinsSpent = table.Column<int>(type: "int", nullable: false),
                    PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrentStage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StageCoinsAccumulated = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPlants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPlants_Plants_PlantId",
                        column: x => x.PlantId,
                        principalSchema: "store",
                        principalTable: "Plants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GrowthHistories",
                schema: "store",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserPlantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AchievementId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrowthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrowthHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrowthHistories_UserPlants_UserPlantId",
                        column: x => x.UserPlantId,
                        principalSchema: "store",
                        principalTable: "UserPlants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrowthHistories_UserPlantId",
                schema: "store",
                table: "GrowthHistories",
                column: "UserPlantId");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_IsAvailable",
                schema: "store",
                table: "Plants",
                column: "IsAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_Level",
                schema: "store",
                table: "Plants",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Plants_Name",
                schema: "store",
                table: "Plants",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPlants_PlantId",
                schema: "store",
                table: "UserPlants",
                column: "PlantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlants_PurchaseDate",
                schema: "store",
                table: "UserPlants",
                column: "PurchaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlants_UserId",
                schema: "store",
                table: "UserPlants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPlants_UserId_PlantId",
                schema: "store",
                table: "UserPlants",
                columns: new[] { "UserId", "PlantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrowthHistories",
                schema: "store");

            migrationBuilder.DropTable(
                name: "UserPlants",
                schema: "store");

            migrationBuilder.DropTable(
                name: "Plants",
                schema: "store");
        }
    }
}
