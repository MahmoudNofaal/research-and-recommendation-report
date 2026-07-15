namespace Domain.ValueObjects
{
    /// <summary>
    /// Strongly typed identifier for a <c>CriteriaPreset</c> aggregate.
    /// </summary>
    public readonly record struct CriteriaPresetId(Guid Value)
    {
        public static CriteriaPresetId New()
            => new(Guid.NewGuid());

        public static CriteriaPresetId From(Guid value) 
            => new(value);

        public override string ToString()
            => Value.ToString();
    }
}
