namespace Domain.Enums
{
    /// <summary>
    /// The file formats a generated report can be exported as. Markdown is the
    /// canonical form; every other format is derived from it by Infrastructure.
    /// </summary>
    public enum ExportFormat
    {
        Markdown = 0,
        Html = 1,
        Pdf = 2,
        Docx = 3
    }
}
