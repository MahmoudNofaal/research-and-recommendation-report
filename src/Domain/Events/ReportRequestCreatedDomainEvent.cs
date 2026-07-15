using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events
{
    /// <summary>
    /// Raised when a user creates a new draft <c>ReportRequest</c>.
    /// </summary>
    public sealed class ReportRequestCreatedDomainEvent : DomainEvent
    {
        public ReportRequestCreatedDomainEvent(ReportRequestId reportRequestId, UserId userId)
        {
            ReportRequestId = reportRequestId;
            UserId = userId;
        }

        public ReportRequestId ReportRequestId { get; }

        public UserId UserId { get; }
    }
}
