using Domain.Common;
using Domain.Errors;
using Domain.Exceptions;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The user-prompt template text of a <c>ReportTemplate</c>: the text sent to
    /// the AI provider with placeholders that get substituted with the specific
    /// request's topics, criteria, and audience. A template missing one of the
    /// required placeholders is not merely malformed input — it is a template
    /// that could never produce a valid generation, so this value object raises
    /// the same domain error catalogue an aggregate would, rather than a bare
    /// <see cref="ArgumentException"/>.
    /// </summary>
    public sealed class UserPromptTemplateText : ValueObject
    {
        public const int MaxLength = 6000;

        public static readonly IReadOnlyList<string> RequiredPlaceholders =
        [
            "{{Topics}}",
            "{{Criteria}}",
            "{{Audience}}"
        ];

        public string Value { get; }

        private UserPromptTemplateText(string value)
        {
            Value = value;
        }

        public static UserPromptTemplateText Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var trimmed = value.Trim();
            if (trimmed.Length > MaxLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(value),
                    $"User prompt template cannot exceed {MaxLength} characters."
                );
            }

            foreach (var placeholder in RequiredPlaceholders)
            {
                if (!trimmed.Contains(placeholder, StringComparison.Ordinal))
                {
                    throw new InvalidReportStateException
                    (
                        ReportDomainError.ReportTemplate.MissingRequiredPlaceholder(placeholder)
                    );
                }
            }

            return new UserPromptTemplateText(trimmed);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
            => Value;
    }
}
