namespace Domain.Common
{
    /// <summary>
    /// Base type for every aggregate root: the single entry point through which a
    /// consistency boundary is loaded, mutated, and persisted. Only aggregate roots
    /// are referenced from repositories; internal entities are reached only through
    /// their owning root.
    /// </summary>
    /// <typeparam name="TId">The strongly typed identifier for this aggregate root.</typeparam>
    public abstract class AggregateRoot<TId> : Entity<TId>
        where TId : notnull
    {
        private readonly List<IDomainEvent> _domainEvents = [];

        protected AggregateRoot(TId id)
            : base(id)
        {
        }

        /// <summary>
        /// Domain events raised by this aggregate that have not yet been dispatched.
        /// Infrastructure is responsible for publishing and clearing these after a
        /// successful commit; the aggregate itself never dispatches its own events.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
