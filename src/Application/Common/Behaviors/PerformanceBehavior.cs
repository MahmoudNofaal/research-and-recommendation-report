using System.Diagnostics;
using Application.Abstractions.Auth;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors
{
    /// <summary>
    /// Flags commands/queries that take longer than expected to complete.
    /// Report-generation work has its own dedicated
    /// <see cref="Domain.ValueObjects.GenerationTiming"/>/token-usage tracking
    /// on <c>ReportGenerationRun</c>, so this threshold is aimed at everything
    /// else in the pipeline — dashboard queries, history search, exports —
    /// where a slow response is very likely an emerging performance problem
    /// rather than an expected, provider-bound wait.
    /// </summary>
    public sealed class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private static readonly TimeSpan SlowRequestThreshold = TimeSpan.FromMilliseconds(500);

        private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly ICurrentUserService _currentUserService;

        public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger, ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            var response = await next(cancellationToken);

            stopwatch.Stop();

            if (stopwatch.Elapsed > SlowRequestThreshold)
            {
                _logger.LogWarning
                (
                    "Long-running request: {RequestName} took {ElapsedMilliseconds}ms for user {UserId}",
                    typeof(TRequest).Name,
                    stopwatch.ElapsedMilliseconds,
                    _currentUserService.UserId?.ToString() ?? "anonymous"
                );
            }

            return response;
        }
    }
}
