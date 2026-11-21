using Application.Options;
using Sunbula.Middleware;
using DebtInfrastructure;
using FinanceInfrastructure;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PlantInfrastructure;
using Sunbula;
using System.Text;
using TaskInfrastructure;
using TimeTrackingInfrastructure;
using DebtApplication.DebtService;
using FinanceApplication.FinanceServiceAbs;
using PlantApplication.StorePlantServiceAbstraction;
using Application.ServiceAbstraction; // TaskManagement
using TimeTrackingApplication.TimeServiceAbstraction;
using Application.Services.Abstraction; // UserIdentity

namespace Sunbula
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ═══════════════════════════════════════════════════════════
            // CONTROLLERS — single AddControllers() with chained parts
            // ═══════════════════════════════════════════════════════════
            builder.Services.AddControllers()
                .AddApplicationPart(typeof(PresentationIdentity.AssemblyReference).Assembly)
                .AddApplicationPart(typeof(TaskPresentation.AssemblyReference).Assembly)
                .AddApplicationPart(typeof(FinancePresentation.AssemblyReference).Assembly)
                .AddApplicationPart(typeof(DebtPresentation.AssemblyReference).Assembly)
                .AddApplicationPart(typeof(TimeTrackingPresentation.AssemblyReference).Assembly)
                .AddApplicationPart(typeof(PlantPresentation.AssemblyReference).Assembly);

            // ═══════════════════════════════════════════════════════════
            // GLOBAL EXCEPTION HANDLING
            // ═══════════════════════════════════════════════════════════
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();

            builder.Services.AddEndpointsApiExplorer();

            // ═══════════════════════════════════════════════════════════
            // SWAGGER
            // ═══════════════════════════════════════════════════════════
            builder.Services.AddSwaggerDocumentation();

            // ═══════════════════════════════════════════════════════════
            // JWT AUTHENTICATION
            // ═══════════════════════════════════════════════════════════
            var jwtSettings = builder.Configuration.GetRequiredSection("Jwt").Get<JwtOptions>()
                ?? throw new InvalidOperationException(
                    "JWT configuration section 'Jwt' is missing or invalid in appsettings.json. " +
                    "Ensure SecretKey, Issuer, and Audience are set.");

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuers = jwtSettings.Issuer
                            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries),
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };

                    options.Events = new JwtBearerEvents
                    {
                        // SSE (EventSource) and WebSocket clients cannot set
                        // custom HTTP headers, so the JWT is passed via query string.
                        // This hook lets the middleware pick it up.
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddAuthorization();

            // ═══════════════════════════════════════════════════════════
            // CORS
            // ═══════════════════════════════════════════════════════════
            var configuredOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? Array.Empty<string>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.SetIsOriginAllowed(origin =>
                    {
                        if (string.IsNullOrWhiteSpace(origin))
                            return false;

                        // 1. Allow localhost & 127.0.0.1 on any port (Angular :4200, Next.js/React :3000, Vite :5173, etc.)
                        if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                        {
                            if (uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) ||
                                uri.Host.Equals("127.0.0.1", StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }

                            // 2. Allow Vercel production domain & all preview deployments (*.vercel.app)
                            if (uri.Host.Equals("sunbula-front-end.vercel.app", StringComparison.OrdinalIgnoreCase) ||
                                uri.Host.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase))
                            {
                                return true;
                            }
                        }

                        // 3. Allow explicitly configured origins from appsettings.json
                        return configuredOrigins.Any(o =>
                            string.Equals(o.TrimEnd('/'), origin.TrimEnd('/'), StringComparison.OrdinalIgnoreCase));
                    })
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Content-Disposition", "X-Pagination", "Location")
                    .SetPreflightMaxAge(TimeSpan.FromHours(1));
                });
            });

            // ═══════════════════════════════════════════════════════════
            // MEDIATR
            // ═══════════════════════════════════════════════════════════
            builder.Services.AddMediatR(cfg =>
            {
                // Core and Module Application Assemblies
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.RegisterServicesFromAssemblies(
                    typeof(IDebtService).Assembly,
                    typeof(IFinancialCategoryService).Assembly,
                    typeof(IPlantService).Assembly,
                    typeof(ICategoryService).Assembly,
                    typeof(IDailyTransactionService).Assembly,
                    typeof(IAuthenticationService).Assembly,
                    // Infrastructure assemblies for Integration Event Handlers
                    typeof(DebtInfrastructure.AssemblyReference).Assembly,
                    typeof(FinanceInfrastructure.AssemblyReference).Assembly,
                    typeof(PlantInfrastructure.AssemblyReference).Assembly,
                    typeof(TaskInfrastructure.AssemblyReference).Assembly,
                    typeof(TimeTrackingInfrastructure.AssemblyReference).Assembly,
                    typeof(UserIdentityInfrastructure.Services.AuthenticationService).Assembly
                );
            });

            // ═══════════════════════════════════════════════════════════
            // MODULE REGISTRATIONS
            // ═══════════════════════════════════════════════════════════
            builder.Services.AddTaskManagementModule(builder.Configuration);
            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddDebtModule(builder.Configuration);
            builder.Services.AddFinanceModule(builder.Configuration);
            builder.Services.AddTimeTrackingModule(builder.Configuration);
            builder.Services.AddStorePlantModule(builder.Configuration);

            var app = builder.Build();

            // ═══════════════════════════════════════════════════════════
            // DATABASE MIGRATIONS
            // ═══════════════════════════════════════════════════════════
            await app.Services.MigrateDatabasesAsync();
            await app.Services.SeedDataAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sunbula API v1");
                    c.RoutePrefix = "swagger";
                });
            }

            app.UseExceptionHandler();
            // app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("AllowFrontend");

            app.UseAuthentication();
             app.UseMiddleware<RequireEmailConfirmedMiddleware>();
            app.UseAuthorization();

            app.MapGet("/health", () => "OK");
            app.MapControllers();

            await app.RunAsync();
        }
    }
}


