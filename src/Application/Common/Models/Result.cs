using Application.Common.Errors;

namespace Application.Common.Models
{
    /// <summary>
    /// Lets generic pipeline infrastructure — most importantly
    /// <see cref="Application.Common.Behaviors.ValidationBehavior{TRequest, TResponse}"/>
    /// and <see cref="Application.Common.Behaviors.AuthorizationBehavior{TRequest, TResponse}"/> —
    /// construct a new failed result of exactly the caller's response shape
    /// (the non-generic <see cref="Result"/>, or a specific closed
    /// <see cref="Result{TValue}"/>) when all it knows at compile time is the
    /// bare type parameter <typeparamref name="TSelf"/>, with no existing
    /// instance to copy from. Implemented by <see cref="Result"/> itself and
    /// by every closed <see cref="Result{TValue}"/>, using a C# static
    /// abstract interface member rather than reflection.
    /// </summary>
    public interface IResult<TSelf> where TSelf : IResult<TSelf>
    {
        static abstract TSelf Failure(ApplicationError error);
    }

    /// <summary>
    /// The outcome of a use case that mutates state but has no data of its own
    /// to hand back beyond whether it succeeded. Every <c>ICommandHandler</c>
    /// that isn't declared against <c>ICommand{TResponse}</c> returns this
    /// type instead of throwing for expected, business-rule-shaped failures —
    /// see <see cref="ApplicationError"/> for how a failure is described, and
    /// <see cref="Result{TValue}"/> for the value-carrying counterpart
    /// returned by every query and by commands that do return data.
    ///
    /// Unexpected failures (a database being unreachable, a programming
    /// error) are never represented here: those still throw, are logged and
    /// re-thrown by <c>UnhandledExceptionBehavior</c>, and are mapped by
    /// Web's exception middleware to a generic error page rather than a
    /// business-shaped result (see architecture-plan.md, "Error Handling").
    /// </summary>
    public class Result : IResult<Result>
    {
        protected Result(bool isSuccess, ApplicationError? error)
        {
            if (isSuccess && error is not null)
            {
                throw new ArgumentException("A successful result cannot carry an error.", nameof(error));
            }

            if (!isSuccess && error is null)
            {
                throw new ArgumentException("A failed result must carry an error.", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>Whether the use case completed without an expected business-rule failure.</summary>
        public bool IsSuccess { get; }

        /// <summary>The mirror of <see cref="IsSuccess"/>, for the common "if it failed" reading.</summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>
        /// The reason the use case failed. Always <see langword="null"/> when
        /// <see cref="IsSuccess"/> is <see langword="true"/>, and always
        /// populated when it is <see langword="false"/>.
        /// </summary>
        public ApplicationError? Error { get; }

        public static Result Success()
            => new(true, error: null);

        public static Result Failure(ApplicationError error)
        {
            ArgumentNullException.ThrowIfNull(error);

            return new Result(false, error);
        }
    }
}
