using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Common.Errors
{
    /// <summary>
    /// Named catalogue of Application-level failures for the export use case —
    /// whether the requested export is even reachable — distinct from the
    /// underlying rendering failures Infrastructure's export renderers may
    /// raise (see architecture-plan.md, "Export Design").
    /// </summary>
    public static class ExportApplicationErrors
    {
        public static ApplicationError NotFound(ReportExportId id) => ApplicationError.NotFound
        (
            "Export.NotFound",
            $"No export with id '{id}' was found."
        );

        /// <summary>
        /// The requested <see cref="ExportFormat"/> exists as a domain concept
        /// but has been turned off for this deployment via
        /// <c>ExportOptions</c> (for example, PDF disabled).
        /// </summary>
        public static ApplicationError FormatDisabled(ExportFormat format) => ApplicationError.Validation
        (
            "Export.FormatDisabled",
            $"Exporting as {format} is not currently enabled."
        );

        /// <summary>
        /// The renderer for an enabled format failed to produce a file. This
        /// is the Application-facing translation of an Infrastructure
        /// rendering exception — the underlying cause stays in server logs,
        /// and the user only ever sees this generic, retryable message (see
        /// ui-ux-specification.md, "8.11 Exports/DownloadError").
        /// </summary>
        public static ApplicationError RenderingFailed(ExportFormat format) => ApplicationError.Failure
        (
            "Export.RenderingFailed",
            $"We couldn't generate your {format} file just now. Please try again."
        );

        /// <summary>
        /// The report backing this export has since been soft-deleted (see
        /// <c>GeneratedReport.Delete</c>) and can no longer be downloaded.
        /// </summary>
        public static ApplicationError ReportDeleted(GeneratedReportId generatedReportId) => ApplicationError.Conflict
        (
            "Export.ReportDeleted",
            $"Report '{generatedReportId}' has been deleted and can no longer be exported."
        );
    }
}
