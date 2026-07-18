using System.Text;
using System.Text.RegularExpressions;
using Domain.ValueObjects;

namespace Application.Policies
{
    /// <summary>
    /// Turns a <see cref="ReportTitle"/> into the sanitized, extension-free
    /// base file name <see cref="ExportFileName.Create"/> expects — the
    /// "Application-level naming policy" that value object's own remarks
    /// defer to, and the <c>baseFileName</c>
    /// <c>IReportExportCoordinator.ExportAsync</c> is handed.
    /// </summary>
    public static class ExportFileNamePolicy
    {
        private const string FallbackName = "report";

        private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
        private static readonly Regex CollapsibleSeparators = new(@"[\s-]+", RegexOptions.Compiled);

        /// <summary>
        /// <paramref name="version"/> is appended as a <c>-v{n}</c> suffix for
        /// every version after the first, so a regenerated report's exports
        /// stay distinguishable on disk — matching the version transparency
        /// the rest of the UI already commits to (see
        /// ui-ux-specification.md, "11.5 Regeneration transparency"). The
        /// first version's file name is left unadorned, since that is the
        /// overwhelmingly common case and does not need a qualifier yet.
        /// </summary>
        public static string BuildBaseFileName(ReportTitle title, int version)
        {
            ArgumentNullException.ThrowIfNull(title);

            if (version < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(version), "Version must be at least 1.");
            }

            var sanitized = Sanitize(title.Value);

            return version == 1 ? sanitized : $"{sanitized}-v{version}";
        }

        private static string Sanitize(string value)
        {
            var builder = new StringBuilder(value.Length);

            foreach (var character in value)
            {
                builder.Append(Array.IndexOf(InvalidFileNameChars, character) >= 0 ? ' ' : character);
            }

            var collapsed = CollapsibleSeparators.Replace(builder.ToString(), "-").Trim('-');

            return collapsed.Length == 0 ? FallbackName : collapsed;
        }
    }
}
