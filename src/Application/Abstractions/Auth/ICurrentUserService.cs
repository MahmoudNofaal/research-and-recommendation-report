using Domain.ValueObjects;

namespace Application.Abstractions.Auth
{
    /// <summary>
    /// Read-only access to the identity of the user making the current
    /// request, resolved from <c>HttpContext</c> by Web's own
    /// <c>CurrentUserService</c> adapter (see architecture-plan.md, "Web
    /// Responsibilities"). Application depends only on this abstraction —
    /// never on <c>HttpContext</c> or <c>ClaimsPrincipal</c> directly — so
    /// every use case, and every pipeline behavior in
    /// <c>Application.Common.Behaviors</c>, stays testable without an HTTP
    /// pipeline.
    /// </summary>
    public interface ICurrentUserService
    {
        /// <summary>
        /// The signed-in user's id, or <see langword="null"/> if the current
        /// request has no authenticated user — the condition
        /// <c>AuthorizationBehavior{TRequest, TResponse}</c> checks before
        /// letting any command or query reach its handler.
        /// </summary>
        UserId? UserId { get; }
    }
}
