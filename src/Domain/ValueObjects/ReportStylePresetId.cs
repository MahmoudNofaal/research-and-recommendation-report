namespace Domain.ValueObjects
{
    /// <summary>
    /// Strongly typed identifier for a <c>ReportStylePreset</c> aggregate.
    /// </summary>
    public readonly record struct ReportStylePresetId(Guid Value)
    {
        public static ReportStylePresetId New() => new(Guid.NewGuid());

        public static ReportStylePresetId From(Guid value) => new(value);

        public override string ToString() => Value.ToString();
    }
}
