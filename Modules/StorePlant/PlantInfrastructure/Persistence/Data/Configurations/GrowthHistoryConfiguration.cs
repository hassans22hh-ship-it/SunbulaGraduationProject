using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantDomain.Entities;

namespace PlantInfrastructure.Persistence.Data.Configurations
{
    public class GrowthHistoryConfiguration:IEntityTypeConfiguration<GrowthHistory>
    {
        public void Configure(EntityTypeBuilder<GrowthHistory> builder)
        {
            builder.ToTable("GrowthHistories", schema: "store");

            builder.HasKey(gh => gh.Id);
            builder.Property(gh => gh.Id).ValueGeneratedNever();

            builder.Property(gh => gh.UserPlantId).IsRequired();

            builder.Property(gh => gh.Stage)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(gh => gh.AchievementId).IsRequired();
            builder.Property(gh => gh.GrowthDate).IsRequired();
            builder.Property(gh => gh.CreatedAt).IsRequired();
            builder.Property(gh => gh.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.HasOne(gh => gh.UserPlant)
                .WithMany(up => up.GrowthHistories)
                .HasForeignKey(gh => gh.UserPlantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(gh => gh.UserPlantId)
                .HasDatabaseName("IX_GrowthHistories_UserPlantId");

            builder.Ignore(gh => gh.DomainEvents);
        }
    }
}
