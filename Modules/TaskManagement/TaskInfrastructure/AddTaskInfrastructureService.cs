using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskInfrastructure.Persistence.Data;

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
                    configuration.GetConnectionString("TaskManagementDb"),
                    b => b.MigrationsAssembly(typeof(TaskManagementDbContext).Assembly.FullName)));

            // Unit of Work
            //services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositories
            //services.AddScoped<ITaskRepository, TaskRepository>();
            //services.AddScoped<ICategoryRepository, CategoryRepository>();
            //services.AddScoped<IFolderRepository, FolderRepository>();

            // Services
            //services.AddScoped<ITaskService, TaskService>();
            //services.AddScoped<ICategoryService, CategoryService>();
            //services.AddScoped<IFolderService, FolderService>();

            return services;
        }
    }
}
