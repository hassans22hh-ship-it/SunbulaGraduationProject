using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTrackingInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class assignintimespan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TotalPausedDurationTemp",
                schema: "tracking",
                table: "TimeSessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            // Convert SQL Server 'time' to .NET ticks (1 microsecond = 10 ticks)
            migrationBuilder.Sql(@"
                UPDATE [tracking].[TimeSessions] 
                SET [TotalPausedDurationTemp] = DATEDIFF_BIG(microsecond, '00:00:00', [TotalPausedDuration]) * 10
            ");

            migrationBuilder.DropColumn(
                name: "TotalPausedDuration",
                schema: "tracking",
                table: "TimeSessions");

            migrationBuilder.RenameColumn(
                name: "TotalPausedDurationTemp",
                schema: "tracking",
                table: "TimeSessions",
                newName: "TotalPausedDuration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "TotalPausedDuration",
                schema: "tracking",
                table: "TimeSessions",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 0L);
        }
    }
}
