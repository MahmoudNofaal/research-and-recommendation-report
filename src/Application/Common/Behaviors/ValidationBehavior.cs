using Application.Common.Errors;
using Application.Common.Models;
using FluentValidation;
using MediatR;

namespace Application.Common.Behaviors
{
    /// <summary>
    /// Runs every registered <see cref="IValidator{T}"/> for the incoming
    /// command or query before its handler ever runs, and short-circuits with
    /// an <see cref="ApplicationErrorType.Validation"/> failure on the first
    /// violations found, rather than invoking the handler — and its Domain
    /// calls — against input that would just be rejected anyway. Requests
    /// with no registered validator pass straight through unchanged.
    /// </summary>
    public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : Result, IResult<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
            {
                return await next(cancellationToken);
            }

            var validationResults = await Task.WhenAll
            (
                _validators.Select(validator => validator.ValidateAsync(request, cancellationToken))
            );

            var failureMessages = validationResults
                .SelectMany(result => result.Errors)
                .Where(failure => failure is not null)
                .Select(failure => failure.ErrorMessage)
                .Distinct()
                .ToList();

            if (failureMessages.Count == 0)
            {
                return await next(cancellationToken);
            }

            return TResponse.Failure
            (
                ApplicationError.Validation("Request.ValidationFailed", string.Join(" ", failureMessages))
            );
        }
    }
}
