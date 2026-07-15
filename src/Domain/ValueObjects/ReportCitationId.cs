namespace Domain.ValueObjects
{
    /// <summary>
    /// Strongly typed identifier for a <c>ReportCitation</c> child entity.
    /// </summary>
    public readonly record struct ReportCitationId(Guid Value)
    {
        public static ReportCitationId New() => new(Guid.NewGuid());

        public static ReportCitationId From(Guid value) => new(value);

        public override string ToString() => Value.ToString();
    }
}
