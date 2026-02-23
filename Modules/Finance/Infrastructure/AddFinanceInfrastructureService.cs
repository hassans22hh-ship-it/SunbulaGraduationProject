using FinanceInfrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceInfrastructure
{
    public static class AddFinanceInfrastructureService
    {
        public static IServiceCollection AddFinanceModule(
       this IServiceCollection services,
       IConfiguration configuration)
        {
            // ── DbContext ────────────────────────────────────────────────────────
            services.AddDbContext<FinanceDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("FinanceDb"),
                    b => b.MigrationsAssembly(typeof(FinanceDbContext).Assembly.FullName)));

            return services;
        }
    }
}
