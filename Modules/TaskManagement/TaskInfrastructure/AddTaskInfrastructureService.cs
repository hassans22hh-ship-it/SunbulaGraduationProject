using Application.ServiceAbstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskDomain.Contracts;
using TaskInfrastructure.Persistence.Data;
using TaskInfrastructure.Persistence.Repositories;
using TaskInfrastructure.Services;

namespace TaskInfrastructure
{
    public static class AddTaskInfrastructureService
    {
        public static IServiceCollection AddTaskManagementModule(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<TaskManagementDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("SunbulaDb"),
                    b => b.MigrationsAssembly(typeof(TaskManagementDbContext).Assembly.FullName)));

            // Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //Repositories
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IFolderRepository, FolderRepository>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<IReportsService, ReportsService>();

            return services;
        }
    }
}
