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
        public static void Main(string[] args)
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
            var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings!.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            builder.Services.AddAuthorization();

            // ═══════════════════════════════════════════════════════════
            // CORS
            // ═══════════════════════════════════════════════════════════
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyMethod()
                          .AllowAnyHeader();
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

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseMiddleware<RequireEmailConfirmedMiddleware>();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
