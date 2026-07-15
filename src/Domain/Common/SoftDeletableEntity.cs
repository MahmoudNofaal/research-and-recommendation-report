using Domain.Errors;
using Domain.Exceptions;

namespace Domain.Common
{
    /// <summary>
    /// Base type for aggregate roots that support soft deletion: the record is
    /// never physically removed, only marked as deleted so it disappears from
    /// normal history while remaining available for audit purposes.
    /// </summary>
    /// <typeparam name="TId">The strongly typed identifier for this aggregate root.</typeparam>
    public abstract class SoftDeletableEntity<TId> : AuditableEntity<TId>
        where TId : notnull
    {
        protected SoftDeletableEntity(TId id)
            : base(id)
        {
        }

        public DateTime? DeletedAtUtc { get; private set; }

        public bool IsDeleted => DeletedAtUtc is not null;

        /// <summary>
        /// Marks the entity as deleted at the given instant. Deletion is a one-way
        /// transition in this model — there is no supported "restore" workflow.
        /// </summary>
        public virtual void Delete(DateTime deletedAtUtc)
        {
            if (IsDeleted)
            {
                throw new InvalidReportStateException(
                    new DomainError(
                        "Entity.AlreadyDeleted",
                        $"{GetType().Name} with id '{Id}' has already been deleted."));
            }

            DeletedAtUtc = deletedAtUtc;
        }
    }
}
