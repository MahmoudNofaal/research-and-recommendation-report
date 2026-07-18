using Domain.Entities;
using Domain.Enums;

namespace Application.Abstractions.Exports
{
    /// <summary>
    /// Renders one <see cref="GeneratedReport"/> into the raw bytes of a
    /// single export <see cref="Format"/>. One implementation is registered
    /// per format in Infrastructure — <c>MarkdownReportExportRenderer</c>,
    /// <c>HtmlReportExportRenderer</c>, <c>PdfReportExportRenderer</c>,
    /// <c>DocxReportExportRenderer</c> — and
    /// <see cref="IReportExportCoordinator"/> picks the right one by matching
    /// <see cref="Format"/> against the requested <see cref="ExportFormat"/>.
    /// Markdown is canonical and every other renderer derives its output from
    /// it rather than from the report's other fields directly (see
    /// architecture-plan.md, "Export Design").
    /// </summary>
    public interface IReportExportRenderer
    {
        ExportFormat Format { get; }

        Task<byte[]> RenderAsync(GeneratedReport report, CancellationToken cancellationToken);
    }
}
