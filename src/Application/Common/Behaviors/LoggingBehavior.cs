using Application.Abstractions.Auth;
using Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors
{
    /// <summary>
    /// Logs that a command or query started and how it concluded — success,
    /// or the specific <see cref="Errors.ApplicationError"/> code if it
    /// failed — tagged with the current user, so support/ops can trace a
    /// report end to end from wizard submission through generation and
    /// export. Deliberately logs only the request's type name, never its
    /// full contents: a <c>ReportRequest</c> can carry free-text
    /// supplementary notes the user may not want duplicated into application
    /// logs.
    /// </summary>
    public sealed class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : Result
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        private readonly ICurrentUserService _currentUserService;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId?.ToString() ?? "anonymous";

            _logger.LogInformation("Handling {RequestName} for user {UserId}", requestName, userId);

            var response = await next(cancellationToken);

            if (response.IsFailure)
            {
                _logger.LogWarning
                (
                    "{RequestName} for user {UserId} failed with {ErrorCode}: {ErrorMessage}",
                    requestName,
                    userId,
                    response.Error!.Code,
                    response.Error!.Message
                );
            }
            else
            {
                _logger.LogInformation("Handled {RequestName} for user {UserId}", requestName, userId);
            }

            return response;
        }
    }
}
