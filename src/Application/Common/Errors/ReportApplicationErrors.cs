using Domain.ValueObjects;

namespace Application.Common.Errors
{
    /// <summary>
    /// Named catalogue of Application-level failures for report requests,
    /// generated reports, and generation runs — the use-case-boundary concern
    /// of "does this exist" that sits above Domain's own business rules (see
    /// <see cref="Domain.Errors.ReportDomainError"/> for those). Every lookup
    /// in this bounded context is scoped by the current user's
    /// <see cref="UserId"/> at the repository level (see architecture-plan.md,
    /// "Ownership pattern"), so a resource that exists but belongs to someone
    /// else and a resource that simply does not exist are deliberately
    /// indistinguishable here — surfacing "not yours" instead of "not found"
    /// would leak the existence of other users' data.
    /// </summary>
    public static class ReportApplicationErrors
    {
        public static class ReportRequest
        {
            public static ApplicationError NotFound(ReportRequestId id) => ApplicationError.NotFound
            (
                "ReportRequest.NotFound",
                $"No report request with id '{id}' was found."
            );

            public static ApplicationError NotSubmitted(ReportRequestId id) => ApplicationError.Conflict
            (
                "ReportRequest.NotSubmitted",
                $"Report request '{id}' has not been submitted yet and cannot be generated."
            );
        }

        public static class GeneratedReport
        {
            public static ApplicationError NotFound(GeneratedReportId id) => ApplicationError.NotFound
            (
                "GeneratedReport.NotFound",
                $"No generated report with id '{id}' was found."
            );
        }

        public static class ReportGenerationRun
        {
            public static ApplicationError NotFound(ReportGenerationRunId id) => ApplicationError.NotFound
            (
                "ReportGenerationRun.NotFound",
                $"No generation run with id '{id}' was found."
            );
        }

        public static class ReportTemplate
        {
            /// <summary>
            /// No <c>ReportTemplate</c> is currently active. Version 1 has no
            /// self-service template management (see
            /// project-vision-statement.md, "Excluded from Version 1"), so
            /// this can only happen if the project owner has not yet
            /// activated one manually.
            /// </summary>
            public static ApplicationError NoActiveTemplate => ApplicationError.Failure
            (
                "ReportTemplate.NoActiveTemplate",
                "No active report template is configured, so generation cannot proceed."
            );
        }
    }
}
