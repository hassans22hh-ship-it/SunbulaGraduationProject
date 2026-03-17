
namespace SharedKernel
{
    public abstract class BaseEntity : IEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        protected BaseEntity(Guid id) {
            Id = id;
        }
        protected BaseEntity() { }
        public Guid Id { get; private set; }
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; private set; }
        public bool IsDeleted { get; private set; }
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;
        public void MarkAsDeleted()
        {
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
        protected void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;

        }
        protected void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }


    }
}
