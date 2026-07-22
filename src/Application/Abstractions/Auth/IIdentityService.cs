using Domain.ValueObjects;

namespace Application.Abstractions.Auth
{
    /// <summary>
    /// Framework-independent account and local-credential operations required by
    /// Application authentication features. The Infrastructure implementation may
    /// use ASP.NET Core Identity, but callers must not depend on Identity types.
    /// </summary>
    public interface IIdentityService
    {
        Task<IdentityOperationResult<AuthenticatedUser>> RegisterAsync
        (
            RegistrationRequest request,
            CancellationToken cancellationToken
        );

        Task<IdentityOperationResult<AuthenticatedUser>> PasswordSignInAsync
        (
            PasswordSignInRequest request,
            CancellationToken cancellationToken
        );

        Task SignOutAsync(CancellationToken cancellationToken);

        Task<AuthenticatedUser?> FindByEmailAsync
        (
            string email,
            CancellationToken cancellationToken
        );

        Task<IdentityOperationResult<AccountProfile>> GetProfileAsync
        (
            UserId userId,
            CancellationToken cancellationToken
        );

        Task<IdentityOperationResult> UpdateProfileAsync
        (
            UserId userId,
            UpdateAccountProfileRequest request,
            CancellationToken cancellationToken
        );

        Task<IdentityOperationResult<string>> GenerateEmailConfirmationTokenAsync
        (
            UserId userId,
            CancellationToken cancellationToken
        );

        Task<IdentityOperationResult> ConfirmEmailAsync
        (
            UserId userId,
            string token,
            CancellationToken cancellationToken
        );

        /// <summary>
        /// Returns success even when the email is unknown. This prevents account
        /// enumeration in the forgot-password flow.
        /// </summary>
        Task<IdentityOperationResult<PasswordResetToken>> GeneratePasswordResetTokenAsync
        (
            string email,
            CancellationToken cancellationToken
        );

        Task<IdentityOperationResult> ResetPasswordAsync
        (
            PasswordResetRequest request,
            CancellationToken cancellationToken
        );

        Task<IdentityOperationResult> ChangePasswordAsync
        (
            UserId userId,
            ChangePasswordRequest request,
            CancellationToken cancellationToken
        );
    }

    public sealed record RegistrationRequest(string DisplayName, string Email, string Password);

    public sealed record PasswordSignInRequest(string Email, string Password, bool IsPersistent);

    public sealed record UpdateAccountProfileRequest(string DisplayName);

    public sealed record PasswordResetRequest(string Email, string Token, string NewPassword);

    public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

    public sealed record AuthenticatedUser
    (
        UserId UserId,
        string Email,
        string DisplayName,
        bool IsEmailConfirmed
    );

    public sealed record AccountProfile(string DisplayName, string Email, bool IsEmailConfirmed);

    /// <summary>
    /// A one-time token is deliberately kept separate from a user record. It is
    /// only passed to the email-composition flow and must never be logged.
    /// </summary>
    public sealed record PasswordResetToken(UserId? UserId, string Token);

    public sealed record IdentityError(string Code, string Description);

    public record IdentityOperationResult(bool Succeeded, IReadOnlyList<IdentityError> Errors)
    {
        public static IdentityOperationResult Success() => new(true, []);

        public static IdentityOperationResult Failure(params IdentityError[] errors) => new(false, errors);
    }

    public sealed record IdentityOperationResult<T>
    (
        bool Succeeded,
        T? Value,
        IReadOnlyList<IdentityError> Errors
    ) : IdentityOperationResult(Succeeded, Errors)
    {
        public static IdentityOperationResult<T> Success(T value)
            => new(true, value, []);

        public new static IdentityOperationResult<T> Failure(params IdentityError[] errors)
            => new(false, default, errors);
    }
}
