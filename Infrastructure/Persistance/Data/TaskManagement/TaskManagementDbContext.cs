
using Domain.Entities.TaskManagement;

namespace Persistance.Data.TaskManagement
{
    public class TaskManagementDbContext:DbContext
    {
        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options)
        : base(options)
        {
        }

        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Folder> Folders => Set<Folder>();
   

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(TaskManagementDbContext).Assembly);

            base.OnModelCreating(builder);
        }
    }
}
