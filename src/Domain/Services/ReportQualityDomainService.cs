using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Services
{
    /// <summary>
    /// Scores how structurally complete a <c>GeneratedReport</c>'s content is
    /// against the standard report structure the prompt template asks the AI
    /// provider to follow. The expected sections depend on the originating
    /// request's <see cref="ReportMode"/>: single-topic research reports are
    /// checked for explanation, visual, use-case, and guidance sections, while
    /// comparison reports are checked for topic explanations, comparison tables,
    /// decision matrices, and scenario-based recommendations.
    ///
    /// This is a pure, stateless algorithm reused identically for the first
    /// generation and for every regeneration, which is exactly the kind of
    /// cross-cutting, no-external-I/O logic a Domain Service exists for; it does
    /// not naturally belong to any single entity's own responsibilities.
    ///
    /// Because the prompt composer instructs the AI provider to title its
    /// sections using these exact names, checking for their literal presence is
    /// a legitimate, mechanically verifiable completeness signal — not a fragile
    /// guess at natural-language structure.
    /// </summary>
    public static class ReportQualityDomainService
    {
        /// <summary>
        /// A rough heuristic for how much Markdown a well-covered comparison
        /// topic should generate on average; used only to flag reports that look
        /// thin relative to how many topics they claim to compare.
        /// </summary>
        public const int ExpectedCharactersPerTopic = 300;

        /// <summary>
        /// A focused single-topic research report should usually be deeper than
        /// one row in a comparison report, so it gets its own lower bound.
        /// </summary>
        public const int ExpectedSingleTopicResearchCharacters = 900;

        private sealed record RequiredSection
        (
            string Code,
            string DisplayName,
            QualityWarningSeverity Severity,
            params string[] AcceptablePhrases
        );

        private static readonly IReadOnlyList<RequiredSection> SingleTopicResearchRequiredSections =
        [
            new
            (
                "Quality.MissingExecutiveSummary",
                "Executive Summary",
                QualityWarningSeverity.Blocking,
                "Executive Summary"
            ),
            new
            (
                "Quality.MissingTopicOverview",
                "Topic Overview",
                QualityWarningSeverity.Blocking,
                "Topic Overview",
                "Overview"
            ),
            new
            (
                "Quality.MissingWhyItMatters",
                "Why the Topic Matters",
                QualityWarningSeverity.Warning,
                "Why the Topic Matters",
                "Why It Matters"
            ),
            new
            (
                "Quality.MissingKeyConcepts",
                "Key Concepts and Terminology",
                QualityWarningSeverity.Warning,
                "Key Concepts",
                "Key Concepts and Terminology",
                "Terminology"
            ),
            new
            (
                "Quality.MissingVisualExplanation",
                "Visual Explanation or Diagram",
                QualityWarningSeverity.Info,
                "Visual Explanation",
                "Diagram",
                "Visual"
            ),
            new
            (
                "Quality.MissingUseCases",
                "Practical Use Cases",
                QualityWarningSeverity.Warning,
                "Practical Use Cases",
                "Use Cases"
            ),
            new
            (
                "Quality.MissingBenefitsRisksTradeoffs",
                "Benefits, Risks, and Tradeoffs",
                QualityWarningSeverity.Warning,
                "Benefits, Risks, and Tradeoffs",
                "Benefits and Risks",
                "Risks and Tradeoffs"
            ),
            new
            (
                "Quality.MissingRecommendedApproach",
                "Recommended Approach or Best Practices",
                QualityWarningSeverity.Warning,
                "Recommended Approach",
                "Best Practices",
                "Recommendations"
            ),
            new
            (
                "Quality.MissingImplementationNotes",
                "Implementation Notes or Learning Path",
                QualityWarningSeverity.Info,
                "Implementation Notes",
                "Learning Path",
                "Next Steps"
            ),
        ];

        private static readonly IReadOnlyList<RequiredSection> ComparisonRequiredSections =
        [
            new
            (
                "Quality.MissingExecutiveSummary",
                "Executive Summary",
                QualityWarningSeverity.Blocking,
                "Executive Summary"
            ),
            new
            (
                "Quality.MissingIntroduction",
                "Introduction",
                QualityWarningSeverity.Warning,
                "Introduction"
            ),
            new
            (
                "Quality.MissingTopicExplanations",
                "Explanation of Each Topic",
                QualityWarningSeverity.Blocking,
                "Explanation of Each Topic",
                "Explanation of the Topics",
                "Topic Overview"
            ),
            new
            (
                "Quality.MissingRelationships",
                "Relationships Between Topics",
                QualityWarningSeverity.Warning,
                "Relationships Between Topics",
                "How These Topics Relate"
            ),
            new
            (
                "Quality.MissingComparisonTable",
                "Comparison Table",
                QualityWarningSeverity.Warning,
                "Comparison Table"
            ),
            new
            (
                "Quality.MissingDecisionMatrix",
                "Decision Matrix",
                QualityWarningSeverity.Warning,
                "Decision Matrix"
            ),
            new
            (
                "Quality.MissingScenarioRecommendations",
                "Scenario-Based Recommendations",
                QualityWarningSeverity.Warning,
                "Scenario-Based Recommendations",
                "Scenario Based Recommendations"
            ),
            new
            (
                "Quality.MissingRisksAndTradeoffs",
                "Risks and Tradeoffs",
                QualityWarningSeverity.Warning,
                "Risks and Tradeoffs",
                "Risks & Tradeoffs"
            ),
            new
            (
                "Quality.MissingImplementationNotes",
                "Implementation Notes",
                QualityWarningSeverity.Info,
                "Implementation Notes"
            ),
        ];

        private static readonly string[] ReferencePhrases = ["References", "Cited Sources", "Sources"];

        /// <summary>
        /// Evaluates a comparison report's content and recommendations, returning
        /// both a numeric score and the specific warnings that produced it.
        /// Kept for existing callers; new callers should pass the originating
        /// request's <see cref="ReportMode"/>.
        /// </summary>
        public static (ReportQualityScore Score, IReadOnlyList<QualityWarning> Warnings) Evaluate
        (
            GeneratedReport report, int topicCount
        ) => Evaluate(report, ReportMode.Comparison, topicCount);

        /// <summary>
        /// Evaluates a generated report's content and recommendations, returning
        /// both a numeric score and the specific warnings that produced it.
        /// <paramref name="reportMode"/> and <paramref name="topicCount"/> come
        /// from the originating <c>ReportRequest</c>, which this service does not
        /// itself load.
        /// </summary>
        public static (ReportQualityScore Score, IReadOnlyList<QualityWarning> Warnings) Evaluate
        (
            GeneratedReport report, ReportMode reportMode, int topicCount
        )
        {
            ArgumentNullException.ThrowIfNull(report);
            if (topicCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(topicCount), "Topic count must be positive.");
            }

            var warnings = new List<QualityWarning>();
            var markdown = report.Content.Markdown;
            var score = ReportQualityScore.MaxValue;
            var requiredSections = RequiredSectionsFor(reportMode);

            foreach (var section in requiredSections)
            {
                if (ContainsAny(markdown, section.AcceptablePhrases))
                {
                    continue;
                }

                warnings.Add(QualityWarning.Create
                (
                    section.Code,
                    $"The report does not appear to include a '{section.DisplayName}' section.",
                    section.Severity
                ));

                score -= DeductionFor(section.Severity);
            }

            if (report.Citations.Count > 0 && !ContainsAny(markdown, ReferencePhrases))
            {
                warnings.Add(QualityWarning.Create
                (
                    "Quality.MissingReferences",
                    "The report cites sources but does not appear to include a references section.",
                    QualityWarningSeverity.Warning
                ));

                score -= DeductionFor(QualityWarningSeverity.Warning);
            }

            if (report.Recommendations.Count == 0)
            {
                warnings.Add(QualityWarning.Create
                (
                    "Quality.NoRecommendations",
                    "The report does not include any scenario-based recommendations.",
                    QualityWarningSeverity.Blocking
                ));

                score -= DeductionFor(QualityWarningSeverity.Blocking);
            }
            else if (report.Recommendations.Count < Math.Min(topicCount, 2))
            {
                warnings.Add(QualityWarning.Create
                (
                    "Quality.FewRecommendations",
                    FewRecommendationsMessageFor(reportMode),
                    QualityWarningSeverity.Info
                ));

                score -= DeductionFor(QualityWarningSeverity.Info);
            }

            var expectedMinimumLength = ExpectedMinimumLengthFor(reportMode, topicCount);
            if (markdown.Length < expectedMinimumLength)
            {
                warnings.Add(QualityWarning.Create
                (
                    "Quality.ThinContent",
                    ThinContentMessageFor(reportMode),
                    QualityWarningSeverity.Info
                ));

                score -= DeductionFor(QualityWarningSeverity.Info);
            }

            var clampedScore = Math.Clamp(score, ReportQualityScore.MinValue, ReportQualityScore.MaxValue);

            return (ReportQualityScore.Create(clampedScore), warnings.AsReadOnly());
        }

        private static bool ContainsAny(string markdown, IReadOnlyList<string> phrases)
            => phrases.Any(phrase => markdown.Contains(phrase, StringComparison.OrdinalIgnoreCase));

        private static IReadOnlyList<RequiredSection> RequiredSectionsFor(ReportMode reportMode)
            => reportMode switch
            {
                ReportMode.SingleTopicResearch => SingleTopicResearchRequiredSections,
                ReportMode.Comparison => ComparisonRequiredSections,
                _ => throw new ArgumentOutOfRangeException(nameof(reportMode), reportMode, "Unsupported report mode.")
            };

        private static int ExpectedMinimumLengthFor(ReportMode reportMode, int topicCount)
            => reportMode switch
            {
                ReportMode.SingleTopicResearch => ExpectedSingleTopicResearchCharacters,
                ReportMode.Comparison => ExpectedCharactersPerTopic * topicCount,
                _ => throw new ArgumentOutOfRangeException(nameof(reportMode), reportMode, "Unsupported report mode.")
            };

        private static string FewRecommendationsMessageFor(ReportMode reportMode)
            => reportMode switch
            {
                ReportMode.SingleTopicResearch =>
                    "The report includes fewer recommendations or next steps than expected for a focused research report.",
                ReportMode.Comparison =>
                    "The report includes fewer scenario-based recommendations than expected for the number of topics compared.",
                _ => throw new ArgumentOutOfRangeException(nameof(reportMode), reportMode, "Unsupported report mode.")
            };

        private static string ThinContentMessageFor(ReportMode reportMode)
            => reportMode switch
            {
                ReportMode.SingleTopicResearch =>
                    "The report's content seems thin for a focused single-topic research report.",
                ReportMode.Comparison =>
                    "The report's content seems thin relative to the number of topics being compared.",
                _ => throw new ArgumentOutOfRangeException(nameof(reportMode), reportMode, "Unsupported report mode.")
            };

        private static int DeductionFor(QualityWarningSeverity severity) => severity switch
        {
            QualityWarningSeverity.Blocking => 25,
            QualityWarningSeverity.Warning => 10,
            QualityWarningSeverity.Info => 5,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, "Unsupported quality warning severity.")
        };
    }
}
