using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Abstractions.Reports
{
    /// <summary>
    /// Read-only access to admin-curated <see cref="CriteriaPreset"/>
    /// reference data, surfaced to the user while building a comparison
    /// report's criteria (or a research report's optional focus areas) — see
    /// ui-ux-specification.md, "8.6 Reports/Create, Step 7" and Flow 6 in
    /// architecture-plan.md ("Get Suggestions").
    /// </summary>
    public interface ICriteriaPresetReader
    {
        Task<IReadOnlyList<CriteriaPreset>> GetActivePresetsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Presets matching the category the user's typed topics suggest —
        /// narrows the suggestion list rather than showing every active
        /// preset regardless of relevance.
        /// </summary>
        Task<IReadOnlyList<CriteriaPreset>> GetActivePresetsByCategoryAsync
        (
            PresetCategory category,
            CancellationToken cancellationToken
        );
    }
}
