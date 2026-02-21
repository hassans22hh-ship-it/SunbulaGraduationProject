using SharedKernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace TaskDomain.Entities.TaskManagement.Events
{
    public sealed record  FolderCreatedEvent(Guid FolderId, Guid UserId) : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
    }
}
