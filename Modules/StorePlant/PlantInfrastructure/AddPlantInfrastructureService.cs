using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlantInfrastructure.Persistence.Data;

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
                    b => b.MigrationsAssembly(typeof(StorePlantDbContext).Assembly.FullName)));

            //// ── AutoMapper ─────────────────────────────────────────────
            //services.AddAutoMapper(typeof(StorePlantMappingProfile).Assembly);

            //// ── Repositories ───────────────────────────────────────────
            //services.AddScoped<IPlantRepository, PlantRepository>();
            //services.AddScoped<IUserPlantRepository, UserPlantRepository>();

            //// ── Unit of Work ───────────────────────────────────────────
            //services.AddScoped<IUnitOfWork, UnitOfWork>();

            //// ── Application Services ───────────────────────────────────
            //services.AddScoped<IPlantService, PlantService>();
            //services.AddScoped<IUserPlantService, UserPlantService>();

            return services;
        }
    }
}
