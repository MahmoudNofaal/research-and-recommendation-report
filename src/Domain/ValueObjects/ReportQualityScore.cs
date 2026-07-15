using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// A 0–100 score produced by <see cref="Domain.Services.ReportQualityDomainService"/>
    /// describing how structurally complete and substantive a generated report's
    /// content is.
    /// </summary>
    public sealed class ReportQualityScore : ValueObject
    {
        public const int MinValue = 0;
        public const int MaxValue = 100;

        /// <summary>
        /// The minimum score a report must reach, with no blocking warnings, to be
        /// considered <see cref="Domain.Enums.ReportStatus.Ready"/>.
        /// </summary>
        public const int ReadyThreshold = 70;

        public int Value { get; }

        private ReportQualityScore(int value)
        {
            Value = value;
        }

        public static ReportQualityScore Create(int value)
        {
            if (value < MinValue || value > MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Report quality score must be between {MinValue} and {MaxValue}.");
            }

            return new ReportQualityScore(value);
        }

        public bool MeetsReadyThreshold => Value >= ReadyThreshold;

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value.ToString();
    }
}
