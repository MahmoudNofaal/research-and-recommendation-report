using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// A single suggested criterion or research focus area from a
    /// <c>CriteriaPreset</c>: a name paired with a short explanation of why it
    /// matters. Modeled as a value object rather than a child entity because a
    /// suggestion has no identity or lifecycle of its own — a preset's
    /// suggestions are simply replaced whole when the preset is edited.
    /// </summary>
    public sealed class SuggestedCriterion : ValueObject
    {
        public const int MaxDescriptionLength = 300;

        public ReportCriterionName Name { get; }

        public string Description { get; }

        private SuggestedCriterion(ReportCriterionName name, string description)
        {
            Name = name;
            Description = description;
        }

        public static SuggestedCriterion Create(ReportCriterionName name, string description)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(description);

            var trimmed = description.Trim();
            if (trimmed.Length > MaxDescriptionLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(description),
                    $"Suggested criterion description cannot exceed {MaxDescriptionLength} characters."
                );
            }

            return new SuggestedCriterion(name, trimmed);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Name;
            yield return Description;
        }
    }
}
