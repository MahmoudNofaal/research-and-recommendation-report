using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The relative importance of a comparison criterion within the decision
    /// matrix, on a fixed 1–10 scale so weights stay comparable across requests.
    /// </summary>
    public sealed class CriterionWeight : ValueObject
    {
        public const int MinValue = 1;
        public const int MaxValue = 10;
        public const int DefaultValue = 5;

        public int Value { get; }

        private CriterionWeight(int value)
        {
            Value = value;
        }

        public static CriterionWeight Create(int value)
        {
            if (value < MinValue || value > MaxValue)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Criterion weight must be between {MinValue} and {MaxValue}.");
            }

            return new CriterionWeight(value);
        }

        public static CriterionWeight Default() => new(DefaultValue);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value.ToString();
    }
}
