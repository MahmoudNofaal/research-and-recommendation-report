using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Abstractions.AI
{
    /// <summary>
    /// The pieces of a generated report as parsed out of one provider's raw
    /// Markdown response, before any of them have been validated or trimmed
    /// by the Domain constructors that will ultimately accept them
    /// (<see cref="ReportContent.Create"/>,
    /// <see cref="Domain.Entities.GeneratedReport.AddRecommendation"/>,
    /// <see cref="Domain.Entities.GeneratedReport.AddCitation"/>).
    /// Deliberately thin — this is a parsing result, not a second copy of the
    /// Domain model — so <see cref="Content"/> reuses the real
    /// <see cref="ReportContent"/> value object directly.
    /// </summary>
    public sealed record ParsedAiReport
    (
        ReportContent Content,
        IReadOnlyList<ParsedRecommendation> Recommendations,
        IReadOnlyList<ParsedCitation> Citations
    );

    /// <summary>
    /// One scenario-based recommendation as parsed from the raw response,
    /// matching <c>GeneratedReport.AddRecommendation</c>'s parameters
    /// exactly.
    /// </summary>
    public sealed record ParsedRecommendation
    (
        string Scenario,
        string RecommendedOption,
        string Reasoning,
        RecommendationStrength Strength
    );

    /// <summary>
    /// One cited source as parsed from the raw response, matching
    /// <c>GeneratedReport.AddCitation</c>'s parameters exactly.
    /// </summary>
    public sealed record ParsedCitation
    (
        string Title,
        SourceUrl Url,
        string SourceName,
        DateTime? PublishedAtUtc,
        DateTime AccessedAtUtc,
        string? Notes
    );

    /// <summary>
    /// Splits a provider's raw Markdown response into the structured pieces a
    /// <c>GeneratedReport</c> is built from. Parsing is intentionally
    /// forgiving rather than throwing on anything unexpected: content that
    /// turns out too thin or missing recommendations is caught downstream by
    /// Domain's own <see cref="Domain.Entities.GeneratedReport.CompleteGeneration"/>
    /// guard and <see cref="Domain.Services.ReportQualityDomainService"/> —
    /// this parser's job is only to locate the sections the prompt template
    /// asked the provider to produce, not to judge whether they are good
    /// enough.
    /// </summary>
    public interface IAiResponseParser
    {
        ParsedAiReport Parse(string rawResponse, ReportMode reportMode);
    }
}
