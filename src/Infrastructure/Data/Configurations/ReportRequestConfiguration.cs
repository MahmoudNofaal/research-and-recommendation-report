using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// Maps the <see cref="ReportRequest"/> aggregate root's own scalar state.
    /// The owned <see cref="ReportRequest.Topics"/> and
    /// <see cref="ReportRequest.Criteria"/> collections are configured
    /// separately in <see cref="ReportTopicConfiguration"/> and
    /// <see cref="ReportCriterionConfiguration"/>, so each child collection's
    /// mapping lives next to the entity it actually describes rather than
    /// bloating this file.
    /// </summary>
    public sealed class ReportRequestConfiguration : IEntityTypeConfiguration<ReportRequest>
    {
        public void Configure(EntityTypeBuilder<ReportRequest> builder)
        {
            builder.ToTable("ReportRequests");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .HasConversion(id => id.Value, value => ReportRequestId.From(value))
                .ValueGeneratedNever();

            builder.Property(r => r.UserId)
                .HasConversion(id => id.Value, value => UserId.From(value))
                .IsRequired();

            builder.Property(r => r.Title)
                .HasConversion(title => title.Value, value => ReportTitle.Create(value))
                .HasMaxLength(ReportTitle.MaxLength)
                .IsRequired();

            builder.Property(r => r.TargetAudience)
                .HasConversion(audience => audience.Value, value => TargetAudience.Create(value))
                .HasMaxLength(TargetAudience.MaxLength)
                .IsRequired();

            builder.Property(r => r.ReportMode)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(r => r.Style)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(r => r.TechnicalDepth)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(r => r.ReportLength)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(r => r.PreferredAiProvider)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(r => r.Status)
                .HasConversion<int>()
                .IsRequired();

            // IndustryOrDomain, CurrentTechnologyStack, PerformanceRequirements,
            // SecurityRequirements, BudgetConsiderations, MustInclude, and
            // MustAvoid all share the same optional-SupplementaryNote shape
            // (see SupplementaryNote's own remarks), so they share one
            // conversion helper instead of seven near-identical blocks.
            ConfigureSupplementaryNote(builder.Property(r => r.IndustryOrDomain), "IndustryOrDomain");
            ConfigureSupplementaryNote(builder.Property(r => r.CurrentTechnologyStack), "CurrentTechnologyStack");
            ConfigureSupplementaryNote(builder.Property(r => r.PerformanceRequirements), "PerformanceRequirements");
            ConfigureSupplementaryNote(builder.Property(r => r.SecurityRequirements), "SecurityRequirements");
            ConfigureSupplementaryNote(builder.Property(r => r.BudgetConsiderations), "BudgetConsiderations");
            ConfigureSupplementaryNote(builder.Property(r => r.MustInclude), "MustInclude");
            ConfigureSupplementaryNote(builder.Property(r => r.MustAvoid), "MustAvoid");

            builder.Property(r => r.CreatedAtUtc).IsRequired();
            builder.Property(r => r.UpdatedAtUtc).IsRequired();
            builder.Property(r => r.DeletedAtUtc);

            // Every real read already goes through the write/read repository's
            // own explicit ownership + soft-delete filtering (see
            // architecture-plan.md, "Ownership pattern"), but a query-level
            // filter is added anyway as a safety net so a handler cannot
            // accidentally see a deleted request just by forgetting the
            // predicate.
            builder.HasQueryFilter(r => r.DeletedAtUtc == null);

            builder.HasIndex(r => r.UserId);

            // Domain events are an in-memory dispatch mechanism for
            // Infrastructure to publish after a successful commit; they are
            // never persisted themselves.
            builder.Ignore(r => r.DomainEvents);
        }

        private static void ConfigureSupplementaryNote
        (
            PropertyBuilder<SupplementaryNote?> propertyBuilder,
            string columnName
        )
        {
            propertyBuilder
                .HasConversion
                (
                    note => note == null ? null : note.Value,
                    value => value == null ? null : SupplementaryNote.Create(value)
                )
                .HasColumnName(columnName)
                .HasMaxLength(SupplementaryNote.MaxLength);
        }
    }
}