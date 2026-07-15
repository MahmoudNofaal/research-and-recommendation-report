namespace Domain.ValueObjects
{
    /// <summary>
    /// Strongly typed identifier for a <c>ReportTemplate</c> aggregate.
    /// </summary>
    public readonly record struct ReportTemplateId(Guid Value)
    {
        public static ReportTemplateId New()
            => new(Guid.NewGuid());

        public static ReportTemplateId From(Guid value)
            => new(value);

        public override string ToString()
            => Value.ToString();
    }
}
