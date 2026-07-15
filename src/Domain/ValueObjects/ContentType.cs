using Domain.Common;
using Domain.Enums;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The MIME content type of an exported file. What MIME type corresponds to
    /// which <see cref="ExportFormat"/> is an intrinsic fact about the format
    /// itself (Markdown content <em>is</em> "text/markdown"), so this mapping
    /// lives in the Domain rather than in an Infrastructure export renderer.
    /// </summary>
    public sealed class ContentType : ValueObject
    {
        public string Value { get; }

        private ContentType(string value)
        {
            Value = value;
        }

        public static ContentType ForFormat(ExportFormat format) => format switch
        {
            ExportFormat.Markdown => new ContentType("text/markdown"),
            ExportFormat.Html => new ContentType("text/html"),
            ExportFormat.Pdf => new ContentType("application/pdf"),
            ExportFormat.Docx => new ContentType("application/vnd.openxmlformats-officedocument.wordprocessingml.document"),
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported export format.")
        };

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString()
            => Value;
    }
}
