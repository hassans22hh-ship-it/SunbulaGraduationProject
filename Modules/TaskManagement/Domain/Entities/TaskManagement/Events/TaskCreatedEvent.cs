using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDomain.Entities.TaskManagement.Events
{
    public class TaskCreatedEvent : SharedKernel.IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public Guid TaskId { get; }
        public Guid UserId { get; }
        public Enums.BehaviorCategory BehaviorType { get; }

        public TaskCreatedEvent(Guid taskId, Guid userId, Enums.BehaviorCategory behaviorType)
        {
            TaskId = taskId;
            UserId = userId;
            BehaviorType = behaviorType;
        }
    }
}
