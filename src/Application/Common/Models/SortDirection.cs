namespace Application.Common.Models
{
    /// <summary>
    /// The direction a paged/sortable query orders its results in — used by
    /// history and search queries such as <c>SearchReportsQuery</c> wherever
    /// the UI exposes a sortable column (see ui-ux-specification.md, "7.5
    /// Tables" and "8.5 Reports/Index").
    /// </summary>
    public enum SortDirection
    {
        Ascending = 0,
        Descending = 1
    }
}
