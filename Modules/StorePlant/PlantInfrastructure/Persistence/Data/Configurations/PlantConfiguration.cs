using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlantDomain.Entities;

namespace PlantInfrastructure.Persistence.Data.Configurations
{
    public sealed  class PlantConfiguration:IEntityTypeConfiguration<Plant>
    {
        public void Configure(EntityTypeBuilder<Plant> builder)
        {
            builder.ToTable("Plants", schema: "store");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedNever();

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.BotanicName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.ImageUrl)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(p => p.Price)
                .IsRequired();

            builder.Property(p => p.Level)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(p => p.Decoration)
                .HasMaxLength(200);

            builder.Property(p => p.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(p => p.IsSeasonal)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.SeasonStart);
            builder.Property(p => p.SeasonEnd);
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.UpdatedAt);
            builder.Property(p => p.IsDeleted).IsRequired().HasDefaultValue(false);

            builder.HasMany(p => p.UserPlants)
                .WithOne(up => up.Plant)
                .HasForeignKey(up => up.PlantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.Name)
                .IsUnique()
                .HasDatabaseName("IX_Plants_Name");

            builder.HasIndex(p => p.Level)
                .HasDatabaseName("IX_Plants_Level");

            builder.HasIndex(p => p.IsAvailable)
                .HasDatabaseName("IX_Plants_IsAvailable");

            builder.Ignore(p => p.DomainEvents);
        }
    }
}
