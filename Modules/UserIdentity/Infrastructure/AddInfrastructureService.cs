using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public static class AddInfrastructureService
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<Persistence.Data.UserIdentityDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("UserIdentityDbContext")));
            // Register repositories, services, etc. here
            // e.g. services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
