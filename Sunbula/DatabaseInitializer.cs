using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DebtInfrastructure.Persistenece.Data;
using FinanceInfrastructure.Persistence.Data;
using PlantInfrastructure.Persistence.Data;
using TaskInfrastructure.Persistence.Data;
using TimeTrackingInfrastructure.Persistence.Data;
using Infrastructure.Persistence.Data; // UserIdentity

namespace Sunbula
{
    public static class DatabaseInitializer
    {
        public static async Task MigrateDatabasesAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();

            // List of DbContext types to migrate
            var dbContextTypes = new List<Type>
            {
                typeof(UserIdentityDbContext),
                typeof(TaskManagementDbContext),
                typeof(FinanceDbContext),
                typeof(DebtDbContext),
                typeof(TimeTrackingDbContext),
                typeof(StorePlantDbContext)
            };

            foreach (var type in dbContextTypes)
            {
                try
                {
                    var context = (DbContext)services.GetRequiredService(type);
                    var dbName = context.Database.GetDbConnection().Database;
                    
                    logger.LogInformation("Migrating database: {DbName}", dbName);
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Successfully migrated database: {DbName}", dbName);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating a database.");
                }
            }
        }
    }
}
