using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDomain.Entities.TaskManagement.Events
{
    public class BehaviorTypeChangedEvent : SharedKernel.IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        public Guid TaskId { get; }
        public Guid UserId { get; }
        public Enums.BehaviorCategory NewBehaviorType { get; }

        public BehaviorTypeChangedEvent(Guid taskId, Guid userId, Enums.BehaviorCategory newBehaviorType)
        {
            TaskId = taskId;
            UserId = userId;
            NewBehaviorType = newBehaviorType;
        }
    }
}
