using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Domain.Entities.TaskManagement;
using TaskDomain.Entities.TaskManagement;

namespace TaskInfrastructure.Persistence.Data
{
    public class TaskManagementDbContext : DbContext
    {
        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
            : base(options) { }

        public DbSet<Domain.Entities.TaskManagement.Task> Tasks => Set<Domain.Entities.TaskManagement.Task>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Folder> Folders => Set<Folder>();
        public DbSet<TaskCategory> TaskCategories => Set<TaskCategory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TaskManagement");
            modelBuilder.Ignore<TaskDomain.Entities.TaskManagement.ValueObjects.TaskColor>();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskManagementDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }

    public class TaskManagementDbContextFactory : IDesignTimeDbContextFactory<TaskManagementDbContext>
    {
        public TaskManagementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TaskManagementDbContext>();
            optionsBuilder.UseSqlServer("Server=.;Database=TaskManagement;Trusted_Connection=True;TrustServerCertificate=True");
            return new TaskManagementDbContext(optionsBuilder.Options);
        }
    }
}
