using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TimeTrackingInfrastructure.Persistence.Data;

namespace TimeTrackingInfrastructure
{
    public static class AddTimeTrackingInfrastructureService
    {
        public static IServiceCollection AddTimeTrackingModule(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            // ── DbContext ──────────────────────────────────────────────
            services.AddDbContext<TimeTrackingDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("TimeTrackingDb"),
                    b => b.MigrationsAssembly(typeof(TimeTrackingDbContext).Assembly.FullName)));

            //// ── Unit of Work ───────────────────────────────────────────
            //services.AddScoped<IUnitOfWork, UnitOfWork>();

            //// ── Repositories ───────────────────────────────────────────
            //services.AddScoped<ITimeSessionRepository, TimeSessionRepository>();
            //services.AddScoped<IDailyTransactionRepository, DailyTransactionRepository>();

            //// ── Services ───────────────────────────────────────────────
            //services.AddScoped<ITimeSessionService, TimeSessionService>();
            //services.AddScoped<IDailyTransactionService, DailyTransactionService>();

            //// ── AutoMapper ─────────────────────────────────────────────
            //services.AddAutoMapper(typeof(TimeTrackingMappingProfile).Assembly);

            return services;
        }
    }
}
