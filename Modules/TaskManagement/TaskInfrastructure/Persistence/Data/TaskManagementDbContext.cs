using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Domain.Entities.TaskManagement;
using TaskDomain.Entities.TaskManagement;

namespace TaskInfrastructure.Persistence.Data
{
    public class TaskManagementDbContext : DbContext
    {
        private readonly MediatR.IMediator _mediator;

        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options, MediatR.IMediator mediator)
            : base(options) 
        {
            _mediator = mediator;
        }

        public DbSet<Domain.Entities.TaskManagement.TaskItem> Tasks => Set<Domain.Entities.TaskManagement.TaskItem>();
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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = ChangeTracker
                .Entries<SharedKernel.BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .SelectMany(e => e.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            ChangeTracker
                .Entries<SharedKernel.BaseEntity>()
                .Select(e => e.Entity)
                .ToList()
                .ForEach(e => e.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            return result;
        }
    }

    public class TaskManagementDbContextFactory : IDesignTimeDbContextFactory<TaskManagementDbContext>
    {
        public TaskManagementDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TaskManagementDbContext>();
            optionsBuilder.UseSqlServer("Server=.;Database=TaskManagement;Trusted_Connection=True;TrustServerCertificate=True");
            return new TaskManagementDbContext(optionsBuilder.Options, null!);
        }
    }
}
