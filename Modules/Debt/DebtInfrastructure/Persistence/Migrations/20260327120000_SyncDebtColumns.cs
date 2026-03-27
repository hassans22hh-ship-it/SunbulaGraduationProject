using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DebtInfrastructure.Persistence.Migrations
{
    public partial class SyncDebtColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[debt].[Debts]') AND name = 'Amount')
                BEGIN
                    ALTER TABLE [debt].[Debts] ADD [Amount] decimal(18,2) NOT NULL DEFAULT 0.0;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[debt].[Debts]') AND name = 'RemainingAmount')
                BEGIN
                    ALTER TABLE [debt].[Debts] ADD [RemainingAmount] decimal(18,2) NOT NULL DEFAULT 0.0;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('[debt].[DebtPayments]') AND name = 'Amount')
                BEGIN
                    ALTER TABLE [debt].[DebtPayments] ADD [Amount] decimal(18,2) NOT NULL DEFAULT 0.0;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                schema: "debt",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "RemainingAmount",
                schema: "debt",
                table: "Debts");

            migrationBuilder.DropColumn(
                name: "Amount",
                schema: "debt",
                table: "DebtPayments");
        }
    }
}
