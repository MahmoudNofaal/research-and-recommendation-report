using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Abstractions.Reports
{
    /// <summary>
    /// The write-side port for the whole report-generation bounded context
    /// (see <see cref="Domain.Exceptions.ReportDomainException"/>'s own
    /// remarks for what that context spans): every aggregate a command
    /// handler loads for mutation, or newly creates, passes through here
    /// rather than through a raw
    /// <see cref="Application.Abstractions.Data.IApplicationDbContext"/>
    /// <c>DbSet</c>, so every write path applies the same ownership scoping
    /// (see architecture-plan.md, "Ownership pattern") in one place.
    ///
    /// Every <c>Get*</c> method returns a <em>tracked</em> aggregate — the
    /// same instance EF Core will persist the handler's in-place mutations of
    /// when <see cref="Application.Abstractions.Data.IUnitOfWork.SaveChangesAsync"/>
    /// is next called — which is why this port is distinct from the
    /// read-only, no-tracking <see cref="IReportReadRepository"/> used by
    /// query handlers that only ever display data.
    ///
    /// <c>Add*</c> methods are synchronous: EF Core's <c>DbSet.Add</c> only
    /// registers the new aggregate with the change tracker, it does not
    /// itself touch the database — the actual insert happens at the next
    /// <c>SaveChangesAsync</c>, exactly like every other write here.
    /// </summary>
    public interface IReportWriteRepository
    {
        void AddReportRequest(ReportRequest reportRequest);

        Task<ReportRequest?> GetReportRequestAsync
        (
            ReportRequestId id,
            UserId userId,
            CancellationToken cancellationToken
        );

        void AddGeneratedReport(GeneratedReport generatedReport);

        Task<GeneratedReport?> GetGeneratedReportAsync
        (
            GeneratedReportId id,
            UserId userId,
            CancellationToken cancellationToken
        );

        void AddGenerationRun(ReportGenerationRun generationRun);

        Task<ReportGenerationRun?> GetGenerationRunAsync
        (
            ReportGenerationRunId id,
            UserId userId,
            CancellationToken cancellationToken
        );

        void AddExport(ReportExport export);
    }
}
