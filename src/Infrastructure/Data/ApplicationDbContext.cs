using Application.Abstractions.Data;
using Domain.Entities;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    /// <summary>
    /// The single EF Core context for the application: Identity tables plus
    /// every Domain aggregate root. Implements <see cref="IApplicationDbContext"/>
    /// so query handlers in Application can depend on the port rather than this
    /// concrete type.
    ///
    /// Registration (in the Web composition root, not here) is expected to
    /// wire up <c>AuditableEntitySaveChangesInterceptor</c> and
    /// <c>SoftDeleteSaveChangesInterceptor</c> via
    /// <c>optionsBuilder.AddInterceptors(...)</c>; this class itself stays
    /// agnostic of which interceptors are attached.
    /// </summary>
    public sealed class ApplicationDbContext
        : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ReportRequest> ReportRequests => Set<ReportRequest>();

        public DbSet<GeneratedReport> GeneratedReports => Set<GeneratedReport>();

        public DbSet<ReportGenerationRun> ReportGenerationRuns => Set<ReportGenerationRun>();

        public DbSet<ReportExport> ReportExports => Set<ReportExport>();

        public DbSet<ReportTemplate> ReportTemplates => Set<ReportTemplate>();

        public DbSet<CriteriaPreset> CriteriaPresets => Set<CriteriaPreset>();

        public DbSet<ReportStylePreset> ReportStylePresets => Set<ReportStylePreset>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Data/Configurations/*.cs (one or more IEntityTypeConfiguration<T>
            // per aggregate, plus one per owned child collection) supply the
            // mapping every aggregate needs beyond EF Core's default
            // conventions: private constructors, strongly typed IDs
            // (ReportRequestId, GeneratedReportId, ...), value-object
            // properties (ReportTitle, TargetAudience, SourceUrl, ...), and
            // privately backed collections (_topics, _criteria, _citations,
            // _recommendations, _suggestedCriteria) are all configured
            // explicitly there rather than relying on convention.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}