using DebtInfrastructure.Persistenece.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DebtInfrastructure
{
    public static class AddDebtInfrastructureService
    {
        public static IServiceCollection AddDebtModule(
    this IServiceCollection services,
    IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<DebtDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DebtDb"),
                    b => b.MigrationsAssembly(typeof(DebtDbContext).Assembly.FullName)));

            //// Unit of Work
            //services.AddScoped<IUnitOfWork, UnitOfWork>();

            //// Repositories
            //services.AddScoped<IDebtRepository, DebtRepository>();

            //// Services
            //services.AddScoped<IDebtService, DebtService>();

            return services;
        }

    }
}
