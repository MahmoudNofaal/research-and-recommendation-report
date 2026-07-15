using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// A stable version token identifying which prompt composition strategy
    /// produced a given generation run (for example, "v1" or
    /// "2026-06-groq-standard"), so historical runs remain interpretable even
    /// after the prompt composer evolves.
    /// </summary>
    public sealed class PromptVersion : ValueObject
    {
        public const int MaxLength = 50;

        public string Value { get; }

        private PromptVersion(string value)
        {
            Value = value;
        }

        public static PromptVersion Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var trimmed = value.Trim();
            if (trimmed.Length > MaxLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Prompt version token cannot exceed {MaxLength} characters.");
            }

            return new PromptVersion(trimmed);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
