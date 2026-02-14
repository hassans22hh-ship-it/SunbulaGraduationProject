

using Domain.Entities.StorePlant;

namespace Persistance.Data.StorePlant
{
    public class StorePlantDbContext : DbContext
    {
        public StorePlantDbContext(DbContextOptions<StorePlantDbContext> options)
       : base(options) { }

        public DbSet<Plant> Plants => Set<Plant>();
        public DbSet<UserPlant> UserPlants => Set<UserPlant>();
        public DbSet<GrowthHistory> GrowthHistories => Set<GrowthHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StorePlantDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
