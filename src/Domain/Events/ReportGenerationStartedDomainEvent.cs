using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Events
{
    /// <summary>
    /// Raised when a <c>ReportGenerationRun</c> transitions from <see cref="GenerationStatus.Pending"/>
    /// to <see cref="GenerationStatus.InProgress"/> and the AI provider call is about to begin.
    /// </summary>
    public sealed class ReportGenerationStartedDomainEvent : DomainEvent
    {
        public ReportGenerationStartedDomainEvent(
            ReportGenerationRunId reportGenerationRunId,
            ReportRequestId reportRequestId,
            UserId userId,
            AiProviderType aiProvider)
        {
            ReportGenerationRunId = reportGenerationRunId;
            ReportRequestId = reportRequestId;
            UserId = userId;
            AiProvider = aiProvider;
        }

        public ReportGenerationRunId ReportGenerationRunId { get; }

        public ReportRequestId ReportRequestId { get; }

        public UserId UserId { get; }

        public AiProviderType AiProvider { get; }
    }
}
