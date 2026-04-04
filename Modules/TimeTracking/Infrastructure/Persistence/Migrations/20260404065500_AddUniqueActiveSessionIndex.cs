using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTrackingInfrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueActiveSessionIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // BR-02 Safety Net: Filtered unique index prevents duplicate active sessions
            // for the same (UserId, TaskId) combination at the database level.
            // This is the last line of defense against race conditions.
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM sys.indexes
                    WHERE name = 'UIX_TimeSessions_UserId_TaskId_Active'
                      AND object_id = OBJECT_ID('tracking.TimeSessions')
                )
                BEGIN
                    CREATE UNIQUE INDEX UIX_TimeSessions_UserId_TaskId_Active
                    ON tracking.TimeSessions (UserId, TaskId)
                    WHERE IsActive = 1 AND IsDeleted = 0;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DROP INDEX IF EXISTS UIX_TimeSessions_UserId_TaskId_Active
                ON tracking.TimeSessions;
            ");
        }
    }
}
