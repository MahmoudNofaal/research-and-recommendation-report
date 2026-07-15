using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// A single topic within a <c>ReportRequest</c>. In single-topic research
    /// mode this is the subject being explained; in comparison mode it is one of
    /// the options being compared. Owned exclusively by its parent request — it
    /// has no repository, no independent lifecycle, and is only ever created,
    /// renamed, or removed through the request's own behavior methods.
    /// </summary>
    public sealed class ReportTopic : Entity<ReportTopicId>
    {
        public const int MaxDescriptionLength = 1000;

        private ReportTopic(ReportTopicId id, ReportTopicName name, string? description, int sortOrder)
            : base(id)
        {
            Name = name;
            Description = description;
            SortOrder = sortOrder;
        }

        /// <summary>EF Core materialization constructor.</summary>
        private ReportTopic()
            : base(default!)
        {
            Name = null!;
        }

        public ReportTopicName Name { get; private set; }

        public string? Description { get; private set; }

        public int SortOrder { get; private set; }

        internal static ReportTopic Create(ReportTopicName name, string? description, int sortOrder)
            => new(ReportTopicId.New(), name, NormalizeDescription(description), sortOrder);

        internal void Rename(ReportTopicName name)
            => Name = name;

        internal void UpdateDescription(string? description)
            => Description = NormalizeDescription(description);

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
                    $"Topic description cannot exceed {MaxDescriptionLength} characters."
                );
            }

            return trimmed;
        }
    }
}
