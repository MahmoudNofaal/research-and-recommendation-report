namespace Domain.Enums
{
    /// <summary>
    /// How strongly a scenario-based recommendation endorses the recommended
    /// option, in the ubiquitous language a reader would expect from an advisory
    /// report rather than a raw numeric confidence score.
    /// </summary>
    public enum RecommendationStrength
    {
        NotRecommended = 0,
        ConditionallyRecommended = 1,
        Recommended = 2,
        StronglyRecommended = 3
    }
}
