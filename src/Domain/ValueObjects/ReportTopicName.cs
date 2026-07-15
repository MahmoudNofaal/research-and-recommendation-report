using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The name of a single topic being compared within a report request (for
    /// example, "ASP.NET Core SignalR"). Uniqueness within a request is enforced
    /// by the <c>ReportRequest</c> aggregate, not by this value object.
    /// </summary>
    public sealed class ReportTopicName : ValueObject
    {
        public const int MaxLength = 150;

        public string Value { get; }

        private ReportTopicName(string value)
        {
            Value = value;
        }

        public static ReportTopicName Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var trimmed = value.Trim();
            if (trimmed.Length > MaxLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Topic name cannot exceed {MaxLength} characters.");
            }

            return new ReportTopicName(trimmed);
        }

        /// <summary>
        /// Case-insensitive equality key used by the aggregate to detect duplicate
        /// topics regardless of casing differences a user might type.
        /// </summary>
        public string ComparisonKey => Value.ToUpperInvariant();

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
