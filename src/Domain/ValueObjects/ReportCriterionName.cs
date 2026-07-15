using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The name of a single comparison criterion (for example, "Latency" or
    /// "Ease of implementation"). Shared between <c>ReportCriterion</c> and
    /// <c>SuggestedCriterion</c>, which both name criteria the same way.
    /// </summary>
    public sealed class ReportCriterionName : ValueObject
    {
        public const int MaxLength = 150;

        public string Value { get; }

        private ReportCriterionName(string value)
        {
            Value = value;
        }

        public static ReportCriterionName Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var trimmed = value.Trim();
            if (trimmed.Length > MaxLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Criterion name cannot exceed {MaxLength} characters.");
            }

            return new ReportCriterionName(trimmed);
        }

        /// <summary>
        /// Case-insensitive equality key used by the aggregate to detect duplicate
        /// criteria regardless of casing differences a user might type.
        /// </summary>
        public string ComparisonKey => Value.ToUpperInvariant();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
