using Domain.Common;
using Domain.Errors;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// An admin-curated set of criteria suggested for a category of topics (for
    /// example, "Real-Time Communication"), surfaced to the user as they build a
    /// report request. <see cref="Activate"/>/<see cref="Deactivate"/> are simple,
    /// idempotent visibility toggles for this reference data — unlike
    /// <c>ReportTemplate</c>, flipping this flag twice has no consequence worth
    /// guarding against.
    /// </summary>
    public sealed class CriteriaPreset : AggregateRoot<CriteriaPresetId>
    {
        public const int MaxNameLength = 150;
        public const int MaxDescriptionLength = 500;

        private readonly List<SuggestedCriterion> _suggestedCriteria = [];

        private CriteriaPreset
        (
            CriteriaPresetId id,
            string name,
            string? description,
            PresetCategory category,
            int sortOrder
        ) : base(id)
        {
            Name = name;
            Description = description;
            Category = category;
            SortOrder = sortOrder;
            IsActive = true;
        }

        /// <summary>EF Core materialization constructor.</summary>
        private CriteriaPreset()
            : base(default!)
        {
            Name = null!;
            Category = null!;
        }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public PresetCategory Category { get; private set; }

        public bool IsActive { get; private set; }

        public int SortOrder { get; private set; }

        public IReadOnlyList<SuggestedCriterion> SuggestedCriteria => _suggestedCriteria.AsReadOnly();

        public static CriteriaPreset Create
        (
            string name,
            string? description,
            PresetCategory category,
            int sortOrder,
            IReadOnlyCollection<SuggestedCriterion> suggestedCriteria
        )
        {
            ArgumentNullException.ThrowIfNull(category);
            ArgumentNullException.ThrowIfNull(suggestedCriteria);

            if (suggestedCriteria.Count == 0)
            {
                throw new InvalidReportStateException
                (
                    ReportDomainError.CriteriaPreset.AtLeastOneSuggestedCriterionRequired
                );
            }

            var preset = new CriteriaPreset
            (
                CriteriaPresetId.New(),
                NormalizeName(name),
                NormalizeDescription(description),
                category,
                sortOrder
            );

            preset._suggestedCriteria.AddRange(suggestedCriteria);

            return preset;
        }

        public void ReplaceSuggestedCriteria(IReadOnlyCollection<SuggestedCriterion> suggestedCriteria)
        {
            ArgumentNullException.ThrowIfNull(suggestedCriteria);

            if (suggestedCriteria.Count == 0)
            {
                throw new InvalidReportStateException
                (
                    ReportDomainError.CriteriaPreset.AtLeastOneSuggestedCriterionRequired
                );
            }

            _suggestedCriteria.Clear();
            _suggestedCriteria.AddRange(suggestedCriteria);
        }

        public void UpdateDetails(string name, string? description, PresetCategory category, int sortOrder)
        {
            ArgumentNullException.ThrowIfNull(category);

            Name = NormalizeName(name);
            Description = NormalizeDescription(description);
            Category = category;
            SortOrder = sortOrder;
        }

        public void Activate() => IsActive = true;

        public void Deactivate() => IsActive = false;

        private static string NormalizeName(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var trimmed = name.Trim();
            if (trimmed.Length > MaxNameLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(name),
                    $"Preset name cannot exceed {MaxNameLength} characters."
                );
            }

            return trimmed;
        }

        private static string? NormalizeDescription(string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                return null;
            }

            var trimmed = description.Trim();
            if (trimmed.Length > MaxDescriptionLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(description),
                    $"Preset description cannot exceed {MaxDescriptionLength} characters."
                );
            }

            return trimmed;
        }
    }
}
