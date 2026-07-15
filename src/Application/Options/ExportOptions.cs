using System.ComponentModel.DataAnnotations;

namespace Application.Options
{
    public sealed class ExportOptions
    {
        public const string SectionName = "Exports";

        [Required]
        public string DefaultFormat { get; init; } = "Markdown";

        [Range(1, 100)]
        public int RetainExportRecordsDays { get; init; } = 30;

        public bool EnableMarkdown { get; init; } = true;

        public bool EnableHtml { get; init; } = true;

        public bool EnablePdf { get; init; } = true;

        public bool EnableDocx { get; init; } = true;
    }
}
