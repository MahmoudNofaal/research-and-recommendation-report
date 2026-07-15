using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// Maps the <see cref="GeneratedReport"/> aggregate root's own scalar and
    /// owned-value-object state. The owned <see cref="GeneratedReport.Citations"/>
    /// and <see cref="GeneratedReport.Recommendations"/> collections are
    /// configured separately in <see cref="ReportCitationConfiguration"/> and
    /// <see cref="ReportRecommendationConfiguration"/>.
    ///
    /// This aggregate does not extend <c>AuditableEntity{TId}</c> or
    /// <c>SoftDeletableEntity{TId}</c> (see its own type-level remarks), so its
    /// <see cref="GeneratedReport.UpdatedAtUtc"/> and
    /// <see cref="GeneratedReport.DeletedAtUtc"/> columns are mapped directly
    /// here rather than inherited from a shared base configuration.
    /// </summary>
    public sealed class GeneratedReportConfiguration : IEntityTypeConfiguration<GeneratedReport>
    {
        public void Configure(EntityTypeBuilder<GeneratedReport> builder)
        {
            builder.ToTable("GeneratedReports");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasConversion(id => id.Value, value => GeneratedReportId.From(value))
                .ValueGeneratedNever();

            builder.Property(r => r.ReportRequestId)
                .HasConversion(id => id.Value, value => ReportRequestId.From(value))
                .IsRequired();

            builder.Property(r => r.UserId)
                .HasConversion(id => id.Value, value => UserId.From(value))
                .IsRequired();

            builder.Property(r => r.Title)
                .HasConversion(title => title.Value, value => ReportTitle.Create(value))
                .HasMaxLength(ReportTitle.MaxLength)
                .IsRequired();

            // Content carries two scalar components (Markdown + Summary), so
            // it is mapped as an owned type on the same table rather than a
            // single-column value converter.
            builder.OwnsOne(r => r.Content, content =>
            {
                content.Property(c => c.Markdown)
                    .HasColumnName("MarkdownContent")
                    .IsRequired();

                content.Property(c => c.Summary)
                    .HasColumnName("Summary")
                    .HasMaxLength(ReportContent.MaximumSummaryLength)
                    .IsRequired();
            });
            builder.Navigation(r => r.Content).IsRequired();

            builder.Property(r => r.AiProvider)
                .HasConversion<int>()
                .HasColumnName("AiProvider")
                .IsRequired();

            builder.Property(r => r.ModelName)
                .HasMaxLength(GeneratedReport.MaxModelNameLength)
                .IsRequired();

            builder.Property(r => r.PromptVersion)
                .HasConversion(version => version.Value, value => PromptVersion.Create(value))
                .HasMaxLength(PromptVersion.MaxLength)
                .IsRequired();

            builder.Property(r => r.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(r => r.QualityScore)
                .HasConversion(score => score.Value, value => ReportQualityScore.Create(value))
                .HasColumnName("QualityScore")
                .IsRequired();

            // QualityWarning has no identity of its own and the whole set is
            // always replaced together by ReportQualityDomainService, so a
            // JSON column (matching the QualityWarningsJson column already
            // sketched in the architecture plan) fits better than a child
            // table with a synthetic key.
            builder.OwnsMany(r => r.QualityWarnings, warnings =>
            {
                warnings.ToJson("QualityWarningsJson");

                warnings.Property(w => w.Code).IsRequired();

                warnings.Property(w => w.Message)
                    .HasMaxLength(QualityWarning.MaxMessageLength)
                    .IsRequired();

                warnings.Property(w => w.Severity)
                    .HasConversion<int>()
                    .IsRequired();
            });

            builder.Property(r => r.Version).IsRequired();
            builder.Property(r => r.GeneratedAtUtc).IsRequired();
            builder.Property(r => r.UpdatedAtUtc).IsRequired();
            builder.Property(r => r.DeletedAtUtc);

            builder.HasQueryFilter(r => r.DeletedAtUtc == null);

            builder.HasIndex(r => r.UserId);
            builder.HasIndex(r => r.ReportRequestId);

            builder.Ignore(r => r.DomainEvents);
        }
    }
}