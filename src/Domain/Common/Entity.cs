namespace Domain.Common
{
    /// <summary>
    /// Base type for every domain entity. Equality is identity-based: two entities
    /// of the same runtime type are equal if and only if their identifiers are equal,
    /// regardless of the state of their other fields.
    /// </summary>
    /// <typeparam name="TId">The strongly typed identifier for this entity.</typeparam>
    public abstract class Entity<TId> : IEquatable<Entity<TId>>
        where TId : notnull
    {
        protected Entity(TId id)
        {
            Id = id;
        }

        public TId Id { get; }

        public override bool Equals(object? obj) => Equals(obj as Entity<TId>);

        public bool Equals(Entity<TId>? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (GetType() != other.GetType())
            {
                return false;
            }

            return Id.Equals(other.Id);
        }

        public override int GetHashCode() => HashCode.Combine(GetType(), Id);

        public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
            left is null ? right is null : left.Equals(right);

        public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
    }
}
