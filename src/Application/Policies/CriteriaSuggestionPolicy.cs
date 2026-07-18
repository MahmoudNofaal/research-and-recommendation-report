using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Policies
{
    /// <summary>
    /// Matches the topics the user has typed so far against admin-curated
    /// <see cref="CriteriaPreset"/> reference data, and returns the
    /// best-matching preset's suggested criteria as chips the user can accept
    /// with one tap (see architecture-plan.md, Flow 6 "User Gets
    /// Suggestions", and ui-ux-specification.md, "8.6 Reports/Create, Step
    /// 7").
    ///
    /// Like <see cref="StyleSuggestionPolicy"/>, matching is a whole-word
    /// overlap score (see <see cref="KeywordOverlap"/>) between the typed
    /// topic names and a preset's <see cref="CriteriaPreset.Name"/> and
    /// <see cref="CriteriaPreset.Category"/> — there is no exact key to join
    /// on, since topics are freely typed. Criteria the user has already added
    /// to the request are filtered out of the result, mirroring the
    /// wizard's own "Already added" duplicate handling (see
    /// ui-ux-specification.md, "7.2 Form Controls").
    /// </summary>
    public static class CriteriaSuggestionPolicy
    {
        public static IReadOnlyList<SuggestedCriterion> Suggest
        (
            IReadOnlyList<ReportTopicName> typedTopics,
            IReadOnlyList<CriteriaPreset> activePresets,
            IReadOnlyList<ReportCriterionName> alreadyAddedCriteria
        )
        {
            ArgumentNullException.ThrowIfNull(typedTopics);
            ArgumentNullException.ThrowIfNull(activePresets);
            ArgumentNullException.ThrowIfNull(alreadyAddedCriteria);

            if (typedTopics.Count == 0 || activePresets.Count == 0)
            {
                return [];
            }

            var combinedTopicText = string.Join(" ", typedTopics.Select(topic => topic.Value));

            var bestPreset = activePresets
                .Select(preset => (Preset: preset, Score: ScorePreset(combinedTopicText, preset)))
                .Where(candidate => candidate.Score > 0)
                .OrderByDescending(candidate => candidate.Score)
                .ThenBy(candidate => candidate.Preset.SortOrder)
                .Select(candidate => candidate.Preset)
                .FirstOrDefault();

            if (bestPreset is null)
            {
                return [];
            }

            var alreadyAddedKeys = alreadyAddedCriteria
                .Select(name => name.ComparisonKey)
                .ToHashSet();

            return bestPreset.SuggestedCriteria
                .Where(suggested => !alreadyAddedKeys.Contains(suggested.Name.ComparisonKey))
                .ToList();
        }

        private static int ScorePreset(string combinedTopicText, CriteriaPreset preset)
            => KeywordOverlap.Score(combinedTopicText, preset.Name)
                + KeywordOverlap.Score(combinedTopicText, preset.Category.Value);
    }
}
