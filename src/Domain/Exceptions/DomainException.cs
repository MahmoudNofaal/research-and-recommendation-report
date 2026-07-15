using Domain.Errors;

namespace Domain.Exceptions
{
    /// <summary>
    /// Base type for every exception raised when the domain refuses to enter or
    /// perform an invalid business state. Always carries the structured
    /// <see cref="DomainError"/> that caused the failure so callers can react to
    /// the error code rather than parsing the exception message.
    /// </summary>
    public abstract class DomainException : Exception
    {
        protected DomainException(DomainError error)
            : base(error.Message)
        {
            Error = error;
        }

        public DomainError Error { get; }
    }
}
