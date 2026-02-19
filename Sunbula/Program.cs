
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi;
using Persistance.Data.TaskManagement;
using Infrastructure;

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
   
         
            #region Plant

            #endregion

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
