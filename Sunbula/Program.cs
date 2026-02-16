
using Microsoft.EntityFrameworkCore;
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
            #region ConnectionByDb
            #region TaskManagementDb
            /*
               * ===================================================
              //TaskManagementDb 
              ======================================================
              madeby:HassanSaied
              Date:8-2-2026
              ========================================>
              ***************************************************

              */
            builder.Services.AddDbContext<TaskManagementDbContext>(options =>
  options.UseSqlServer(
      builder.Configuration.GetConnectionString("TaskManagementDb")));
            #endregion
            #region StorePlantDb
            /*
               * ===================================================
              //StorePlantDb 
              ======================================================
              madeby:Yara Mahmoud
              Date:14-2-2026
              ========================================>
              ***************************************************

              */
            builder.Services.AddDbContext<StorePlantDbContext>(options =>
  options.UseSqlServer(
      builder.Configuration.GetConnectionString("StorePlantDb")));
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
