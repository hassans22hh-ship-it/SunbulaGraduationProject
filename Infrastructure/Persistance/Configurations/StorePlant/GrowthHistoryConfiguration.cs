
using Domain.Entities.StorePlant;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistance.Configurations.StorePlant
{
    internal class GrowthHistoryConfiguration:IEntityTypeConfiguration<GrowthHistory>
    {
        public void Configure(EntityTypeBuilder<GrowthHistory> builder)
        {
            builder.ToTable("GrowthHistories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Stage).IsRequired();
            builder.Property(x => x.GrowthDate).IsRequired();
            builder.Property(x => x.AchievedAt).IsRequired();

            // UserPlant 1 -> Many GrowthHistory
            builder.HasOne(x => x.UserPlant)
                .WithMany(x => x.GrowthHistory)
                .HasForeignKey(x => x.UserPlantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserPlantId);
        }
    }
}
