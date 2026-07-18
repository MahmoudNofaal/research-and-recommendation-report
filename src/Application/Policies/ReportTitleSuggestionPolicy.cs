using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Policies
{
    /// <summary>
    /// Suggests a starting title for the wizard's first step, derived from
    /// the topics the user has already added and the chosen
    /// <see cref="ReportMode"/> — for example "SignalR vs gRPC vs
    /// WebSockets" for a comparison, or "Understanding CQRS" for a
    /// single-topic research report (see ui-ux-specification.md, "8.5
    /// Reports/Index" and "8.7 Reports/Details", where both examples appear
    /// in context). The result is always just a starting point the user can
    /// freely edit before submitting — never itself a validated
    /// <see cref="ReportTitle"/> — so it is trimmed to fit
    /// <see cref="ReportTitle.MaxLength"/> defensively rather than left to
    /// fail validation the moment it is accepted as-is.
    /// </summary>
    public static class ReportTitleSuggestionPolicy
    {
        private const string ComparisonSeparator = " vs ";
        private const string SingleTopicPrefix = "Understanding ";

        public static string? Suggest(ReportMode reportMode, IReadOnlyList<ReportTopicName> topics)
        {
            ArgumentNullException.ThrowIfNull(topics);

            if (topics.Count == 0)
            {
                return null;
            }

            var suggestion = reportMode switch
            {
                ReportMode.Comparison => string.Join(ComparisonSeparator, topics.Select(topic => topic.Value)),
                ReportMode.SingleTopicResearch => SingleTopicPrefix + topics[0].Value,
                _ => null
            };

            if (suggestion is null)
            {
                return null;
            }

            return suggestion.Length > ReportTitle.MaxLength
                ? suggestion[..ReportTitle.MaxLength]
                : suggestion;
        }
    }
}
