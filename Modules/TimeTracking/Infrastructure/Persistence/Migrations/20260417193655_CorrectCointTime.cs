using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTrackingInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CorrectCointTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CoinsEarned",
                schema: "tracking",
                table: "TimeSessions",
                type: "int",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "TotalCoins",
                schema: "tracking",
                table: "DailyTransactions",
                type: "int",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldDefaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "CoinsEarned",
                schema: "tracking",
                table: "TimeSessions",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 10,
                oldScale: 2,
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalCoins",
                schema: "tracking",
                table: "DailyTransactions",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "int",
                oldPrecision: 10,
                oldScale: 2,
                oldDefaultValue: 0);
        }
    }
}
