using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events
{
    /// <summary>
    /// Raised when a user soft-deletes a <c>GeneratedReport</c>, removing it from
    /// normal history.
    /// </summary>
    public sealed class ReportDeletedDomainEvent : DomainEvent
    {
        public ReportDeletedDomainEvent
        (
            GeneratedReportId generatedReportId,
            ReportRequestId reportRequestId,
            UserId userId
        )
        {
            GeneratedReportId = generatedReportId;
            ReportRequestId = reportRequestId;
            UserId = userId;
        }

        public GeneratedReportId GeneratedReportId { get; }

        public ReportRequestId ReportRequestId { get; }

        public UserId UserId { get; }
    }
}
