using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTrackingInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class chageTypeOfDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "DurationMinutes",
                schema: "tracking",
                table: "TimeSessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DurationMinutes",
                schema: "tracking",
                table: "TimeSessions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 0L);
        }
    }
}
