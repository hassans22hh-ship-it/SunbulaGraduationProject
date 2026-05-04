using Microsoft.EntityFrameworkCore;
using SharedKernel;
using TimeTrackingDomain.Entities;

namespace TimeTrackingInfrastructure.Persistence.Data
{
    public sealed  class TimeTrackingDbContext : DbContext
    {
        private readonly MediatR.IMediator _mediator;

        public TimeTrackingDbContext(DbContextOptions<TimeTrackingDbContext> options, MediatR.IMediator mediator)
            : base(options) 
        {
            _mediator = mediator;
        }

        public DbSet<TimeSession> TimeSessions => Set<TimeSession>();
        public DbSet<DailyTransaction> DailyTransactions => Set<DailyTransaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("tracking");
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TimeTrackingDbContext).Assembly);

            // Global soft-delete filter
            modelBuilder.Entity<TimeSession>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<DailyTransaction>().HasQueryFilter(e => !e.IsDeleted);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Collect domain events before save
            var domainEvents = ChangeTracker
                .Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .SelectMany(e => e.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            // Clear domain events after save
            ChangeTracker
                .Entries<BaseEntity>()
                .Select(e => e.Entity)
                .ToList()
                .ForEach(e => e.ClearDomainEvents());

            // Dispatch domain events via MediatR/EventBus
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            return result;
        }
    }
}
