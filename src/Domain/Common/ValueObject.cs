namespace Domain.Common
{
    /// <summary>
    /// Base type for every value object in the domain. Value objects are immutable
    /// and compared structurally: two instances are equal if every one of their
    /// equality components is equal, regardless of identity.
    /// </summary>
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        protected abstract IEnumerable<object?> GetEqualityComponents();

        public override bool Equals(object? obj) => Equals(obj as ValueObject);

        public bool Equals(ValueObject? other)
        {
            if (other is null || GetType() != other.GetType())
            {
                return false;
            }

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode() =>
            GetEqualityComponents().Aggregate(0, (hash, component) => HashCode.Combine(hash, component));

        public static bool operator ==(ValueObject? left, ValueObject? right) =>
            left is null ? right is null : left.Equals(right);

        public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
    }
}
