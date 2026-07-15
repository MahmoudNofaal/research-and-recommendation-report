namespace Domain.ValueObjects
{
    /// <summary>
    /// Strongly typed identifier for a <c>ReportRequest</c> aggregate, preventing
    /// accidental mix-ups with other Guid-backed identifiers in method signatures.
    /// </summary>
    public readonly record struct ReportRequestId(Guid Value)
    {
        public static ReportRequestId New()
            => new(Guid.NewGuid());

        public static ReportRequestId From(Guid value)
            => new(value);

        public override string ToString()
            => Value.ToString();
    }
}
