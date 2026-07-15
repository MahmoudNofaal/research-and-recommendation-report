namespace Domain.ValueObjects
{
    /// <summary>
    /// Strongly typed identifier for a <c>ReportCriterion</c> child entity.
    /// </summary>
    public readonly record struct ReportCriterionId(Guid Value)
    {
        public static ReportCriterionId New() => new(Guid.NewGuid());

        public static ReportCriterionId From(Guid value) => new(value);

        public override string ToString() => Value.ToString();
    }
}
