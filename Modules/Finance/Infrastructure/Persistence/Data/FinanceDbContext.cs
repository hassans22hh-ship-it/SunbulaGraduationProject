using FinanceDomain.Entities;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace FinanceInfrastructure.Persistence.Data
{
    public class FinanceDbContext:DbContext
    {
        public FinanceDbContext(DbContextOptions<FinanceDbContext> options) : base(options) { }

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

            // TODO: Dispatch domain events via MediatR / event bus

            return result;
        }
    }
}
