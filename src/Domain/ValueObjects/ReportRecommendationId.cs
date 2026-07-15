namespace Domain.ValueObjects
{
    /// <summary>
    /// Strongly typed identifier for a <c>ReportRecommendation</c> child entity.
    /// </summary>
    public readonly record struct ReportRecommendationId(Guid Value)
    {
        public static ReportRecommendationId New()
            => new(Guid.NewGuid());

        public static ReportRecommendationId From(Guid value)
            => new(value);

        public override string ToString()
            => Value.ToString();
    }
}
