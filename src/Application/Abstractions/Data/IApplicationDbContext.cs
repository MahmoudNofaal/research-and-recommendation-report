using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data
{
    /// <summary>
    /// Read-oriented view of the persistence context: queryable <see cref="DbSet{TEntity}"/>s
    /// for every aggregate root, so query handlers can compose LINQ projections
    /// directly instead of needing a bespoke repository method for every shape
    /// of data they need to read.
    ///
    /// Application intentionally depends on <c>Microsoft.EntityFrameworkCore</c>
    /// only for the <see cref="DbSet{TEntity}"/> type itself — never for
    /// <c>DbContext</c>, change tracking, migrations, or any other EF Core
    /// implementation detail. Those all stay in Infrastructure, where the
    /// concrete <c>ApplicationDbContext</c> backing this interface lives.
    ///
    /// Writes never go through this interface. Command handlers mutate
    /// aggregates loaded through the write-side repository ports
    /// (<c>IReportWriteRepository</c>, etc.) and commit through
    /// <see cref="IUnitOfWork"/>; keeping the two concerns on separate
    /// interfaces makes it obvious, at the handler level, which use cases only
    /// read and which ones intend to change state.
    ///
    /// Identity-related sets (<c>ApplicationUser</c>, <c>ApplicationRole</c>)
    /// are deliberately absent — Application never references Identity types,
    /// only <see cref="Domain.ValueObjects.UserId"/>.
    /// </summary>
    public interface IApplicationDbContext
    {
        DbSet<ReportRequest> ReportRequests { get; }

        DbSet<GeneratedReport> GeneratedReports { get; }

        DbSet<ReportGenerationRun> ReportGenerationRuns { get; }

        DbSet<ReportExport> ReportExports { get; }

        DbSet<ReportTemplate> ReportTemplates { get; }

        DbSet<CriteriaPreset> CriteriaPresets { get; }

        DbSet<ReportStylePreset> ReportStylePresets { get; }
    }
}