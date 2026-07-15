namespace Domain.Enums
{
    /// <summary>
    /// The requested overall length/depth of coverage for a generated report.
    /// Influences prompt composition and, indirectly, the minimum content length
    /// enforced by <c>ReportContent</c>.
    /// </summary>
    public enum ReportLength
    {
        Concise = 0,
        Standard = 1,
        Comprehensive = 2
    }
}
