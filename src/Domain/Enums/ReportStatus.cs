namespace Domain.Enums
{
    /// <summary>
    /// The quality-gate outcome of a <c>GeneratedReport</c>'s current content, as
    /// computed by <see cref="Domain.Services.ReportQualityDomainService"/>. This is
    /// deliberately distinct from soft deletion (tracked separately via
    /// <c>DeletedAtUtc</c>) and from the request's own editability status.
    /// </summary>
    public enum ReportStatus
    {
        /// <summary>
        /// The content has at least one blocking quality warning, or falls below
        /// the minimum acceptable quality score, and should not be presented as a
        /// finished result without the user's awareness.
        /// </summary>
        Draft = 0,

        /// <summary>
        /// The content has cleared the quality gate and is fit to preview and export.
        /// </summary>
        Ready = 1
    }
}
