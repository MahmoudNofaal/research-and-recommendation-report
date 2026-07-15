using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// Maps <see cref="GeneratedReport.Citations"/> as an owned collection
    /// stored in the <c>ReportCitations</c> table. Owned exclusively by its
    /// parent report and replaced wholesale on regeneration (see
    /// <see cref="GeneratedReport.Regenerate"/>).
    /// </summary>
    public sealed class ReportCitationConfiguration : IEntityTypeConfiguration<GeneratedReport>
    {
        public void Configure(EntityTypeBuilder<GeneratedReport> builder)
        {
            builder.Navigation(r => r.Citations)
                .HasField("_citations")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.OwnsMany(r => r.Citations, citations =>
            {
                citations.ToTable("ReportCitations");
                citations.WithOwner().HasForeignKey("GeneratedReportId");
                citations.HasKey(c => c.Id);

                citations.Property(c => c.Id)
                    .HasConversion(id => id.Value, value => ReportCitationId.From(value))
                    .ValueGeneratedNever();

                citations.Property(c => c.Title)
                    .HasMaxLength(ReportCitation.MaxTitleLength)
                    .IsRequired();

                citations.Property(c => c.Url)
                    .HasConversion(url => url.Value.ToString(), value => SourceUrl.Create(value))
                    .HasColumnName("Url")
                    .IsRequired();

                citations.Property(c => c.SourceName)
                    .HasMaxLength(ReportCitation.MaxSourceNameLength)
                    .IsRequired();

                citations.Property(c => c.PublishedAtUtc);

                citations.Property(c => c.AccessedAtUtc)
                    .IsRequired();

                citations.Property(c => c.Notes)
                    .HasMaxLength(ReportCitation.MaxNotesLength);

                citations.Property(c => c.SortOrder)
                    .IsRequired();

                citations.UsePropertyAccessMode(PropertyAccessMode.Field);
            });
        }
    }
}