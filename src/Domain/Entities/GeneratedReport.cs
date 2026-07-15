using Domain.Common;
using Domain.Enums;
using Domain.Errors;
using Domain.Events;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// The current canonical generated report for a submitted <c>ReportRequest</c>:
    /// Markdown content, citations, scenario-based recommendations, and the
    /// quality assessment that decided whether it is fit to present.
    ///
    /// This aggregate intentionally does not extend <see cref="AuditableEntity{TId}"/>
    /// or <see cref="SoftDeletableEntity{TId}"/>. Its "created" moment
    /// (<see cref="GeneratedAtUtc"/>) is a meaningful business fact tied directly
    /// to the generation event, not generic row-insertion bookkeeping, and its
    /// soft-delete fields are therefore hand-rolled here to keep that distinction
    /// honest rather than borrowing a base type whose naming would blur it.
    ///
    /// Construction is a two-phase process mirroring how a report actually comes
    /// together: <see cref="Create"/> (or <see cref="Regenerate"/>) establishes
    /// the content and metadata, then <see cref="AddRecommendation"/> and
    /// <see cref="AddCitation"/> populate the AI-parsed detail, and finally
    /// <see cref="CompleteGeneration"/> validates the whole (at least one
    /// recommendation must exist), records the quality assessment, computes
    /// <see cref="Status"/>, and raises <see cref="ReportGeneratedDomainEvent"/>.
    /// Application never persists the aggregate mid-sequence, so the transient
    /// "no recommendations yet" state is never observable outside this build-up.
    /// </summary>
    public sealed class GeneratedReport : AggregateRoot<GeneratedReportId>
    {
        public const int MaxModelNameLength = 100;

        private readonly List<ReportCitation> _citations = [];
        private readonly List<ReportRecommendation> _recommendations = [];

        private GeneratedReport
        (
            GeneratedReportId id,
            ReportRequestId reportRequestId,
            UserId userId,
            ReportTitle title,
            ReportContent content,
            AiProviderType aiProvider,
            string modelName,
            PromptVersion promptVersion,
            DateTime generatedAtUtc
        ) : base(id)
        {
            ReportRequestId = reportRequestId;
            UserId = userId;
            Title = title;
            Content = content;
            AiProvider = aiProvider;
            ModelName = modelName;
            PromptVersion = promptVersion;
            GeneratedAtUtc = generatedAtUtc;
            UpdatedAtUtc = generatedAtUtc;
            Version = 1;
            Status = ReportStatus.Draft;
            QualityScore = ReportQualityScore.Create(ReportQualityScore.MinValue);
            QualityWarnings = [];
        }

        /// <summary>EF Core materialization constructor.</summary>
        private GeneratedReport()
            : base(default!)
        {
            Title = null!;
            Content = null!;
            ModelName = null!;
            PromptVersion = null!;
            QualityScore = null!;
            QualityWarnings = [];
        }

        public ReportRequestId ReportRequestId { get; }

        public UserId UserId { get; }

        public ReportTitle Title { get; private set; }

        public ReportContent Content { get; private set; }

        public AiProviderType AiProvider { get; private set; }

        public string ModelName { get; private set; }

        public PromptVersion PromptVersion { get; private set; }

        public ReportStatus Status { get; private set; }

        public ReportQualityScore QualityScore { get; private set; }

        public IReadOnlyCollection<QualityWarning> QualityWarnings { get; private set; }

        public int Version { get; private set; }

        /// <summary>When this version's content was produced — a business fact, not row-insertion bookkeeping.</summary>
        public DateTime GeneratedAtUtc { get; private set; }

        /// <summary>Generic "last modified" bookkeeping, stamped by Infrastructure on any change.</summary>
        public DateTime UpdatedAtUtc { get; internal set; }

        public DateTime? DeletedAtUtc { get; private set; }

        public bool IsDeleted => DeletedAtUtc is not null;

        public IReadOnlyList<ReportCitation> Citations
            => _citations.AsReadOnly();

        public IReadOnlyList<ReportRecommendation> Recommendations
            => _recommendations.AsReadOnly();

        public static GeneratedReport Create
        (
            GeneratedReportId id,
            ReportRequestId reportRequestId,
            UserId userId,
            ReportTitle title,
            ReportContent content,
            AiProviderType aiProvider,
            string modelName,
            PromptVersion promptVersion,
            DateTime generatedAtUtc
        )
        {
            ArgumentNullException.ThrowIfNull(title);
            EnsureSubstantiveContent(content);
            EnsureConcreteProvider(aiProvider);

            var trimmedModelName = NormalizeModelName(modelName);

            return new GeneratedReport
            (
                id,
                reportRequestId,
                userId,
                title,
                content,
                aiProvider,
                trimmedModelName,
                promptVersion,
                generatedAtUtc
            );
        }

        public ReportRecommendation AddRecommendation
        (
            string scenario,
            string recommendedOption,
            string reasoning,
            RecommendationStrength strength
       )
        {
            EnsureNotDeleted();

            var recommendation = ReportRecommendation.Create
            (
                scenario,
                recommendedOption,
                reasoning,
                strength,
                _recommendations.Count
            );

            _recommendations.Add(recommendation);

            return recommendation;
        }

        public ReportCitation AddCitation
        (
            string title,
            SourceUrl url,
            string sourceName,
            DateTime? publishedAtUtc,
            DateTime accessedAtUtc,
            string? notes = null
        )
        {
            EnsureNotDeleted();

            var citation = ReportCitation.Create
            (
                title,
                url,
                sourceName,
                publishedAtUtc,
                accessedAtUtc,
                notes,
                _citations.Count
            );

            _citations.Add(citation);

            return citation;
        }

        /// <summary>
        /// Finalizes the current build-up: validates that at least one
        /// recommendation was produced, records the quality assessment, computes
        /// <see cref="Status"/>, and raises <see cref="ReportGeneratedDomainEvent"/>.
        /// Called once after generation and once after every regeneration.
        /// </summary>
        public void CompleteGeneration
        (
            ReportQualityScore qualityScore,
            IReadOnlyCollection<QualityWarning> qualityWarnings
        )
        {
            EnsureNotDeleted();
            ArgumentNullException.ThrowIfNull(qualityScore);
            ArgumentNullException.ThrowIfNull(qualityWarnings);

            if (_recommendations.Count == 0)
            {
                throw new InvalidReportStateException(ReportDomainError.GeneratedReport.AtLeastOneRecommendationRequired);
            }

            QualityScore = qualityScore;
            QualityWarnings = [.. qualityWarnings];
            Status = ComputeStatus(qualityScore, QualityWarnings);

            RaiseDomainEvent(new ReportGeneratedDomainEvent(Id, ReportRequestId, UserId, Version, Status));
        }

        /// <summary>
        /// Replaces this report's content wholesale with a new AI generation
        /// attempt, incrementing <see cref="Version"/> and clearing the previous
        /// recommendations and citations. The caller must repopulate them via
        /// <see cref="AddRecommendation"/>/<see cref="AddCitation"/> and finish
        /// with <see cref="CompleteGeneration"/>, exactly as during the first
        /// generation.
        /// </summary>
        public void Regenerate
        (
            ReportTitle title,
            ReportContent content,
            AiProviderType aiProvider,
            string modelName,
            PromptVersion promptVersion,
            DateTime generatedAtUtc
        )
        {
            EnsureNotDeleted();
            ArgumentNullException.ThrowIfNull(title);
            EnsureSubstantiveContent(content);
            EnsureConcreteProvider(aiProvider);

            Title = title;
            Content = content;
            AiProvider = aiProvider;
            ModelName = NormalizeModelName(modelName);
            PromptVersion = promptVersion;
            GeneratedAtUtc = generatedAtUtc;
            UpdatedAtUtc = generatedAtUtc;
            Version += 1;
            Status = ReportStatus.Draft;
            QualityScore = ReportQualityScore.Create(ReportQualityScore.MinValue);
            QualityWarnings = [];

            _recommendations.Clear();
            _citations.Clear();
        }

        public void Delete(DateTime deletedAtUtc)
        {
            if (IsDeleted)
            {
                throw new InvalidReportStateException(ReportDomainError.GeneratedReport.AlreadyDeleted);
            }

            DeletedAtUtc = deletedAtUtc;
            UpdatedAtUtc = deletedAtUtc;

            RaiseDomainEvent(new ReportDeletedDomainEvent(Id, ReportRequestId, UserId));
        }

        private void EnsureNotDeleted()
        {
            if (IsDeleted)
            {
                throw new InvalidReportStateException(ReportDomainError.GeneratedReport.CannotModifyDeletedReport);
            }
        }

        private static void EnsureSubstantiveContent(ReportContent content)
        {
            ArgumentNullException.ThrowIfNull(content);

            if (!content.IsSubstantive)
            {
                throw new InvalidReportStateException
                (
                    ReportDomainError.GeneratedReport.ContentTooShort(ReportContent.MinimumSubstantiveMarkdownLength)
                );
            }
        }

        private static void EnsureConcreteProvider(AiProviderType aiProvider)
        {
            if (!aiProvider.IsConcreteProvider())
            {
                throw new InvalidReportStateException(ReportDomainError.ReportGenerationRun.ConcreteProviderRequired);
            }
        }

        private static string NormalizeModelName(string modelName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(modelName);

            var trimmed = modelName.Trim();
            if (trimmed.Length > MaxModelNameLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(modelName),
                    $"Model name cannot exceed {MaxModelNameLength} characters."
                );
            }

            return trimmed;
        }

        private static ReportStatus ComputeStatus(ReportQualityScore score, IReadOnlyCollection<QualityWarning> warnings)
            => warnings.Any(w => w.Severity == QualityWarningSeverity.Blocking) || !score.MeetsReadyThreshold
                ? ReportStatus.Draft
                : ReportStatus.Ready;
    }
}
