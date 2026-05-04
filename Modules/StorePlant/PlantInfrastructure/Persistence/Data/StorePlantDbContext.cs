using Microsoft.EntityFrameworkCore;
using PlantDomain.Entities;
using SharedKernel;

namespace PlantInfrastructure.Persistence.Data
{
    public sealed class StorePlantDbContext:DbContext
    {
        private readonly MediatR.IMediator _mediator;

        public StorePlantDbContext(DbContextOptions<StorePlantDbContext> options, MediatR.IMediator mediator)
            : base(options) 
        {
            _mediator = mediator;
        }

        public DbSet<Plant> Plants => Set<Plant>();
        public DbSet<UserPlant> UserPlants => Set<UserPlant>();
        public DbSet<GrowthHistory> GrowthHistory => Set<GrowthHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("plant");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StorePlantDbContext).Assembly);

            // Global soft delete filters
            modelBuilder.Entity<Plant>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<UserPlant>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<GrowthHistory>().HasQueryFilter(e => !e.IsDeleted);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var domainEvents = ChangeTracker
                .Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .SelectMany(e => e.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            ChangeTracker
                .Entries<BaseEntity>()
                .Select(e => e.Entity)
                .ToList()
                .ForEach(e => e.ClearDomainEvents());

            // Dispatch domain events via MediatR
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            return result;
        }
    }
}
