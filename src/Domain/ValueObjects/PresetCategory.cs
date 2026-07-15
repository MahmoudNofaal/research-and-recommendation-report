using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The category a <c>CriteriaPreset</c> is grouped under (for example,
    /// "Real-Time Communication" or "Databases"), used to surface relevant
    /// suggestions as the user types their topics.
    /// </summary>
    public sealed class PresetCategory : ValueObject
    {
        public const int MaxLength = 100;

        public string Value { get; }

        private PresetCategory(string value)
        {
            Value = value;
        }

        public static PresetCategory Create(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            var trimmed = value.Trim();
            if (trimmed.Length > MaxLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(value),
                    $"Preset category cannot exceed {MaxLength} characters."
                );
            }

            return new PresetCategory(trimmed);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
            => Value;
    }
}
