using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Services
{
    /// <summary>
    /// Scores how structurally complete a <c>GeneratedReport</c>'s content is
    /// against the standard report structure the prompt template asks the AI
    /// provider to follow (executive summary, introduction, topic explanations,
    /// relationships, comparison table, decision matrix, scenario-based
    /// recommendations, risks/tradeoffs, implementation notes, and — when
    /// citations exist — references).
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
        /// A rough heuristic for how much Markdown a well-covered topic should
        /// generate on average; used only to flag reports that look thin relative
        /// to how many topics they claim to compare.
        /// </summary>
        public const int ExpectedCharactersPerTopic = 300;

        private sealed record RequiredSection(string Code, string DisplayName, QualityWarningSeverity Severity, params string[] AcceptablePhrases);

        private static readonly IReadOnlyList<RequiredSection> RequiredSections =
        [
            new("Quality.MissingExecutiveSummary", "Executive Summary", QualityWarningSeverity.Blocking, "Executive Summary"),
            new("Quality.MissingIntroduction", "Introduction", QualityWarningSeverity.Warning, "Introduction"),
            new("Quality.MissingTopicExplanations", "Explanation of Each Topic", QualityWarningSeverity.Blocking, "Explanation of Each Topic", "Explanation of the Topics", "Topic Overview"),
            new("Quality.MissingRelationships", "Relationships Between Topics", QualityWarningSeverity.Warning, "Relationships Between Topics", "How These Topics Relate"),
            new("Quality.MissingComparisonTable", "Comparison Table", QualityWarningSeverity.Warning, "Comparison Table"),
            new("Quality.MissingDecisionMatrix", "Decision Matrix", QualityWarningSeverity.Warning, "Decision Matrix"),
            new("Quality.MissingScenarioRecommendations", "Scenario-Based Recommendations", QualityWarningSeverity.Warning, "Scenario-Based Recommendations", "Scenario Based Recommendations"),
            new("Quality.MissingRisksAndTradeoffs", "Risks and Tradeoffs", QualityWarningSeverity.Warning, "Risks and Tradeoffs", "Risks & Tradeoffs"),
            new("Quality.MissingImplementationNotes", "Implementation Notes", QualityWarningSeverity.Info, "Implementation Notes"),
        ];

        private static readonly string[] ReferencePhrases = ["References", "Cited Sources", "Sources"];

        /// <summary>
        /// Evaluates a generated report's content and recommendations, returning
        /// both a numeric score and the specific warnings that produced it.
        /// <paramref name="topicCount"/> comes from the originating
        /// <c>ReportRequest</c>, which this service does not itself load.
        /// </summary>
        public static (ReportQualityScore Score, IReadOnlyList<QualityWarning> Warnings) Evaluate(
            GeneratedReport report, int topicCount)
        {
            ArgumentNullException.ThrowIfNull(report);
            if (topicCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(topicCount), "Topic count must be positive.");
            }

            var warnings = new List<QualityWarning>();
            var markdown = report.Content.Markdown;
            var score = ReportQualityScore.MaxValue;

            foreach (var section in RequiredSections)
            {
                if (ContainsAny(markdown, section.AcceptablePhrases))
                {
                    continue;
                }

                warnings.Add(QualityWarning.Create(
                    section.Code,
                    $"The report does not appear to include a '{section.DisplayName}' section.",
                    section.Severity));
                score -= DeductionFor(section.Severity);
            }

            if (report.Citations.Count > 0 && !ContainsAny(markdown, ReferencePhrases))
            {
                warnings.Add(QualityWarning.Create(
                    "Quality.MissingReferences",
                    "The report cites sources but does not appear to include a references section.",
                    QualityWarningSeverity.Warning));
                score -= DeductionFor(QualityWarningSeverity.Warning);
            }

            if (report.Recommendations.Count == 0)
            {
                warnings.Add(QualityWarning.Create(
                    "Quality.NoRecommendations",
                    "The report does not include any scenario-based recommendations.",
                    QualityWarningSeverity.Blocking));
                score -= DeductionFor(QualityWarningSeverity.Blocking);
            }
            else if (report.Recommendations.Count < Math.Min(topicCount, 2))
            {
                warnings.Add(QualityWarning.Create(
                    "Quality.FewRecommendations",
                    "The report includes fewer scenario-based recommendations than expected for the number of topics compared.",
                    QualityWarningSeverity.Info));
                score -= DeductionFor(QualityWarningSeverity.Info);
            }

            var expectedMinimumLength = ExpectedCharactersPerTopic * topicCount;
            if (markdown.Length < expectedMinimumLength)
            {
                warnings.Add(QualityWarning.Create(
                    "Quality.ThinContent",
                    "The report's content seems thin relative to the number of topics being compared.",
                    QualityWarningSeverity.Info));
                score -= DeductionFor(QualityWarningSeverity.Info);
            }

            var clampedScore = Math.Clamp(score, ReportQualityScore.MinValue, ReportQualityScore.MaxValue);
            return (ReportQualityScore.Create(clampedScore), warnings.AsReadOnly());
        }

        private static bool ContainsAny(string markdown, IReadOnlyList<string> phrases) =>
            phrases.Any(phrase => markdown.Contains(phrase, StringComparison.OrdinalIgnoreCase));

        private static int DeductionFor(QualityWarningSeverity severity) => severity switch
        {
            QualityWarningSeverity.Blocking => 25,
            QualityWarningSeverity.Warning => 10,
            QualityWarningSeverity.Info => 5,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, "Unsupported quality warning severity.")
        };
    }
}
