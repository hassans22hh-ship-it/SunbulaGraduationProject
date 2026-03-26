using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DebtInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixBugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Debts",
                newName: "Debts",
                newSchema: "debt");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                schema: "debt",
                table: "Debts",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPaid",
                schema: "debt",
                table: "Debts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                schema: "debt",
                table: "Debts",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "DebtType",
                schema: "debt",
                table: "Debts",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CreditorName",
                schema: "debt",
                table: "Debts",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

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
            migrationBuilder.DropIndex(
                name: "IX_Debts_DueDate",
                schema: "debt",
                table: "Debts");

            migrationBuilder.DropIndex(
                name: "IX_Debts_IsPaid",
                schema: "debt",
                table: "Debts");

            migrationBuilder.DropIndex(
                name: "IX_Debts_UserId",
                schema: "debt",
                table: "Debts");

            migrationBuilder.DropIndex(
                name: "IX_Debts_UserId_DebtType",
                schema: "debt",
                table: "Debts");

            migrationBuilder.DropIndex(
                name: "IX_Debts_UserId_IsPaid_DueDate",
                schema: "debt",
                table: "Debts");

            migrationBuilder.RenameTable(
                name: "Debts",
                schema: "debt",
                newName: "Debts");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Debts",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPaid",
                table: "Debts",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Debts",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "DebtType",
                table: "Debts",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "CreditorName",
                table: "Debts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
