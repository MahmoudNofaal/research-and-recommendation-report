namespace Domain.Common
{
    /// <summary>
    /// Marks a type as a domain event: an immutable record of something meaningful
    /// that happened to an aggregate, expressed in the language of the business.
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// The instant, in UTC, at which the underlying business fact occurred.
        /// </summary>
        DateTime OccurredOnUtc { get; }
    }
}
