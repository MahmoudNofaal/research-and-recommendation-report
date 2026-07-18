using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Abstractions.Reports
{
    /// <summary>
    /// Read-only, ownership-scoped lookups for the single- and small-set
    /// aggregate reads that recur across several query handlers (report
    /// details, preview, regeneration) — see architecture-plan.md, "Ownership
    /// pattern". Broader, paginated/filterable projections (report history,
    /// search) instead compose custom LINQ directly against
    /// <see cref="Application.Abstractions.Data.IApplicationDbContext"/>,
    /// since a fixed repository method per possible filter/sort combination
    /// would not scale; this port exists only for the handful of lookups that
    /// are always "the whole aggregate, for this user, by id."
    ///
    /// Every result here is untracked: callers only ever display what they
    /// get back, never mutate it — mutation goes through
    /// <see cref="IReportWriteRepository"/> instead.
    /// </summary>
    public interface IReportReadRepository
    {
        Task<ReportRequest?> GetReportRequestAsync
        (
            ReportRequestId id,
            UserId userId,
            CancellationToken cancellationToken
        );

        Task<GeneratedReport?> GetGeneratedReportAsync
        (
            GeneratedReportId id,
            UserId userId,
            CancellationToken cancellationToken
        );

        /// <summary>
        /// The current (highest <see cref="GeneratedReport.Version"/>)
        /// generated report for a submitted request — what Preview and
        /// Details both show by default.
        /// </summary>
        Task<GeneratedReport?> GetCurrentGeneratedReportForRequestAsync
        (
            ReportRequestId reportRequestId,
            UserId userId,
            CancellationToken cancellationToken
        );

        /// <summary>
        /// Every generation attempt for a request, in order — the Details
        /// page's generation-history timeline (see
        /// ui-ux-specification.md, "8.7 Reports/Details").
        /// </summary>
        Task<IReadOnlyList<ReportGenerationRun>> GetGenerationRunsForRequestAsync
        (
            ReportRequestId reportRequestId,
            UserId userId,
            CancellationToken cancellationToken
        );
    }
}
