namespace Domain.ValueObjects
{
    /// <summary>
    /// Strongly typed identifier for a <c>GeneratedReport</c> aggregate.
    /// </summary>
    public readonly record struct GeneratedReportId(Guid Value)
    {
        public static GeneratedReportId New()
            => new(Guid.NewGuid());

        public static GeneratedReportId From(Guid value) 
            => new(value);

        public override string ToString()
            => Value.ToString();
    }
}
