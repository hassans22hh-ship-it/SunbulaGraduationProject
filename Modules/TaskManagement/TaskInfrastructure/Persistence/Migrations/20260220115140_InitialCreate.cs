using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "TaskManagement");

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Emoji = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Color = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    BehaviorType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FolderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsArchived = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Folders_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "TaskManagement",
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "TaskCategories",
                schema: "TaskManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "TaskManagement",
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskCategories_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "TaskManagement",
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Folders_UserId",
                schema: "TaskManagement",
                table: "Folders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_UserId_Name",
                schema: "TaskManagement",
                table: "Folders",
                columns: new[] { "UserId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskCategories_CategoryId",
                schema: "TaskManagement",
                table: "TaskCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCategories_TaskId",
                schema: "TaskManagement",
                table: "TaskCategories",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCategories_TaskId_CategoryId",
                schema: "TaskManagement",
                table: "TaskCategories",
                columns: new[] { "TaskId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_BehaviorType",
                schema: "TaskManagement",
                table: "Tasks",
                column: "BehaviorType");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedAt",
                schema: "TaskManagement",
                table: "Tasks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_FolderId",
                schema: "TaskManagement",
                table: "Tasks",
                column: "FolderId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_IsArchived",
                schema: "TaskManagement",
                table: "Tasks",
                column: "IsArchived");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_Status",
                schema: "TaskManagement",
                table: "Tasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId",
                schema: "TaskManagement",
                table: "Tasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_UserId_Title",
                schema: "TaskManagement",
                table: "Tasks",
                columns: new[] { "UserId", "Title" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskCategories",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "Folders",
                schema: "TaskManagement");
        }
    }
}
