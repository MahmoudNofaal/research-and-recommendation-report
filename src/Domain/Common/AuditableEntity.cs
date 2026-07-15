namespace Domain.Common
{
    /// <summary>
    /// Base type for aggregate roots that need generic "record created / record last
    /// modified" bookkeeping with no further business meaning attached to either
    /// timestamp. These properties are stamped by Infrastructure (a SaveChanges
    /// interceptor) at persistence time, never by the aggregate's own business
    /// methods — the internal setters keep them out of reach of anything other
    /// than Infrastructure and Domain itself.
    ///
    /// Entities whose "created" or "updated" moment IS a meaningful business fact
    /// (for example, a generated report's generation timestamp) intentionally do
    /// not use this base type — see <c>GeneratedReport</c>.
    /// </summary>
    /// <typeparam name="TId">The strongly typed identifier for this aggregate root.</typeparam>
    public abstract class AuditableEntity<TId> : AggregateRoot<TId>
        where TId : notnull
    {
        protected AuditableEntity(TId id)
            : base(id)
        {
        }

        public DateTime CreatedAtUtc { get; internal set; }

        public DateTime UpdatedAtUtc { get; internal set; }
    }
}
