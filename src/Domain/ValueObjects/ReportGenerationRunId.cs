namespace Domain.ValueObjects
{
    /// <summary>
    /// Strongly typed identifier for a <c>ReportGenerationRun</c> aggregate.
    /// </summary>
    public readonly record struct ReportGenerationRunId(Guid Value)
    {
        public static ReportGenerationRunId New() => new(Guid.NewGuid());

        public static ReportGenerationRunId From(Guid value) => new(value);

        public override string ToString() => Value.ToString();
    }
}
