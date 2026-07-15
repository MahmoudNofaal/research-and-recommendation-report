using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The system prompt text of a <c>ReportTemplate</c>: the fixed instructions
    /// that establish the AI provider's role and behavior before any per-request
    /// details are supplied.
    /// </summary>
    public sealed class SystemPromptText : ValueObject
    {
        public const int MaxLength = 4000;

        public string Value { get; }

        private SystemPromptText(string value)
        {
            Value = value;
        }

        public static SystemPromptText Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var trimmed = value.Trim();
            if (trimmed.Length > MaxLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(value),
                    $"System prompt text cannot exceed {MaxLength} characters."
                );
            }

            return new SystemPromptText(trimmed);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
