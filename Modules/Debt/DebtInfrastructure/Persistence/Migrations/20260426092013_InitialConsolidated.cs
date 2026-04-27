using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DebtInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialConsolidated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "debt");

            migrationBuilder.CreateTable(
                name: "Debts",
                schema: "debt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditorName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DebtType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DebtPayments",
                schema: "debt",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DebtId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DebtPayments_Debts_DebtId",
                        column: x => x.DebtId,
                        principalSchema: "debt",
                        principalTable: "Debts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DebtPayments_DebtId",
                schema: "debt",
                table: "DebtPayments",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtPayments_PaymentDate",
                schema: "debt",
                table: "DebtPayments",
                column: "PaymentDate");

            migrationBuilder.CreateIndex(
                name: "IX_Debts_DueDate",
                schema: "debt",
                table: "Debts",
                column: "DueDate");

            migrationBuilder.CreateIndex(
                name: "IX_Debts_IsPaid",
                schema: "debt",
                table: "Debts",
                column: "IsPaid");

            migrationBuilder.CreateIndex(
                name: "IX_Debts_UserId",
                schema: "debt",
                table: "Debts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Debts_UserId_DebtType",
                schema: "debt",
                table: "Debts",
                columns: new[] { "UserId", "DebtType" });

            migrationBuilder.CreateIndex(
                name: "IX_Debts_UserId_IsPaid_DueDate",
                schema: "debt",
                table: "Debts",
                columns: new[] { "UserId", "IsPaid", "DueDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DebtPayments",
                schema: "debt");

            migrationBuilder.DropTable(
                name: "Debts",
                schema: "debt");
        }
    }
}
