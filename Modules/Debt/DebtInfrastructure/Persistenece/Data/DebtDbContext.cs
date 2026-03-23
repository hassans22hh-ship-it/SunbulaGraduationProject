
using DebtDomain.Entities;
using DebtDomain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace DebtInfrastructure.Persistenece.Data
{
    public sealed class DebtDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public DebtDbContext(DbContextOptions<DebtDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<DebtDomain.Entities.Debt> Debts => Set<DebtDomain.Entities.Debt>();
        public DbSet<DebtPayment> DebtPayments => Set<DebtPayment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DebtDbContext).Assembly);

            // Global query filter for soft delete
            modelBuilder.Entity<DebtDomain.Entities.Debt>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<DebtPayment>().HasQueryFilter(e => !e.IsDeleted);

            // Map Money value objects as owned types
            modelBuilder.Entity<DebtDomain.Entities.Debt>().OwnsOne(d => d.Amount, m =>
                m.Property(p => p.Value).HasColumnName("Amount"));
            modelBuilder.Entity<DebtDomain.Entities.Debt>().OwnsOne(d => d.RemainingAmount, m =>
                m.Property(p => p.Value).HasColumnName("RemainingAmount"));
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Collect domain events
            var domainEvents = ChangeTracker
                .Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .SelectMany(e => e.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            // Clear domain events
            ChangeTracker
                .Entries<BaseEntity>()
                .Select(e => e.Entity)
                .ToList()
                .ForEach(e => e.ClearDomainEvents());

            // Dispatch domain events via MediatR
            foreach (var domainEvent in domainEvents)
                await _mediator.Publish(domainEvent, cancellationToken);

            return result;
        }
    }
}
