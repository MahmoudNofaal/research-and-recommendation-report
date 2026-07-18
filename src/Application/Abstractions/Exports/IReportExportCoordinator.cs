using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Abstractions.Exports
{
    /// <summary>
    /// The rendered bytes of one export, paired with the same
    /// <see cref="ExportFileName"/> and <see cref="ContentType"/> value
    /// objects a <c>ReportExport</c> audit record carries — so the command
    /// handler that persists the <c>ReportExport</c> and the controller
    /// action that streams the file back to the browser both work from
    /// exactly one shape.
    /// </summary>
    public sealed record ExportedFile(byte[] Content, ExportFileName FileName, ContentType ContentType);

    /// <summary>
    /// Selects the <see cref="IReportExportRenderer"/> matching the requested
    /// <see cref="ExportFormat"/> and produces the finished, named,
    /// correctly-content-typed file — the single entry point
    /// <c>ExportReportCommandHandler</c> calls after verifying the report's
    /// ownership (see architecture-plan.md, "Export Design": "Web only
    /// returns the file response").
    /// </summary>
    public interface IReportExportCoordinator
    {
        /// <summary>
        /// <paramref name="baseFileName"/> is the sanitized, extension-free
        /// name an Application-level naming policy has already derived from
        /// the report's title — this coordinator only appends the format's
        /// extension via <see cref="ExportFileName.Create"/>; it does not
        /// invent a name of its own.
        /// </summary>
        Task<ExportedFile> ExportAsync
        (
            GeneratedReport report,
            ExportFormat format,
            string baseFileName,
            CancellationToken cancellationToken
        );
    }
}
