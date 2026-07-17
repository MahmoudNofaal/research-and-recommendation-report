using Domain.Errors;

namespace Application.Common.Errors
{
    /// <summary>
    /// How an <see cref="ApplicationError"/> should be treated by callers that
    /// need to react differently to different kinds of expected failure —
    /// principally Web's <c>ProblemDetailsMapper</c>/<c>MapApplicationResultFilter</c>,
    /// which use this to pick an HTTP status code, and
    /// <c>Application.Common.Behaviors.AuthorizationBehavior{TRequest, TResponse}</c>,
    /// which short-circuits the pipeline with <see cref="Unauthorized"/> before
    /// a handler ever runs.
    /// </summary>
    public enum ApplicationErrorType
    {
        /// <summary>A generic failure with no more specific classification.</summary>
        Failure = 0,

        /// <summary>The request itself, or a business rule it violates, is invalid.</summary>
        Validation = 1,

        /// <summary>The requested resource does not exist (or does not exist for this user).</summary>
        NotFound = 2,

        /// <summary>No authenticated user is associated with the request.</summary>
        Unauthorized = 3,

        /// <summary>An authenticated user exists but may not act on the resource.</summary>
        Forbidden = 4,

        /// <summary>The request conflicts with the resource's current state.</summary>
        Conflict = 5
    }

    /// <summary>
    /// A machine-readable code paired with a human-readable message and a
    /// broad classification, describing why an Application use case could not
    /// complete. This is the Application-layer counterpart to Domain's
    /// <see cref="DomainError"/>: a <see cref="Domain.Exceptions.DomainException"/>
    /// raised while mutating an aggregate is caught at the command handler
    /// boundary and translated into one of these via
    /// <see cref="FromDomainError"/>, so Web only ever has to reason about one
    /// error shape (<see cref="Application.Common.Models.Result"/> plus this
    /// type), regardless of whether the failure originated in Domain or in
    /// Application itself.
    /// </summary>
    public sealed record ApplicationError(string Code, string Message, ApplicationErrorType Type)
    {
        public static ApplicationError Failure(string code, string message)
            => new(code, message, ApplicationErrorType.Failure);

        public static ApplicationError Validation(string code, string message)
            => new(code, message, ApplicationErrorType.Validation);

        public static ApplicationError NotFound(string code, string message)
            => new(code, message, ApplicationErrorType.NotFound);

        public static ApplicationError Unauthorized(string code, string message)
            => new(code, message, ApplicationErrorType.Unauthorized);

        public static ApplicationError Forbidden(string code, string message)
            => new(code, message, ApplicationErrorType.Forbidden);

        public static ApplicationError Conflict(string code, string message)
            => new(code, message, ApplicationErrorType.Conflict);

        /// <summary>
        /// Wraps a Domain-level business rule violation as an Application
        /// error, preserving its original <see cref="DomainError.Code"/> and
        /// <see cref="DomainError.Message"/> verbatim. Domain errors are almost
        /// always the caller's fault (an invalid request shape, or an
        /// attempted illegal state transition), so they default to
        /// <see cref="ApplicationErrorType.Validation"/> unless the caller
        /// knows better.
        /// </summary>
        public static ApplicationError FromDomainError(DomainError domainError, ApplicationErrorType type = ApplicationErrorType.Validation)
        {
            ArgumentNullException.ThrowIfNull(domainError);

            return new ApplicationError(domainError.Code, domainError.Message, type);
        }

        public override string ToString()
            => $"[{Type}] {Code}: {Message}";
    }
}
