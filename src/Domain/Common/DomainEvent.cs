namespace Domain.Common
{
    /// <summary>
    /// Base type for all domain events raised by aggregates in this model.
    /// The occurrence timestamp is captured at construction time, since raising
    /// the event and the underlying fact happening are, for every event in this
    /// model, the same instant from the aggregate's point of view.
    /// </summary>
    public abstract class DomainEvent : IDomainEvent
    {
        protected DomainEvent()
        {
            OccurredOnUtc = DateTime.UtcNow;
        }

        public DateTime OccurredOnUtc { get; }
    }
}
