using FinanceDomain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace FinanceInfrastructure.Persistence.Data
{
    public class FinanceDbContext : DbContext
    {
        private readonly IMediator _mediator;

        public FinanceDbContext(DbContextOptions<FinanceDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }

        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<FinancialCategory> FinancialCategories => Set<FinancialCategory>();
        public DbSet<FinancialTransaction> FinancialTransactions => Set<FinancialTransaction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinanceDbContext).Assembly);

            // Global soft-delete filters
            modelBuilder.Entity<Wallet>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<FinancialCategory>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<FinancialTransaction>().HasQueryFilter(e => !e.IsDeleted);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Collect domain events before saving
            var entities = ChangeTracker
                .Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Any())
                .ToList();

            var domainEvents = entities.SelectMany(e => e.DomainEvents).ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            // Clear events after save
            entities.ForEach(e => e.ClearDomainEvents());

            // Dispatch domain events via MediatR
            foreach (var domainEvent in domainEvents)
                await _mediator.Publish(domainEvent, cancellationToken);

            return result;
        }
    }
}
