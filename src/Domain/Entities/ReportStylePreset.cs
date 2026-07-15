using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// An admin-curated suggestion of report style, technical depth, and target
    /// audience, offered to the user while still letting them override any part
    /// of it. <see cref="DefaultStyle"/> is included alongside <see cref="DefaultDepth"/>
    /// because a "style preset" that could not actually suggest a style would not
    /// fulfill the purpose implied by its own name and by the vision's style-suggestion
    /// feature — it is included here even though it did not appear as a literal
    /// column in the original schema sketch.
    /// </summary>
    public sealed class ReportStylePreset : AggregateRoot<ReportStylePresetId>
    {
        public const int MaxNameLength = 150;
        public const int MaxDescriptionLength = 500;

        private ReportStylePreset(
            ReportStylePresetId id,
            string name,
            string? description,
            TargetAudience recommendedAudience,
            ReportStyle defaultStyle,
            TechnicalDepth defaultDepth,
            int sortOrder)
            : base(id)
        {
            Name = name;
            Description = description;
            RecommendedAudience = recommendedAudience;
            DefaultStyle = defaultStyle;
            DefaultDepth = defaultDepth;
            SortOrder = sortOrder;
            IsActive = true;
        }

        /// <summary>EF Core materialization constructor.</summary>
        private ReportStylePreset()
            : base(default!)
        {
            Name = null!;
            RecommendedAudience = null!;
        }

        public string Name { get; private set; }

        public string? Description { get; private set; }

        public TargetAudience RecommendedAudience { get; private set; }

        public ReportStyle DefaultStyle { get; private set; }

        public TechnicalDepth DefaultDepth { get; private set; }

        public bool IsActive { get; private set; }

        public int SortOrder { get; private set; }

        public static ReportStylePreset Create(
            string name,
            string? description,
            TargetAudience recommendedAudience,
            ReportStyle defaultStyle,
            TechnicalDepth defaultDepth,
            int sortOrder)
        {
            ArgumentNullException.ThrowIfNull(recommendedAudience);

            return new ReportStylePreset(
                ReportStylePresetId.New(),
                NormalizeName(name),
                NormalizeDescription(description),
                recommendedAudience,
                defaultStyle,
                defaultDepth,
                sortOrder);
        }

        public void UpdateDetails(
            string name,
            string? description,
            TargetAudience recommendedAudience,
            ReportStyle defaultStyle,
            TechnicalDepth defaultDepth,
            int sortOrder)
        {
            ArgumentNullException.ThrowIfNull(recommendedAudience);

            Name = NormalizeName(name);
            Description = NormalizeDescription(description);
            RecommendedAudience = recommendedAudience;
            DefaultStyle = defaultStyle;
            DefaultDepth = defaultDepth;
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
                throw new ArgumentOutOfRangeException(nameof(name), $"Style preset name cannot exceed {MaxNameLength} characters.");
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
                throw new ArgumentOutOfRangeException(nameof(description), $"Style preset description cannot exceed {MaxDescriptionLength} characters.");
            }

            return trimmed;
        }
    }
}
