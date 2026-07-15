using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Events
{
    /// <summary>
    /// Raised whenever a <c>GeneratedReport</c> receives new content — either the
    /// first successful generation for a request, or a subsequent regeneration.
    /// This single event represents "a report now exists / has new content"; a
    /// successful <c>ReportGenerationRun</c> does not raise its own duplicate
    /// event for the same real-world occurrence.
    /// </summary>
    public sealed class ReportGeneratedDomainEvent : DomainEvent
    {
        public ReportGeneratedDomainEvent
        (
            GeneratedReportId generatedReportId,
            ReportRequestId reportRequestId,
            UserId userId,
            int version,
            ReportStatus status
        )
        {
            GeneratedReportId = generatedReportId;
            ReportRequestId = reportRequestId;
            UserId = userId;
            Version = version;
            Status = status;
        }

        public GeneratedReportId GeneratedReportId { get; }

        public ReportRequestId ReportRequestId { get; }

        public UserId UserId { get; }

        public int Version { get; }

        public ReportStatus Status { get; }
    }
}
