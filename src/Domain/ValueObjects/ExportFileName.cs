using Domain.Common;
using Domain.Enums;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The file name of a single export, guaranteed to contain only characters
    /// valid in a file name and to carry the extension appropriate for its
    /// export format. The base name itself (typically derived from the report
    /// title) is built by an Application-level naming policy that knows about
    /// sanitizing user-authored titles; this value object only enforces the
    /// domain-level shape rule once a candidate name is supplied.
    /// </summary>
    public sealed class ExportFileName : ValueObject
    {
        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

        public string Value { get; }

        private ExportFileName(string value)
        {
            Value = value;
        }

        public static ExportFileName Create(string baseFileName, ExportFormat format)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(baseFileName);

            var trimmed = baseFileName.Trim();
            if (trimmed.IndexOfAny(InvalidFileNameChars) >= 0)
            {
                throw new ArgumentException(
                    "Export file name contains characters that are not valid in a file name.",
                    nameof(baseFileName));
            }

            var expectedExtension = ExtensionFor(format);
            var fileName = trimmed.EndsWith(expectedExtension, StringComparison.OrdinalIgnoreCase)
                ? trimmed
                : trimmed + expectedExtension;

            return new ExportFileName(fileName);
        }

        private static string ExtensionFor(ExportFormat format) => format switch
        {
            ExportFormat.Markdown => ".md",
            ExportFormat.Html => ".html",
            ExportFormat.Pdf => ".pdf",
            ExportFormat.Docx => ".docx",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, "Unsupported export format.")
        };

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;
    }
}
