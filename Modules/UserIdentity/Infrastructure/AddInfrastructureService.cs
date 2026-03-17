using Application.Mappings;
using Application.Options;
using Application.Services.Abstraction;
using Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserIdentityInfrastructure.Persistence.Repositories;
using UserIdentityInfrastructure.Services;

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
            // AutoMapper
            services.AddAutoMapper(cfg =>cfg.AddProfile<UserMappingProfile>());

            // Unit of Work & Repositories
            services.AddScoped<IUnitOfWork, UserUnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // Options
            services.Configure<JwtOptions>(
                configuration.GetSection(JwtOptions.SectionName));
            return services;
        }
    }
}
