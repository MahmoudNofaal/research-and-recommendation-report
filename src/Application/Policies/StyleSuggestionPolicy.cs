using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Policies
{
    /// <summary>
    /// Decides which, if any, admin-curated <see cref="ReportStylePreset"/>
    /// to badge as "Suggested for your audience" while the user is still
    /// choosing a report style (see ui-ux-specification.md, "8.6
    /// Reports/Create, Step 5"). The suggestion never blocks or removes the
    /// other style cards — this only ever adds a ribbon to one of them.
    ///
    /// <see cref="TargetAudience"/> is free text, so there is no exact key to
    /// match a preset's <see cref="ReportStylePreset.RecommendedAudience"/>
    /// against; matching instead scores how many whole words the two
    /// descriptions share (see <see cref="KeywordOverlap"/>) and picks the
    /// preset with the highest score, breaking ties by
    /// <see cref="ReportStylePreset.SortOrder"/>. A typed audience that
    /// shares no words with any preset gets no suggestion at all, rather than
    /// an arbitrary guess.
    /// </summary>
    public static class StyleSuggestionPolicy
    {
        public static ReportStylePreset? Suggest(TargetAudience typedAudience, IReadOnlyList<ReportStylePreset> activePresets)
        {
            ArgumentNullException.ThrowIfNull(typedAudience);
            ArgumentNullException.ThrowIfNull(activePresets);

            return activePresets
                .Select(preset => (Preset: preset, Score: KeywordOverlap.Score(typedAudience.Value, preset.RecommendedAudience.Value)))
                .Where(candidate => candidate.Score > 0)
                .OrderByDescending(candidate => candidate.Score)
                .ThenBy(candidate => candidate.Preset.SortOrder)
                .Select(candidate => candidate.Preset)
                .FirstOrDefault();
        }
    }
}
