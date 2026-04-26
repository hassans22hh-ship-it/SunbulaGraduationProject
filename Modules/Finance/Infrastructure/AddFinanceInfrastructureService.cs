using FinanceApplication.FinanceServiceAbs;
using FinanceApplication.Mapping;
using FinanceDomain.contracts;
using FinanceInfrastructure.financeSService;
using FinanceInfrastructure.Persistence.Data;
using FinanceInfrastructure.Persistence.Repositories;
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
                    configuration.GetConnectionString("SunbulaDb"),
                    b => b.MigrationsAssembly(typeof(FinanceDbContext).Assembly.FullName)));

            // ── Repositories & UoW ──────────────────────────────────────────────
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ── Services ────────────────────────────────────────────────────────
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IFinancialTransactionService, FinancialTransactionService>();
            services.AddScoped<IFinancialCategoryService, FinancialCategoryService>();

            // ── Mapping ─────────────────────────────────────────────────────────
            services.AddAutoMapper(cfg => cfg.AddProfile<FinanceMappingProfile>());

            return services;
        }
    }
}

