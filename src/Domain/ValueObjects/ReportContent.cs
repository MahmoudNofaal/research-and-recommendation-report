using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The canonical Markdown body and executive summary of a generated report.
    /// Every other export format is derived from this content by Infrastructure.
    ///
    /// This value object only guards basic shape (non-blank, summary length
    /// bound). Whether the Markdown is <em>substantive enough</em> to accept —
    /// the domain-level guard against empty or truncated AI output — is exposed
    /// via <see cref="IsSubstantive"/> and is enforced by the <c>GeneratedReport</c>
    /// aggregate, which decides what happens when content falls short; that is a
    /// business rule outcome (an expected, retryable failure), not a constructor
    /// contract violation.
    /// </summary>
    public sealed class ReportContent : ValueObject
    {
        public const int MinimumSubstantiveMarkdownLength = 200;
        public const int MaximumSummaryLength = 600;

        public string Markdown { get; }

        public string Summary { get; }

        private ReportContent(string markdown, string summary)
        {
            Markdown = markdown;
            Summary = summary;
        }

        public static ReportContent Create(string markdown, string summary)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(markdown);
            ArgumentException.ThrowIfNullOrWhiteSpace(summary);

            var trimmedSummary = summary.Trim();
            if (trimmedSummary.Length > MaximumSummaryLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(summary),
                    $"Summary cannot exceed {MaximumSummaryLength} characters.");
            }

            return new ReportContent(markdown.Trim(), trimmedSummary);
        }

        /// <summary>
        /// True when the Markdown body is long enough to be considered a
        /// substantive report rather than empty or truncated AI output.
        /// </summary>
        public bool IsSubstantive => Markdown.Length >= MinimumSubstantiveMarkdownLength;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Markdown;
            yield return Summary;
        }
    }
}
