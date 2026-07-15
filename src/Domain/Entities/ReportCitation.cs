using Domain.Common;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// A single source cited by a generated report when live research was used.
    /// Owned exclusively by its parent report and replaced wholesale on
    /// regeneration.
    /// </summary>
    public sealed class ReportCitation : Entity<ReportCitationId>
    {
        public const int MaxTitleLength = 300;
        public const int MaxSourceNameLength = 200;
        public const int MaxNotesLength = 500;

        private ReportCitation
        (
            ReportCitationId id,
            string title,
            SourceUrl url,
            string sourceName,
            DateTime? publishedAtUtc,
            DateTime accessedAtUtc,
            string? notes,
            int sortOrder
        ) : base(id)
        {
            Title = title;
            Url = url;
            SourceName = sourceName;
            PublishedAtUtc = publishedAtUtc;
            AccessedAtUtc = accessedAtUtc;
            Notes = notes;
            SortOrder = sortOrder;
        }

        /// <summary>EF Core materialization constructor.</summary>
        private ReportCitation()
            : base(default!)
        {
            Title = null!;
            Url = null!;
            SourceName = null!;
        }

        public string Title { get; }

        public SourceUrl Url { get; }

        public string SourceName { get; }

        public DateTime? PublishedAtUtc { get; }

        public DateTime AccessedAtUtc { get; }

        public string? Notes { get; }

        public int SortOrder { get; private set; }

        internal static ReportCitation Create
        (
            string title,
            SourceUrl url,
            string sourceName,
            DateTime? publishedAtUtc,
            DateTime accessedAtUtc,
            string? notes,
            int sortOrder
        )
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(title);
            ArgumentNullException.ThrowIfNull(url);
            ArgumentException.ThrowIfNullOrWhiteSpace(sourceName);

            var trimmedTitle = title.Trim();
            if (trimmedTitle.Length > MaxTitleLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(title),
                    $"Citation title cannot exceed {MaxTitleLength} characters."
                );
            }

            var trimmedSourceName = sourceName.Trim();
            if (trimmedSourceName.Length > MaxSourceNameLength)
            {
                throw new ArgumentOutOfRangeException
                (
                    nameof(sourceName),
                    $"Citation source name cannot exceed {MaxSourceNameLength} characters."
                );
            }

            string? trimmedNotes = null;
            if (!string.IsNullOrWhiteSpace(notes))
            {
                trimmedNotes = notes.Trim();
                if (trimmedNotes.Length > MaxNotesLength)
                {
                    throw new ArgumentOutOfRangeException
                    (
                        nameof(notes),
                        $"Citation notes cannot exceed {MaxNotesLength} characters."
                    );
                }
            }

            return new ReportCitation
            (
                ReportCitationId.New(),
                trimmedTitle,
                url,
                trimmedSourceName,
                publishedAtUtc,
                accessedAtUtc,
                trimmedNotes,
                sortOrder
            );
        }

        internal void UpdateSortOrder(int sortOrder)
            => SortOrder = sortOrder;
    }
}
