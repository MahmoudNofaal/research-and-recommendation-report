using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// A single evaluation criterion or research focus area within a
    /// <c>ReportRequest</c> (for example, "Latency" for comparison reports or
    /// "Implementation Steps" for single-topic research). Owned exclusively by
    /// its parent request.
    /// </summary>
    public sealed class ReportCriterion : Entity<ReportCriterionId>
    {
        public const int MaxDescriptionLength = 1000;

        private ReportCriterion
        (
            ReportCriterionId id,
            ReportCriterionName name,
            string? description,
            CriterionWeight weight,
            int sortOrder
        ) : base(id)
        {
            Name = name;
            Description = description;
            Weight = weight;
            SortOrder = sortOrder;
        }

        /// <summary>EF Core materialization constructor.</summary>
        private ReportCriterion()
            : base(default!)
        {
            Name = null!;
            Weight = null!;
        }

        public ReportCriterionName Name { get; private set; }

        public string? Description { get; private set; }

        public CriterionWeight Weight { get; private set; }

        public int SortOrder { get; private set; }

        internal static ReportCriterion Create
        (
            ReportCriterionName name,
            string? description,
            CriterionWeight? weight,
            int sortOrder
        ) => new
        (
            ReportCriterionId.New(),
            name,
            NormalizeDescription(description),
            weight ?? CriterionWeight.Default(),
            sortOrder
        );

        internal void Rename(ReportCriterionName name)
            => Name = name;

        internal void UpdateDescription(string? description)
            => Description = NormalizeDescription(description);

        internal void UpdateWeight(CriterionWeight weight)
            => Weight = weight;

        internal void UpdateSortOrder(int sortOrder)
            => SortOrder = sortOrder;

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
                    $"Criterion description cannot exceed {MaxDescriptionLength} characters."
                );
            }

            return trimmed;
        }
    }
}
