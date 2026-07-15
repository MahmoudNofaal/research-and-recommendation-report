namespace Domain.ValueObjects
{
    /// <summary>
    /// Strongly typed identifier for a <c>ReportTopic</c> child entity.
    /// </summary>
    public readonly record struct ReportTopicId(Guid Value)
    {
        public static ReportTopicId New()
            => new(Guid.NewGuid());

        public static ReportTopicId From(Guid value)
            => new(value);

        public override string ToString()
            => Value.ToString();
    }
}
