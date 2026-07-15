using Domain.Common;
using Domain.Enums;
using Domain.Errors;
using Domain.Events;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// The user's structured ask: which topics to compare, for whom, in what
    /// style and depth, using which criteria, plus any optional constraints.
    ///
    /// A request is mutable only while <see cref="ReportRequestStatus.Draft"/>.
    /// Once <see cref="Submit"/> is called it is frozen — every mutation method
    /// guards on this and throws <see cref="InvalidReportRequestException"/> if
    /// the request has already been submitted. This keeps a submitted request an
    /// honest, stable record of what was actually asked for when generation ran,
    /// even if the user later reopens the wizard to explore different options
    /// (which creates a new request rather than editing history).
    /// </summary>
    public sealed class ReportRequest : SoftDeletableEntity<ReportRequestId>
    {
        public const int MinimumTopicCount = 2;
        public const int MaximumTopicCount = 8;
        public const int MinimumCriterionCount = 2;

        private readonly List<ReportTopic> _topics = [];
        private readonly List<ReportCriterion> _criteria = [];

        private ReportRequest(
            ReportRequestId id,
            UserId userId,
            ReportTitle title,
            TargetAudience targetAudience,
            ReportStyle style,
            TechnicalDepth technicalDepth,
            ReportLength reportLength,
            AiProviderType preferredAiProvider)
            : base(id)
        {
            UserId = userId;
            Title = title;
            TargetAudience = targetAudience;
            Style = style;
            TechnicalDepth = technicalDepth;
            ReportLength = reportLength;
            PreferredAiProvider = preferredAiProvider;
            Status = ReportRequestStatus.Draft;
        }

        /// <summary>EF Core materialization constructor.</summary>
        private ReportRequest()
            : base(default!)
        {
            Title = null!;
            TargetAudience = null!;
        }

        public UserId UserId { get; }

        public ReportTitle Title { get; private set; }

        public TargetAudience TargetAudience { get; private set; }

        public ReportStyle Style { get; private set; }

        public TechnicalDepth TechnicalDepth { get; private set; }

        public ReportLength ReportLength { get; private set; }

        public AiProviderType PreferredAiProvider { get; private set; }

        public ReportRequestStatus Status { get; private set; }

        public SupplementaryNote? IndustryOrDomain { get; private set; }

        public SupplementaryNote? CurrentTechnologyStack { get; private set; }

        public SupplementaryNote? PerformanceRequirements { get; private set; }

        public SupplementaryNote? SecurityRequirements { get; private set; }

        public SupplementaryNote? BudgetConsiderations { get; private set; }

        public SupplementaryNote? MustInclude { get; private set; }

        public SupplementaryNote? MustAvoid { get; private set; }

        public IReadOnlyList<ReportTopic> Topics => _topics.AsReadOnly();

        public IReadOnlyList<ReportCriterion> Criteria => _criteria.AsReadOnly();

        public bool IsDraft => Status == ReportRequestStatus.Draft;

        public bool CanSubmit =>
            IsDraft && _topics.Count >= MinimumTopicCount && _criteria.Count >= MinimumCriterionCount;

        public static ReportRequest Create(
            UserId userId,
            ReportTitle title,
            TargetAudience targetAudience,
            ReportStyle style,
            TechnicalDepth technicalDepth,
            ReportLength reportLength,
            AiProviderType preferredAiProvider = AiProviderType.SystemDefault)
        {
            ArgumentNullException.ThrowIfNull(title);
            ArgumentNullException.ThrowIfNull(targetAudience);

            var request = new ReportRequest(
                ReportRequestId.New(),
                userId,
                title,
                targetAudience,
                style,
                technicalDepth,
                reportLength,
                preferredAiProvider);

            request.RaiseDomainEvent(new ReportRequestCreatedDomainEvent(request.Id, userId));

            return request;
        }

        public void UpdateCoreDetails(
            ReportTitle title,
            TargetAudience targetAudience,
            ReportStyle style,
            TechnicalDepth technicalDepth,
            ReportLength reportLength)
        {
            EnsureEditable();
            ArgumentNullException.ThrowIfNull(title);
            ArgumentNullException.ThrowIfNull(targetAudience);

            Title = title;
            TargetAudience = targetAudience;
            Style = style;
            TechnicalDepth = technicalDepth;
            ReportLength = reportLength;
        }

        public void UpdatePreferredAiProvider(AiProviderType preferredAiProvider)
        {
            EnsureEditable();
            PreferredAiProvider = preferredAiProvider;
        }

        /// <summary>
        /// Replaces every optional constraint field at once, since the wizard's
        /// "optional constraints" step edits them together. Pass <c>null</c> for
        /// any field the user leaves blank.
        /// </summary>
        public void UpdateSupplementaryNotes(
            SupplementaryNote? industryOrDomain,
            SupplementaryNote? currentTechnologyStack,
            SupplementaryNote? performanceRequirements,
            SupplementaryNote? securityRequirements,
            SupplementaryNote? budgetConsiderations,
            SupplementaryNote? mustInclude,
            SupplementaryNote? mustAvoid)
        {
            EnsureEditable();

            IndustryOrDomain = industryOrDomain;
            CurrentTechnologyStack = currentTechnologyStack;
            PerformanceRequirements = performanceRequirements;
            SecurityRequirements = securityRequirements;
            BudgetConsiderations = budgetConsiderations;
            MustInclude = mustInclude;
            MustAvoid = mustAvoid;
        }

        public ReportTopic AddTopic(ReportTopicName name, string? description = null)
        {
            EnsureEditable();
            ArgumentNullException.ThrowIfNull(name);

            if (_topics.Count >= MaximumTopicCount)
            {
                throw new InvalidReportRequestException(
                    ReportDomainError.ReportRequest.MaximumTopicsExceeded(MaximumTopicCount));
            }

            if (_topics.Any(t => t.Name.ComparisonKey == name.ComparisonKey))
            {
                throw new InvalidReportRequestException(
                    ReportDomainError.ReportRequest.DuplicateTopicName(name.Value));
            }

            var topic = ReportTopic.Create(name, description, _topics.Count);
            _topics.Add(topic);
            return topic;
        }

        public void RemoveTopic(ReportTopicId topicId)
        {
            EnsureEditable();

            var topic = _topics.FirstOrDefault(t => t.Id == topicId) ??
                throw new InvalidReportRequestException(ReportDomainError.ReportRequest.TopicNotFound);

            _topics.Remove(topic);
            ResequenceTopics();
        }

        public void ReorderTopics(IReadOnlyList<ReportTopicId> orderedTopicIds)
        {
            EnsureEditable();
            ArgumentNullException.ThrowIfNull(orderedTopicIds);

            if (orderedTopicIds.Count != _topics.Count || orderedTopicIds.Distinct().Count() != _topics.Count)
            {
                throw new InvalidReportRequestException(ReportDomainError.ReportRequest.TopicNotFound);
            }

            var reordered = new List<ReportTopic>(_topics.Count);
            foreach (var topicId in orderedTopicIds)
            {
                var topic = _topics.FirstOrDefault(t => t.Id == topicId) ??
                    throw new InvalidReportRequestException(ReportDomainError.ReportRequest.TopicNotFound);
                reordered.Add(topic);
            }

            _topics.Clear();
            _topics.AddRange(reordered);
            ResequenceTopics();
        }

        public ReportCriterion AddCriterion(
            ReportCriterionName name,
            string? description = null,
            CriterionWeight? weight = null)
        {
            EnsureEditable();
            ArgumentNullException.ThrowIfNull(name);

            if (_criteria.Any(c => c.Name.ComparisonKey == name.ComparisonKey))
            {
                throw new InvalidReportRequestException(
                    ReportDomainError.ReportRequest.DuplicateCriterionName(name.Value));
            }

            var criterion = ReportCriterion.Create(name, description, weight, _criteria.Count);
            _criteria.Add(criterion);
            return criterion;
        }

        public void RemoveCriterion(ReportCriterionId criterionId)
        {
            EnsureEditable();

            var criterion = _criteria.FirstOrDefault(c => c.Id == criterionId) ??
                throw new InvalidReportRequestException(ReportDomainError.ReportRequest.CriterionNotFound);

            _criteria.Remove(criterion);
            ResequenceCriteria();
        }

        public void UpdateCriterionWeight(ReportCriterionId criterionId, CriterionWeight weight)
        {
            EnsureEditable();
            ArgumentNullException.ThrowIfNull(weight);

            var criterion = _criteria.FirstOrDefault(c => c.Id == criterionId) ??
                throw new InvalidReportRequestException(ReportDomainError.ReportRequest.CriterionNotFound);

            criterion.UpdateWeight(weight);
        }

        /// <summary>
        /// Freezes the request so it can be handed off for generation. Once
        /// submitted, the request can no longer be edited; exploring different
        /// inputs means creating a new request.
        /// </summary>
        public void Submit()
        {
            EnsureEditable();

            if (_topics.Count < MinimumTopicCount)
            {
                throw new InvalidReportRequestException(
                    ReportDomainError.ReportRequest.MinimumTopicsRequired(MinimumTopicCount));
            }

            if (_criteria.Count < MinimumCriterionCount)
            {
                throw new InvalidReportRequestException(
                    ReportDomainError.ReportRequest.MinimumCriteriaRequired(MinimumCriterionCount));
            }

            Status = ReportRequestStatus.Submitted;
        }

        private void EnsureEditable()
        {
            if (Status != ReportRequestStatus.Draft)
            {
                throw new InvalidReportRequestException(ReportDomainError.ReportRequest.AlreadySubmitted);
            }
        }

        private void ResequenceTopics()
        {
            for (var index = 0; index < _topics.Count; index++)
            {
                _topics[index].UpdateSortOrder(index);
            }
        }

        private void ResequenceCriteria()
        {
            for (var index = 0; index < _criteria.Count; index++)
            {
                _criteria[index].UpdateSortOrder(index);
            }
        }
    }
}
