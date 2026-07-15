namespace Domain.ValueObjects
{
    /// <summary>
    /// Strongly typed identifier for a <c>ReportExport</c> aggregate.
    /// </summary>
    public readonly record struct ReportExportId(Guid Value)
    {
        public static ReportExportId New()
            => new(Guid.NewGuid());

        public static ReportExportId From(Guid value)
            => new(value);

        public override string ToString()
            => Value.ToString();
    }
}
