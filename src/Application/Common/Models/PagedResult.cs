namespace Application.Common.Models
{
    /// <summary>
    /// One page of a larger result set, together with enough paging metadata
    /// for a caller (typically Web's history/search views) to render page
    /// controls without a second round trip. Used as the payload of
    /// <c>Result{PagedResult{TItem}}</c> returned by paged queries such as
    /// <c>SearchReportsQuery</c> (see ui-ux-specification.md, "8.5 Reports/Index").
    /// </summary>
    /// <typeparam name="TItem">The type of item on this page.</typeparam>
    public sealed class PagedResult<TItem>
    {
        private PagedResult(IReadOnlyList<TItem> items, int pageNumber, int pageSize, int totalCount)
        {
            Items = items;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public IReadOnlyList<TItem> Items { get; }

        /// <summary>The 1-based page this result represents.</summary>
        public int PageNumber { get; }

        public int PageSize { get; }

        /// <summary>The total number of items across every page, not just this one.</summary>
        public int TotalCount { get; }

        public int TotalPages
            => PageSize <= 0 ? 0 : (int) Math.Ceiling(TotalCount / (double) PageSize);

        public bool HasPreviousPage
            => PageNumber > 1;

        public bool HasNextPage
            => PageNumber < TotalPages;

        public static PagedResult<TItem> Create
        (
            IReadOnlyList<TItem> items,
            int pageNumber,
            int pageSize,
            int totalCount
        )
        {
            ArgumentNullException.ThrowIfNull(items);

            if (pageNumber < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be at least 1.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");
            }

            if (totalCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(totalCount), "Total count cannot be negative.");
            }

            return new PagedResult<TItem>(items, pageNumber, pageSize, totalCount);
        }

        /// <summary>Shorthand for a page with no results — the "filtered empty" state (see ui-ux-specification.md, "10. States").</summary>
        public static PagedResult<TItem> Empty(int pageNumber, int pageSize)
            => Create(Array.Empty<TItem>(), pageNumber, pageSize, totalCount: 0);
    }
}
