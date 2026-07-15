namespace Domain.ValueObjects
{
    /// <summary>
    /// Strongly typed identifier for the authenticated user who owns a piece of
    /// data. The actual <c>ApplicationUser</c> record is an Infrastructure/Identity
    /// concern; the Domain only ever needs the identifier to enforce ownership.
    /// </summary>
    public readonly record struct UserId(Guid Value)
    {
        public static UserId From(Guid value) => new(value);

        public override string ToString() => Value.ToString();
    }
}
