using Domain.Common;
using Domain.Enums;

namespace Domain.ValueObjects
{
    /// <summary>
    /// A single quality concern raised about a generated report's content, such
    /// as a missing required section or thin comparison coverage. A collection of
    /// these, together with the numeric <see cref="ReportQualityScore"/>, is what
    /// <see cref="Domain.Services.ReportQualityDomainService"/> produces and what
    /// <c>GeneratedReport</c> uses to compute its <see cref="Enums.ReportStatus"/>.
    /// </summary>
    public sealed class QualityWarning : ValueObject
    {
        public const int MaxMessageLength = 500;

        public string Code { get; }

        public string Message { get; }

        public QualityWarningSeverity Severity { get; }

        private QualityWarning(string code, string message, QualityWarningSeverity severity)
        {
            Code = code;
            Message = message;
            Severity = severity;
        }

        public static QualityWarning Create(string code, string message, QualityWarningSeverity severity)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(code);
            ArgumentException.ThrowIfNullOrWhiteSpace(message);

            var trimmedMessage = message.Trim();
            if (trimmedMessage.Length > MaxMessageLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(message),
                    $"Quality warning message cannot exceed {MaxMessageLength} characters.");
            }

            return new QualityWarning(code.Trim(), trimmedMessage, severity);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Code;
            yield return Message;
            yield return Severity;
        }

        public override string ToString() => $"[{Severity}] {Code}: {Message}";
    }
}
