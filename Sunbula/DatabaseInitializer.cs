using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using DebtInfrastructure.Persistenece.Data;
using FinanceInfrastructure.Persistence.Data;
using PlantInfrastructure.Persistence.Data;
using TaskInfrastructure.Persistence.Data;
using TimeTrackingInfrastructure.Persistence.Data;
using Infrastructure.Persistence.Data; // UserIdentity
using PlantInfrastructure.Persistence.Dataseeding;
using Application.Services.Abstraction; // For IPasswordHasher
using Domain.Entities; // For User
using Domain.Entities.ValueObjects; // For Email
using Domain.Enums; // For UserRole

namespace Sunbula
{
    public static class DatabaseInitializer
    {
        public static async Task MigrateDatabasesAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            var configuration = services.GetRequiredService<IConfiguration>();

            var autoMigrate = configuration.GetValue<bool>("Database:AutoMigrate", true);
            if (!autoMigrate)
            {
                logger.LogInformation("Automatic database migration is disabled via 'Database:AutoMigrate'. Skipping startup migrations.");
                return;
            }

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

        public static async Task SeedDataAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            var env = services.GetRequiredService<IWebHostEnvironment>();

            try
            {
                // 1. Seed Plants
                var storeContext = services.GetRequiredService<StorePlantDbContext>();
                await plantDbIntializer.SeedPlantsAsync(storeContext, logger);
                logger.LogInformation("Plant seeding check completed.");

                // 2. Seed Admin User - strictly gated behind Development environment
                if (!env.IsDevelopment())
                {
                    logger.LogInformation("Skipping default admin user seeding in non-Development environment ({EnvironmentName}).", env.EnvironmentName);
                    return;
                }

                var identityContext = services.GetRequiredService<UserIdentityDbContext>();
                var passwordHasher = services.GetRequiredService<IPasswordHasher>();

                var adminEmail = "admin@sunbula.com";

                var adminUser = identityContext.Users
                  .AsEnumerable()
                  .FirstOrDefault(u => u.Email.Value == adminEmail);
                if (adminUser == null)
                {
                    logger.LogInformation("Creating default admin user for Development environment...");
                    var passwordHash = passwordHasher.HashPassword("AdminPassword123!");
                    var email = Email.Create(adminEmail);
                    
                    var newAdmin = User.CreateAdmin(
                        email,
                        "System",
                        "Admin",
                        passwordHash,
                        "0000000000"
                    );
                    
                    newAdmin.ConfirmEmail(); // Default admin should be confirmed

                    await identityContext.Users.AddAsync(newAdmin);
                    await identityContext.SaveChangesAsync();
                    logger.LogInformation("Default admin user created successfully.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
