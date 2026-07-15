using Domain.Common;
using Domain.Enums;
using Domain.Errors;
using Domain.Events;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// A single AI generation attempt: one call to a provider, whether it
    /// succeeds or fails. Modeled as its own aggregate root — separate from
    /// <c>GeneratedReport</c> — because an attempt can fail before any generated
    /// content exists at all, and because the audit trail of every attempt
    /// (including every failure and retry) is a first-class business record in
    /// its own right, not a detail of the report it may or may not have produced.
    ///
    /// <see cref="Domain.ValueObjects.GeneratedReportId"/> is always known up
    /// front: it is a client-generated identifier, so the Application layer
    /// allocates it before the very first attempt at generating a report (not
    /// only on regeneration), and this run simply carries a reference to it.
    ///
    /// Status follows a strict, one-directional state machine:
    /// <see cref="GenerationStatus.Pending"/> → <see cref="GenerationStatus.InProgress"/>
    /// → terminal (<see cref="GenerationStatus.Succeeded"/> or
    /// <see cref="GenerationStatus.Failed"/>). Terminal states cannot be re-entered.
    /// </summary>
    public sealed class ReportGenerationRun : AggregateRoot<ReportGenerationRunId>
    {
        public const int MaxModelNameLength = 100;
        public const int MaxPromptTextLength = 10_000;
        public const int MaxRawResponseLength = 20_000;

        private ReportGenerationRun(
            ReportGenerationRunId id,
            ReportRequestId reportRequestId,
            GeneratedReportId generatedReportId,
            UserId userId,
            AiProviderType aiProvider,
            string modelName,
            PromptVersion promptVersion,
            string promptText)
            : base(id)
        {
            ReportRequestId = reportRequestId;
            GeneratedReportId = generatedReportId;
            UserId = userId;
            AiProvider = aiProvider;
            ModelName = modelName;
            PromptVersion = promptVersion;
            PromptText = promptText;
            Status = GenerationStatus.Pending;
        }

        /// <summary>EF Core materialization constructor.</summary>
        private ReportGenerationRun()
            : base(default!)
        {
            ModelName = null!;
            PromptVersion = null!;
            PromptText = null!;
        }

        public ReportRequestId ReportRequestId { get; }

        public GeneratedReportId GeneratedReportId { get; }

        public UserId UserId { get; }

        public AiProviderType AiProvider { get; }

        public string ModelName { get; }

        public PromptVersion PromptVersion { get; }

        public string PromptText { get; }

        public string? RawResponse { get; private set; }

        public GenerationStatus Status { get; private set; }

        public GenerationTiming? Timing { get; private set; }

        public TokenUsage? TokenUsage { get; private set; }

        public ErrorDetail? ErrorDetail { get; private set; }

        public static ReportGenerationRun Create(
            ReportRequestId reportRequestId,
            GeneratedReportId generatedReportId,
            UserId userId,
            AiProviderType aiProvider,
            string modelName,
            PromptVersion promptVersion,
            string promptText)
        {
            if (!aiProvider.IsConcreteProvider())
            {
                throw new InvalidReportStateException(ReportDomainError.ReportGenerationRun.ConcreteProviderRequired);
            }

            ArgumentException.ThrowIfNullOrWhiteSpace(modelName);
            ArgumentNullException.ThrowIfNull(promptVersion);
            ArgumentException.ThrowIfNullOrWhiteSpace(promptText);

            var trimmedModelName = modelName.Trim();
            if (trimmedModelName.Length > MaxModelNameLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(modelName),
                    $"Model name cannot exceed {MaxModelNameLength} characters.");
            }

            var trimmedPromptText = promptText.Trim();
            if (trimmedPromptText.Length > MaxPromptTextLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(promptText),
                    $"Prompt text cannot exceed {MaxPromptTextLength} characters.");
            }

            return new ReportGenerationRun(
                ReportGenerationRunId.New(),
                reportRequestId,
                generatedReportId,
                userId,
                aiProvider,
                trimmedModelName,
                promptVersion,
                trimmedPromptText);
        }

        /// <summary>
        /// Marks the run as actively calling the AI provider. This is the
        /// point at which the run becomes durable evidence that an attempt was
        /// made, independent of whether the subsequent HTTP call ever completes.
        /// </summary>
        public void Begin(DateTime startedAtUtc)
        {
            EnsureStatus(GenerationStatus.Pending);

            Timing = GenerationTiming.Start(startedAtUtc);
            Status = GenerationStatus.InProgress;

            RaiseDomainEvent(new ReportGenerationStartedDomainEvent(Id, ReportRequestId, UserId, AiProvider));
        }

        public void Succeed(string rawResponse, TokenUsage tokenUsage, DateTime completedAtUtc)
        {
            EnsureStatus(GenerationStatus.InProgress);
            ArgumentException.ThrowIfNullOrWhiteSpace(rawResponse);
            ArgumentNullException.ThrowIfNull(tokenUsage);

            var trimmedResponse = rawResponse.Trim();
            if (trimmedResponse.Length > MaxRawResponseLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rawResponse),
                    $"Raw response cannot exceed {MaxRawResponseLength} characters.");
            }

            RawResponse = trimmedResponse;
            TokenUsage = tokenUsage;
            Timing = Timing!.Complete(completedAtUtc);
            Status = GenerationStatus.Succeeded;
        }

        /// <summary>
        /// Records a failure. Allowed from either <see cref="GenerationStatus.Pending"/>
        /// (for example, the provider factory could not resolve a healthy
        /// provider before the call was ever made) or
        /// <see cref="GenerationStatus.InProgress"/> (the call itself failed or
        /// returned invalid content).
        /// </summary>
        public void Fail(ErrorDetail errorDetail, DateTime completedAtUtc)
        {
            if (Status is not (GenerationStatus.Pending or GenerationStatus.InProgress))
            {
                throw new InvalidReportStateException(
                    ReportDomainError.ReportGenerationRun.InvalidStatusTransition(Status.ToString(), GenerationStatus.Failed.ToString()));
            }

            ArgumentNullException.ThrowIfNull(errorDetail);

            ErrorDetail = errorDetail;
            Timing = Timing?.Complete(completedAtUtc) ?? GenerationTiming.Start(completedAtUtc).Complete(completedAtUtc);
            Status = GenerationStatus.Failed;

            RaiseDomainEvent(new ReportGenerationFailedDomainEvent(Id, ReportRequestId, UserId, errorDetail));
        }

        private void EnsureStatus(GenerationStatus required)
        {
            if (Status != required)
            {
                throw new InvalidReportStateException(
                    ReportDomainError.ReportGenerationRun.InvalidStatusTransition(Status.ToString(), required.ToString()));
            }
        }
    }
}
