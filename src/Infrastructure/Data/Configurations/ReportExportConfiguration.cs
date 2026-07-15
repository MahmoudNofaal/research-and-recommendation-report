using System.Reflection;
using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    /// <summary>
    /// Maps the immutable, append-only <see cref="ReportExport"/> aggregate
    /// root. An export is created once and never modified (see its own
    /// type-level remarks), so every property is a get-only auto-property with
    /// no setter at all — field access is used throughout.
    /// </summary>
    public sealed class ReportExportConfiguration : IEntityTypeConfiguration<ReportExport>
    {
        public void Configure(EntityTypeBuilder<ReportExport> builder)
        {
            builder.ToTable("ReportExports");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasConversion(id => id.Value, value => ReportExportId.From(value))
                .ValueGeneratedNever();

            builder.Property(e => e.GeneratedReportId)
                .HasConversion(id => id.Value, value => GeneratedReportId.From(value))
                .IsRequired();

            builder.Property(e => e.UserId)
                .HasConversion(id => id.Value, value => UserId.From(value))
                .IsRequired();

            builder.Property(e => e.Format)
                .HasConversion<int>()
                .IsRequired();

            // ExportFileName's only public factory (Create(baseFileName,
            // format)) re-derives and possibly appends an extension, which
            // would corrupt a value already read back from storage (a stored
            // "report.pdf" run back through Create(..., ExportFormat.Markdown)
            // would become "report.pdf.md"). The persisted string is always
            // exactly what a prior Create call already produced, so reading
            // it back goes through the private constructor directly instead.
            builder.Property(e => e.FileName)
                .HasConversion(fileName => fileName.Value, value => CreateExportFileName(value))
                .HasColumnName("FileName")
                .IsRequired();

            // ContentType.ForFormat(...) is likewise the only public
            // construction path (the MIME mapping is an intrinsic fact about
            // the ExportFormat, not free-form input — see ContentType's own
            // remarks). The stored value is always exactly what
            // ForFormat(Format) already produced, so the round trip
            // reconstructs that same instance through its private constructor
            // instead of duplicating the format-to-MIME mapping here.
            builder.Property(e => e.ContentType)
                .HasConversion(contentType => contentType.Value, value => CreateContentType(value))
                .HasColumnName("ContentType")
                .IsRequired();

            builder.Property(e => e.ExportedAtUtc)
                .IsRequired();

            builder.HasIndex(e => e.GeneratedReportId);
            builder.HasIndex(e => e.UserId);

            builder.Ignore(e => e.DomainEvents);

            builder.UsePropertyAccessMode(PropertyAccessMode.Field);
        }

        private static ExportFileName CreateExportFileName(string value)
            => (ExportFileName) GetPrivateStringConstructor(typeof(ExportFileName)).Invoke([value]);

        private static ContentType CreateContentType(string value)
            => (ContentType) GetPrivateStringConstructor(typeof(ContentType)).Invoke([value]);

        private static ConstructorInfo GetPrivateStringConstructor(Type type)
            => type.GetConstructor
            (
                BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                types: [typeof(string)],
                modifiers: null
            )
            ?? throw new InvalidOperationException($"{type.Name} has no private (string) constructor to materialize from storage.");
    }
}