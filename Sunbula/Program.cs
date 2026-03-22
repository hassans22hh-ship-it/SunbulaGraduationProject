
using Application.Options;
using DebtInfrastructure;
using FinanceInfrastructure;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using PlantInfrastructure;
using System.Text;
using TaskInfrastructure;
using TimeTrackingInfrastructure;

namespace Sunbula
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddApplicationPart(typeof(PresentationIdentity.AssemblyReference).Assembly);
            builder.Services.AddControllers().AddApplicationPart(typeof(TaskPresentation.AssemblyReference).Assembly);
             builder.Services.AddControllers().AddApplicationPart(typeof(FinancePresentation.AssemblyReference).Assembly);
            builder.Services.AddControllers().AddApplicationPart(typeof(DebtPresentation.AssemblyReference).Assembly);
            builder.Services.AddControllers().AddApplicationPart(typeof(TimeTrackingPresentation.AssemblyReference).Assembly);
            builder.Services.AddControllers().AddApplicationPart(typeof(PlantPresentation.AssemblyReference).Assembly);


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            // ═══════════════════════════════════════════════════════════
            // SWAGGER
            // ═══════════════════════════════════════════════════════════
            builder.Services.AddSwaggerGen();

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
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")

                            .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            #region Tasks
            builder.Services.AddTaskManagementModule(builder.Configuration);
            #endregion
            #region UserIdentity
            builder.Services.AddInfrastructure(builder.Configuration);
            #endregion
            #region ConnectionByDb

            builder.Services.AddDebtModule(builder.Configuration);
            builder.Services.AddFinanceModule(builder.Configuration);
             builder.Services.AddTimeTrackingModule(builder.Configuration);
            builder.Services.AddStorePlantModule(builder.Configuration);
            #endregion
            var app = builder.Build();



            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI();

            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");

            app.UseAuthentication();    

            app.UseAuthorization();

           
            app.MapControllers();

            app.Run();
        }


    }
}
