using Application.Abstractions.Auth;
using Application.Common.Errors;
using Application.Common.Models;
using MediatR;

namespace Application.Common.Behaviors
{
    /// <summary>
    /// Rejects any command or query with an
    /// <see cref="ApplicationErrorType.Unauthorized"/> failure before its
    /// handler ever runs, if the current request has no signed-in user at
    /// all. Every use case in this application requires authentication (see
    /// architecture-plan.md, "Security Design") — there is no notion of an
    /// anonymous command or query — so this behavior applies uniformly rather
    /// than needing a per-request opt-in marker.
    ///
    /// This is deliberately only an authentication check. <em>Ownership</em> —
    /// whether the signed-in user may act on the specific report/export/etc.
    /// being referenced — is a data-shaped concern that only the handler,
    /// loading through a repository port scoped to the user's id, can answer;
    /// see <see cref="ReportApplicationErrors"/> for how that failure
    /// surfaces instead (as "not found," never as a distinguishable
    /// "forbidden," so as not to reveal that a resource exists at all).
    /// </summary>
    public sealed class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : Result, IResult<TResponse>
    {
        private readonly ICurrentUserService _currentUserService;

        public AuthorizationBehavior(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is null)
            {
                return Task.FromResult
                (
                    TResponse.Failure
                    (
                        ApplicationError.Unauthorized
                        (
                            "Request.Unauthenticated",
                            "You must be signed in to do this."
                        )
                    )
                );
            }

            return next(cancellationToken);
        }
    }
}
