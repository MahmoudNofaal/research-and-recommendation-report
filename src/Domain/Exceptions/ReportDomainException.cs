using Domain.Errors;

namespace Domain.Exceptions
{
    /// <summary>
    /// Base type for domain exceptions raised within the report-generation
    /// bounded context (report requests, generated reports, generation runs,
    /// templates, presets, and exports).
    /// </summary>
    public abstract class ReportDomainException : DomainException
    {
        protected ReportDomainException(DomainError error)
            : base(error)
        {
        }
    }
}
