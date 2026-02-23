
using DebtInfrastructure;
using FinanceInfrastructure;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using TaskInfrastructure;
using TimeTrackingInfrastructure;
using PlantInfrastructure;
using Persistance.Data.StorePlant;
using Persistance.Data.TaskManagement;

namespace Sunbula
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
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

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }


    }
}
