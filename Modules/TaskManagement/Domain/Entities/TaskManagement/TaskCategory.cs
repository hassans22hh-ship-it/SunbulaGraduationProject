using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDomain.Entities.TaskManagement
{
    public class TaskCategory : SharedKernel.BaseEntity
    {
        public Guid TaskId { get; private set; }
        public Guid CategoryId { get; private set; }

        private TaskCategory() { } // EF Core

        private TaskCategory(Guid taskId, Guid categoryId) : base(Guid.NewGuid())
        {
            TaskId = taskId;
            CategoryId = categoryId;
        }

        public static TaskCategory Create(Guid taskId, Guid categoryId)
        {
            return new TaskCategory(taskId, categoryId);
        }
    }
}
