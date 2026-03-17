using Domain.Entities.TaskManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskDomain.Entities.TaskManagement.ValueObjects;

namespace TaskInfrastructure.Persistence.Configurations
{
    public class FolderConfiguration:IEntityTypeConfiguration<Domain.Entities.TaskManagement.Folder>
    {
        public void Configure(EntityTypeBuilder<Folder> builder)
        {
            // Table
            builder.ToTable("Folders", schema: "TaskManagement");

            // Primary Key
            builder.HasKey(f => f.Id);
            builder.Property(f => f.Id).ValueGeneratedNever();

            // Properties
            builder.Property(f => f.UserId)
                .IsRequired();

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Value Object - TaskColor
            builder.Property(f => f.Color)
                .HasConversion(
                    color => color.Value,
                    value => TaskColor.Create(value))
                .IsRequired()
                .HasMaxLength(7);

            // Audit fields
            builder.Property(f => f.CreatedAt)
                .IsRequired();

            builder.Property(f => f.UpdatedAt);

            builder.Property(f => f.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Relationships
            builder.HasMany(f => f.Tasks)
                .WithOne(t => t.Folder)
                .HasForeignKey(t => t.FolderId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(f => f.UserId)
                .HasDatabaseName("IX_Folders_UserId");

            builder.HasIndex(f => new { f.UserId, f.Name })
                .IsUnique()
                .HasDatabaseName("IX_Folders_UserId_Name");

            // Ignore domain events
            builder.Ignore(f => f.DomainEvents);
        }
    }
}
