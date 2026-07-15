using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// A single scenario-based recommendation attached to a generated report (for
    /// example, "For a small internal tool, prefer SignalR — strongly
    /// recommended"). Owned exclusively by its parent report and replaced
    /// wholesale on regeneration.
    /// </summary>
    public sealed class ReportRecommendation : Entity<ReportRecommendationId>
    {
        public const int MaxScenarioLength = 300;
        public const int MaxRecommendedOptionLength = 200;
        public const int MaxReasoningLength = 1500;

        private ReportRecommendation
        (
            ReportRecommendationId id,
            string scenario,
            string recommendedOption,
            string reasoning,
            RecommendationStrength strength,
            int sortOrder
        ) : base(id)
        {
            Scenario = scenario;
            RecommendedOption = recommendedOption;
            Reasoning = reasoning;
            Strength = strength;
            SortOrder = sortOrder;
        }

        /// <summary>EF Core materialization constructor.</summary>
        private ReportRecommendation()
            : base(default!)
        {
            Scenario = null!;
            RecommendedOption = null!;
            Reasoning = null!;
        }

        public string Scenario { get; }

        /// <summary>
        /// The option the report recommends for this scenario, expressed in
        /// natural language rather than a hard reference to a topic — AI-authored
        /// recommendations may qualify or combine options in ways a foreign key
        /// to a single <see cref="ValueObjects.ReportTopicId"/> could not capture.
        /// </summary>
        public string RecommendedOption { get; }

        public string Reasoning { get; }

        public RecommendationStrength Strength { get; }

        public int SortOrder { get; private set; }

        internal static ReportRecommendation Create
        (
            string scenario,
            string recommendedOption,
            string reasoning,
            RecommendationStrength strength,
            int sortOrder
        )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(scenario);
            ArgumentException.ThrowIfNullOrWhiteSpace(recommendedOption);
            ArgumentException.ThrowIfNullOrWhiteSpace(reasoning);

            var trimmedScenario = scenario.Trim();
            if (trimmedScenario.Length > MaxScenarioLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(scenario),
                    $"Scenario cannot exceed {MaxScenarioLength} characters."
                );
            }

            var trimmedOption = recommendedOption.Trim();
            if (trimmedOption.Length > MaxRecommendedOptionLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(recommendedOption),
                    $"Recommended option cannot exceed {MaxRecommendedOptionLength} characters."
                );
            }

            var trimmedReasoning = reasoning.Trim();
            if (trimmedReasoning.Length > MaxReasoningLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(reasoning),
                    $"Reasoning cannot exceed {MaxReasoningLength} characters."
                );
            }

            return new ReportRecommendation
            (
                ReportRecommendationId.New(),
                trimmedScenario,
                trimmedOption,
                trimmedReasoning,
                strength,
                sortOrder
            );
        }

        internal void UpdateSortOrder(int sortOrder)
            => SortOrder = sortOrder;
    }
}
