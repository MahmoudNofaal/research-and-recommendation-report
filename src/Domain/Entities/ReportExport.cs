using Domain.Common;
using Domain.Enums;
using Domain.Events;
using Domain.ValueObjects;

namespace Domain.Entities
{
    /// <summary>
    /// An immutable record of a single download of a generated report in a given
    /// format. Once created, an export is never modified or deleted — it exists
    /// purely as an audit trail of what was downloaded, when, and as what file.
    /// </summary>
    public sealed class ReportExport : AggregateRoot<ReportExportId>
    {
        private ReportExport
        (
            ReportExportId id,
            GeneratedReportId generatedReportId,
            UserId userId,
            ExportFormat format,
            ExportFileName fileName,
            ContentType contentType,
            DateTime exportedAtUtc
        ) : base(id)
        {
            GeneratedReportId = generatedReportId;
            UserId = userId;
            Format = format;
            FileName = fileName;
            ContentType = contentType;
            ExportedAtUtc = exportedAtUtc;
        }

        /// <summary>EF Core materialization constructor.</summary>
        private ReportExport()
            : base(default!)
        {
            FileName = null!;
            ContentType = null!;
        }

        public GeneratedReportId GeneratedReportId { get; }

        public UserId UserId { get; }

        public ExportFormat Format { get; }

        public ExportFileName FileName { get; }

        public ContentType ContentType { get; }

        public DateTime ExportedAtUtc { get; }

        public static ReportExport Create
        (
            GeneratedReportId generatedReportId,
            UserId userId,
            ExportFormat format,
            ExportFileName fileName,
            DateTime exportedAtUtc
        )
        {
            ArgumentNullException.ThrowIfNull(fileName);

            var export = new ReportExport
            (
                ReportExportId.New(),
                generatedReportId,
                userId,
                format,
                fileName,
                ContentType.ForFormat(format),
                exportedAtUtc
            );

            export.RaiseDomainEvent
            (
                new ReportExportedDomainEvent(export.Id, generatedReportId, userId, format)
            );

            return export;
        }
    }
}
