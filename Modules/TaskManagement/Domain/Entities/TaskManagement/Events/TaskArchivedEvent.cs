using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDomain.Entities.TaskManagement.Events
{
    public class TaskArchivedEvent : SharedKernel.IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public Guid TaskId { get; }
        public Guid UserId { get; }

        public TaskArchivedEvent(Guid taskId, Guid userId)
        {
            TaskId = taskId;
            UserId = userId;
        }
    }
}
