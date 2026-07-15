namespace Domain.Enums
{
    /// <summary>
    /// The editability lifecycle of a <c>ReportRequest</c>. A request starts as a
    /// <see cref="Draft"/> that the user can freely reshape, and becomes
    /// <see cref="Submitted"/> once it is frozen and handed off for generation.
    /// </summary>
    public enum ReportRequestStatus
    {
        Draft = 0,
        Submitted = 1
    }
}
