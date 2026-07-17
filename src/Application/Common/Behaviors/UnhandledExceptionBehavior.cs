using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors
{
    /// <summary>
    /// The outermost link in the pipeline: logs and re-throws any exception
    /// that escapes every other behavior or the handler itself. This is only
    /// ever an unexpected failure (infrastructure outage, programming error) —
    /// expected business-rule violations never reach here, because handlers
    /// catch their own <see cref="Domain.Exceptions.DomainException"/>s and
    /// translate them into a failed <see cref="Models.Result"/> instead of
    /// letting them throw (see architecture-plan.md, "Error Handling"). This
    /// behavior exists purely to guarantee the failure is logged with the
    /// request that triggered it, before Web's exception middleware turns it
    /// into a generic error page.
    /// </summary>
    public sealed class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

        public UnhandledExceptionBehavior(ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next(cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                _logger.LogError(exception, "Unhandled exception while processing {RequestName}", typeof(TRequest).Name);

                throw;
            }
        }
    }
}
