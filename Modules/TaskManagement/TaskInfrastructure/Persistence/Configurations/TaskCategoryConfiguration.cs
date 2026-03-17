
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskDomain.Entities.TaskManagement;

namespace TaskInfrastructure.Persistence.Configurations
{
    public class TaskCategoryConfiguration: IEntityTypeConfiguration<TaskCategory>
    {
        public void Configure(EntityTypeBuilder<TaskCategory> builder)
        {
            // Table
            builder.ToTable("TaskCategories", schema: "TaskManagement");

            // Primary Key
            builder.HasKey(tc => tc.Id);
            builder.Property(tc => tc.Id).ValueGeneratedNever();

            // Properties
            builder.Property(tc => tc.TaskId)
                .IsRequired();

            builder.Property(tc => tc.CategoryId)
                .IsRequired();

            // Audit fields
            builder.Property(tc => tc.CreatedAt)
                .IsRequired();

            builder.Property(tc => tc.UpdatedAt);

            builder.Property(tc => tc.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // Relationships configured in Task and Category

            // Indexes
            builder.HasIndex(tc => new { tc.TaskId, tc.CategoryId })
                .IsUnique()
                .HasDatabaseName("IX_TaskCategories_TaskId_CategoryId");

            builder.HasIndex(tc => tc.TaskId)
                .HasDatabaseName("IX_TaskCategories_TaskId");

            builder.HasIndex(tc => tc.CategoryId)
                .HasDatabaseName("IX_TaskCategories_CategoryId");

            // Ignore domain events
            builder.Ignore(tc => tc.DomainEvents);
        }
    }
}
