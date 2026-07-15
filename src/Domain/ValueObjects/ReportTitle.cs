using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The title of a report request / generated report. Required and bounded so
    /// it remains usable in navigation, history lists, and export file names.
    /// </summary>
    public sealed class ReportTitle : ValueObject
    {
        public const int MaxLength = 200;

        public string Value { get; }

        private ReportTitle(string value)
        {
            Value = value;
        }

        public static ReportTitle Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var trimmed = value.Trim();
            if (trimmed.Length > MaxLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(value),
                    $"Report title cannot exceed {MaxLength} characters."
                );
            }

            return new ReportTitle(trimmed);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
            => Value;
    }
}
