using Application.Common.Errors;

namespace Application.Common.Models
{
    /// <summary>
    /// The outcome of a use case that, on success, hands back a
    /// <typeparamref name="TValue"/> alongside the fact that it succeeded.
    /// Returned by every <c>IQueryHandler</c> and by any <c>ICommandHandler</c>
    /// declared against <c>ICommand{TResponse}</c> rather than the plain,
    /// non-generic <c>ICommand</c>. See <see cref="Result"/> for the sibling
    /// used when a command has nothing further to report than success or
    /// failure.
    /// </summary>
    /// <typeparam name="TValue">The type of data returned on success.</typeparam>
    public sealed class Result<TValue> : Result, IResult<Result<TValue>>
    {
        private readonly TValue? _value;

        private Result(TValue value)
            : base(true, error: null)
        {
            _value = value;
        }

        private Result(ApplicationError error)
            : base(false, error)
        {
            _value = default;
        }

        /// <summary>
        /// The successful use case's return value. Only ever read after
        /// checking <see cref="Result.IsSuccess"/> first — reading this on a
        /// failed result is a programming error, not an expected business
        /// outcome, so it throws rather than silently returning a default.
        /// </summary>
        public TValue Value
            => IsSuccess
                ? _value!
                : throw new InvalidOperationException
                (
                    $"Cannot access {nameof(Value)} on a failed {nameof(Result<TValue>)}. Check {nameof(IsSuccess)} first."
                );

        public static Result<TValue> Success(TValue value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return new Result<TValue>(value);
        }

        public static new Result<TValue> Failure(ApplicationError error)
        {
            ArgumentNullException.ThrowIfNull(error);

            return new Result<TValue>(error);
        }

        /// <summary>Lets a handler simply <c>return value;</c> on the happy path.</summary>
        public static implicit operator Result<TValue>(TValue value)
            => Success(value);
    }
}
