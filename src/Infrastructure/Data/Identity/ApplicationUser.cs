using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Identity
{
    /// <summary>
    /// The persisted identity record for an authenticated user. This is an
    /// Infrastructure/Identity concern only — Domain and Application never
    /// reference this type, working instead with the lightweight
    /// <see cref="Domain.ValueObjects.UserId"/> that this user's
    /// <see cref="IdentityUser{TKey}.Id"/> converts to at the Web boundary
    /// (see <c>CurrentUserService</c>).
    /// </summary>
    public sealed class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// The name shown back to the user throughout the app (dashboard
        /// greeting, report history, etc.), distinct from their login email.
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// When the account was created. Simple audit fact for this Identity
        /// record specifically — unrelated to <c>AuditableEntity{TId}</c>,
        /// which this type does not use.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }
    }
}