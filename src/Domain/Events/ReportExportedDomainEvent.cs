using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Events
{
    /// <summary>
    /// Raised when a user downloads a generated report in a given export format.
    /// </summary>
    public sealed class ReportExportedDomainEvent : DomainEvent
    {
        public ReportExportedDomainEvent
        (
            ReportExportId reportExportId,
            GeneratedReportId generatedReportId,
            UserId userId,
            ExportFormat format
        )
        {
            ReportExportId = reportExportId;
            GeneratedReportId = generatedReportId;
            UserId = userId;
            Format = format;
        }

        public ReportExportId ReportExportId { get; }

        public GeneratedReportId GeneratedReportId { get; }

        public UserId UserId { get; }

        public ExportFormat Format { get; }
    }
}
