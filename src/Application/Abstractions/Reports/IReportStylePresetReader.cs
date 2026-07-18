using Domain.Entities;

namespace Application.Abstractions.Reports
{
    /// <summary>
    /// Read-only access to admin-curated <see cref="ReportStylePreset"/>
    /// reference data — the style/depth/audience suggestions offered while
    /// building a report request, which the user may accept or override in
    /// full (see <see cref="ReportStylePreset"/>'s own remarks).
    /// </summary>
    public interface IReportStylePresetReader
    {
        Task<IReadOnlyList<ReportStylePreset>> GetActivePresetsAsync(CancellationToken cancellationToken);
    }
}
