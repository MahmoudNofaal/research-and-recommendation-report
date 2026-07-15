using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The error code and message recorded when a <c>ReportGenerationRun</c>
    /// fails. A run either has no error detail (still pending, in progress, or
    /// succeeded) or has exactly one, fully populated — there is no representation
    /// for a partially recorded failure.
    /// </summary>
    public sealed class ErrorDetail : ValueObject
    {
        public const int MaxMessageLength = 1000;

        public string ErrorCode { get; }

        public string ErrorMessage { get; }

        private ErrorDetail(string errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public static ErrorDetail Create(string errorCode, string errorMessage)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(errorCode);
            ArgumentException.ThrowIfNullOrWhiteSpace(errorMessage);

            var trimmedMessage = errorMessage.Trim();
            if (trimmedMessage.Length > MaxMessageLength)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(errorMessage),
                    $"Error message cannot exceed {MaxMessageLength} characters.");
            }

            return new ErrorDetail(errorCode.Trim(), trimmedMessage);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return ErrorCode;
            yield return ErrorMessage;
        }

        public override string ToString() => $"{ErrorCode}: {ErrorMessage}";
    }
}
