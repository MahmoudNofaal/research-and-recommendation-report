using Domain.Errors;

namespace Domain.Exceptions
{
    /// <summary>
    /// Raised when an aggregate in the report bounded context is asked to make an
    /// illegal state transition: generating on an already-terminal generation run,
    /// modifying a deleted report, or toggling a template/preset flag that is
    /// already set.
    /// </summary>
    public sealed class InvalidReportStateException : ReportDomainException
    {
        public InvalidReportStateException(DomainError error)
            : base(error)
        {
        }
    }
}
