using Task = Domain.Entities.TaskManagement.Task;
using Category = Domain.Entities.TaskManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskDomain.Entities.TaskManagement.ValueObjects;

namespace TaskInfrastructure.Persistence.Configurations
{
    public sealed class TaskConfiguration:IEntityTypeConfiguration<Task>
    {
        public void Configure(EntityTypeBuilder<Task> builder)
        {
            // Table
            builder.ToTable("Tasks", schema: "TaskManagement");

            // Primary Key
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).ValueGeneratedNever();

            // Properties
            builder.Property(t => t.UserId)
                .IsRequired();

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Emoji)
                .HasMaxLength(10);

            // Value Object - TaskColor
            builder.Property(t => t.Color)
                .HasConversion(
                    color => color.Value,
                    value => TaskColor.Create(value))
                .IsRequired()
                .HasMaxLength(7);

            // Enum - BehaviorCategory
            builder.Property(t => t.BehaviorType)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(t => t.FolderId);

            // Enum - TaskStatus
            builder.Property(t => t.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(t => t.IsArchived)
                .IsRequired()
                .HasDefaultValue(false);

            // Audit fields
            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.UpdatedAt);

            builder.Property(t => t.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(t => t.Folder)
                .WithMany(f => f.Tasks)
                .HasForeignKey(t => t.FolderId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(t => t.TaskCategories)
                .WithOne(tc => tc.Tasks)
                .HasForeignKey(tc => tc.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(t => t.UserId)
                .HasDatabaseName("IX_Tasks_UserId");

            builder.HasIndex(t => new { t.UserId, t.Title })
                .IsUnique()
                .HasDatabaseName("IX_Tasks_UserId_Title");

            builder.HasIndex(t => t.FolderId)
                .HasDatabaseName("IX_Tasks_FolderId");

            builder.HasIndex(t => t.BehaviorType)
                .HasDatabaseName("IX_Tasks_BehaviorType");

            builder.HasIndex(t => t.Status)
                .HasDatabaseName("IX_Tasks_Status");

            builder.HasIndex(t => t.IsArchived)
                .HasDatabaseName("IX_Tasks_IsArchived");

            builder.HasIndex(t => t.CreatedAt)
                .HasDatabaseName("IX_Tasks_CreatedAt");

            // Ignore domain events
            builder.Ignore(t => t.DomainEvents);
        }
    }
}
