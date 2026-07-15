using Domain.Errors;

namespace Domain.Exceptions
{
    /// <summary>
    /// Raised when a <c>ReportRequest</c> is asked to enter or maintain an invalid
    /// shape: too few or too many topics, duplicate topic or criterion names, too
    /// few criteria, or an edit attempted after submission.
    /// </summary>
    public sealed class InvalidReportRequestException : ReportDomainException
    {
        public InvalidReportRequestException(DomainError error)
            : base(error)
        {
        }
    }
}
