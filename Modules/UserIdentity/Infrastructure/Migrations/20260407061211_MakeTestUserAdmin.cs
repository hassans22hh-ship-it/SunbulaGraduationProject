using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserIdentityInfrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeTestUserAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [Identity].[Users] SET Role = 1 WHERE Email = 'test_Sunbula@test.com'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [Identity].[Users] SET Role = 0 WHERE Email = 'test_Sunbula@test.com'");
        }
    }
}
