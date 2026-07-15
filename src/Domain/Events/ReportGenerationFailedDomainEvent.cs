using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Events
{
    /// <summary>
    /// Raised when a <c>ReportGenerationRun</c> transitions to
    /// <see cref="Enums.GenerationStatus.Failed"/>, carrying the recorded
    /// <see cref="ErrorDetail"/> so subscribers (logging, alerting, provider
    /// health tracking) do not need to reload the run to learn why it failed.
    /// </summary>
    public sealed class ReportGenerationFailedDomainEvent : DomainEvent
    {
        public ReportGenerationFailedDomainEvent
        (
            ReportGenerationRunId reportGenerationRunId,
            ReportRequestId reportRequestId,
            UserId userId,
            ErrorDetail errorDetail
        )
        {
            ReportGenerationRunId = reportGenerationRunId;
            ReportRequestId = reportRequestId;
            UserId = userId;
            ErrorDetail = errorDetail;
        }

        public ReportGenerationRunId ReportGenerationRunId { get; }

        public ReportRequestId ReportRequestId { get; }

        public UserId UserId { get; }

        public ErrorDetail ErrorDetail { get; }
    }
}
