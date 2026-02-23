
using DebtDomain.Entities;
using DebtDomain.ValueObjects;

namespace DebtInfrastructure.Persistenece.Data
{
    public sealed class DebtDbContext : DbContext
    {

        public DebtDbContext(DbContextOptions<DebtDbContext> options)
            : base(options)
        {
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
            modelBuilder.Ignore<Money>();

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

            // TODO: Dispatch domain events (MediatR integration)

            return result;
        }
    }
}
