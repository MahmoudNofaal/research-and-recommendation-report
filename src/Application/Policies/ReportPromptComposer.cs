using System.Text;
using Application.Abstractions.AI;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Policies
{
    /// <summary>
    /// The pure, no-I/O implementation of <see cref="IAiPromptComposer"/> —
    /// see that interface's own remarks for why this lives in Application
    /// with no Infrastructure adapter at all. Substitutes the active
    /// <see cref="ReportTemplate"/>'s three required placeholders
    /// (<see cref="UserPromptTemplateText.RequiredPlaceholders"/>) with the
    /// request's actual topics, criteria, and audience, then appends every
    /// other captured input — mode, style, depth, length, and whichever
    /// optional <see cref="SupplementaryNote"/> fields were filled in — as a
    /// trailing "Report configuration" block, so nothing the user specified
    /// is silently dropped just because the template only mandates three
    /// placeholders.
    /// </summary>
    public sealed class ReportPromptComposer : IAiPromptComposer
    {
        public AiGenerationRequest Compose(ReportRequest reportRequest, ReportTemplate template)
        {
            ArgumentNullException.ThrowIfNull(reportRequest);
            ArgumentNullException.ThrowIfNull(template);

            var userPrompt = template.UserPromptTemplate.Value
                .Replace("{{Topics}}", FormatTopics(reportRequest.Topics), StringComparison.Ordinal)
                .Replace("{{Criteria}}", FormatCriteria(reportRequest.ReportMode, reportRequest.Criteria), StringComparison.Ordinal)
                .Replace("{{Audience}}", reportRequest.TargetAudience.Value, StringComparison.Ordinal);

            userPrompt = AppendReportConfiguration(userPrompt, reportRequest);

            return new AiGenerationRequest(template.SystemPrompt.Value, userPrompt);
        }

        private static string FormatTopics(IReadOnlyList<ReportTopic> topics)
        {
            var lines = topics
                .OrderBy(topic => topic.SortOrder)
                .Select(topic => string.IsNullOrWhiteSpace(topic.Description)
                    ? $"- {topic.Name.Value}"
                    : $"- {topic.Name.Value}: {topic.Description}");

            return string.Join(Environment.NewLine, lines);
        }

        private static string FormatCriteria(ReportMode reportMode, IReadOnlyList<ReportCriterion> criteria)
        {
            if (criteria.Count == 0)
            {
                return reportMode == ReportMode.SingleTopicResearch
                    ? "No specific research focus areas were provided — use your judgment about what a reader unfamiliar with the topic would find most useful."
                    : "No specific evaluation criteria were provided — use your judgment about what an engineering or business decision-maker would weigh most.";
            }

            var lines = criteria
                .OrderBy(criterion => criterion.SortOrder)
                .Select(criterion => string.IsNullOrWhiteSpace(criterion.Description)
                    ? $"- {criterion.Name.Value} (importance {criterion.Weight.Value}/10)"
                    : $"- {criterion.Name.Value} (importance {criterion.Weight.Value}/10): {criterion.Description}");

            return string.Join(Environment.NewLine, lines);
        }

        private static string AppendReportConfiguration(string userPrompt, ReportRequest reportRequest)
        {
            var builder = new StringBuilder(userPrompt);

            builder.AppendLine().AppendLine();
            builder.AppendLine("Report configuration:");
            builder.AppendLine($"- Mode: {reportRequest.ReportMode}");
            builder.AppendLine($"- Style: {reportRequest.Style}");
            builder.AppendLine($"- Technical depth: {reportRequest.TechnicalDepth}");
            builder.AppendLine($"- Length: {reportRequest.ReportLength}");

            AppendOptionalNote(builder, "Industry or domain", reportRequest.IndustryOrDomain);
            AppendOptionalNote(builder, "Current technology stack", reportRequest.CurrentTechnologyStack);
            AppendOptionalNote(builder, "Performance requirements", reportRequest.PerformanceRequirements);
            AppendOptionalNote(builder, "Security requirements", reportRequest.SecurityRequirements);
            AppendOptionalNote(builder, "Budget considerations", reportRequest.BudgetConsiderations);
            AppendOptionalNote(builder, "Must include", reportRequest.MustInclude);
            AppendOptionalNote(builder, "Must avoid", reportRequest.MustAvoid);

            return builder.ToString();
        }

        private static void AppendOptionalNote(StringBuilder builder, string label, SupplementaryNote? note)
        {
            if (note is not null)
            {
                builder.AppendLine($"- {label}: {note.Value}");
            }
        }
    }
}
