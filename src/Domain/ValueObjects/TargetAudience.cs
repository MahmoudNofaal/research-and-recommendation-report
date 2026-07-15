using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// A free-text description of who the generated report is written for (for
    /// example, "backend engineers evaluating options for a high-traffic API").
    /// Drives prompt composition and report tone.
    /// </summary>
    public sealed class TargetAudience : ValueObject
    {
        public const int MaxLength = 300;

        public string Value { get; }

        private TargetAudience(string value)
        {
            Value = value;
        }

        public static TargetAudience Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var trimmed = value.Trim();
            if (trimmed.Length > MaxLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Target audience description cannot exceed {MaxLength} characters.");
            }

            return new TargetAudience(trimmed);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
