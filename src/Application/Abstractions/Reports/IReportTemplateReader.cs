using Domain.Entities;

namespace Application.Abstractions.Reports
{
    /// <summary>
    /// Read-only access to the admin-curated <see cref="ReportTemplate"/>
    /// reference data that drives prompt composition. Version 1 has no
    /// self-service template management (see project-vision-statement.md,
    /// "Excluded from Version 1"), so this only ever needs to answer "which
    /// template is active right now" — templates are otherwise managed
    /// directly by the project owner.
    /// </summary>
    public interface IReportTemplateReader
    {
        /// <summary>
        /// The template <c>GenerateReportCommandHandler</c> composes its
        /// prompt from, or <see langword="null"/> if none has been activated
        /// yet — see
        /// <c>ReportApplicationErrors.ReportTemplate.NoActiveTemplate</c>.
        /// </summary>
        Task<ReportTemplate?> GetActiveTemplateAsync(CancellationToken cancellationToken);
    }
}
