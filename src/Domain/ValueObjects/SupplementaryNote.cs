using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// A bounded, optional piece of free-text context supplied on a report
    /// request — industry/domain, current technology stack, performance
    /// requirements, security requirements, budget considerations, must-include
    /// notes, and must-avoid notes all share this same shape and the same
    /// validation rule, so they share this one value object type rather than six
    /// near-identical ones. The length bound exists to protect the prompt/token
    /// budget sent to the AI provider.
    /// </summary>
    public sealed class SupplementaryNote : ValueObject
    {
        public const int MaxLength = 2000;

        public string Value { get; }

        private SupplementaryNote(string value)
        {
            Value = value;
        }

        public static SupplementaryNote Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var trimmed = value.Trim();
            if (trimmed.Length > MaxLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Supplementary note cannot exceed {MaxLength} characters.");
            }

            return new SupplementaryNote(trimmed);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
