namespace Domain.Enums
{
    /// <summary>
    /// The narrative style a generated report should adopt, which the user may
    /// pick directly or accept as a suggestion derived from a
    /// <c>ReportStylePreset</c>.
    /// </summary>
    public enum ReportStyle
    {
        Technical = 0,
        Executive = 1,
        Educational = 2,
        Balanced = 3
    }
}
