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
                options.UseSqlServer(configuration.GetConnectionString("SunbulaDb")));
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
            services.Configure<EmailOptions>(
                configuration.GetSection("EmailSettings"));

            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IUserIntegrationService, UserIntegrationService>();
            services.AddScoped<IUserSettingsService, UserSettingsService>();

            // Coin Stream Manager
            services.AddSingleton<ICoinStreamManager, CoinStreamManager>();

            return services;
        }
    }
}
