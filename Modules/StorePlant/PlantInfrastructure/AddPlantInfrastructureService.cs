using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlantApplication.StoreplantMappings;
using PlantApplication.StorePlantServiceAbstraction;
using PlantDomain.Contracts;
using PlantInfrastructure.Persistence.Data;
using PlantInfrastructure.Persistence.Repositories;
using PlantInfrastructure.StorePlantServices;

namespace PlantInfrastructure
{
    public  static class AddPlantInfrastructureService
    {
        public static IServiceCollection AddStorePlantModule(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            // ── DbContext ──────────────────────────────────────────────
            services.AddDbContext<StorePlantDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("StorePlantDb"),
                    b => b.MigrationsAssembly(typeof(StorePlantDbContext).Assembly.FullName)
                          .CommandTimeout(30))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());

            // ── AutoMapper ─────────────────────────────────────────────
            services.AddAutoMapper(cfg => { }, typeof(StorePlantMappingProfile).Assembly);

            // ── Repositories ───────────────────────────────────────────
            services.AddScoped<IPlantRepository, PlantRepository>();
            services.AddScoped<IUserPlantRepository, UserPlantRepository>();

            // ── Unit of Work ───────────────────────────────────────────
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ── Application Services ───────────────────────────────────
            services.AddScoped<IPlantService, PlantService>();
            services.AddScoped<IUserPlantService, UserPlantService>();

            return services;
        }
    }
}
