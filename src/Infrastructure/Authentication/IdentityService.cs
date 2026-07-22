using Application.Abstractions.Auth;
using Domain.ValueObjects;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Identity;
using ApplicationIdentityError = Application.Abstractions.Auth.IdentityError;

namespace Infrastructure.Authentication
{
    /// <summary>
    /// ASP.NET Core Identity implementation of the local-account Application port.
    /// Identity types are translated here and never escape into Application.
    /// </summary>
    internal sealed class IdentityService : IIdentityService
    {
        private const string UserNotFoundCode = "Identity.UserNotFound";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityOperationResult<AuthenticatedUser>> RegisterAsync
        (
            RegistrationRequest request,
            CancellationToken cancellationToken
        )
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                DisplayName = request.DisplayName,
                CreatedAtUtc = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return IdentityOperationResult<AuthenticatedUser>.Failure(IdentityErrorMapper.ToErrors(result));
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return IdentityOperationResult<AuthenticatedUser>.Success(ToAuthenticatedUser(user));
        }

        public async Task<IdentityOperationResult<AuthenticatedUser>> PasswordSignInAsync
        (
            PasswordSignInRequest request,
            CancellationToken cancellationToken
        )
        {
            var result = await _signInManager.PasswordSignInAsync
            (
                request.Email,
                request.Password,
                request.IsPersistent,
                lockoutOnFailure: true
            );

            if (!result.Succeeded)
            {
                return IdentityOperationResult<AuthenticatedUser>.Failure(ToSignInError(result));
            }

            var user = await _userManager.FindByEmailAsync(request.Email);

            return user is null
                ? IdentityOperationResult<AuthenticatedUser>.Failure(UserNotFoundError())
                : IdentityOperationResult<AuthenticatedUser>.Success(ToAuthenticatedUser(user));
        }

        public Task SignOutAsync(CancellationToken cancellationToken)
            => _signInManager.SignOutAsync();

        public async Task<AuthenticatedUser?> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(email);

            return user is null ? null : ToAuthenticatedUser(user);
        }

        public async Task<IdentityOperationResult<AccountProfile>> GetProfileAsync
        (
            UserId userId,
            CancellationToken cancellationToken
        )
        {
            var user = await FindUserAsync(userId);

            return user is null
                ? IdentityOperationResult<AccountProfile>.Failure(UserNotFoundError())
                : IdentityOperationResult<AccountProfile>.Success(new AccountProfile(user.DisplayName, user.Email ?? string.Empty, user.EmailConfirmed));
        }

        public async Task<IdentityOperationResult> UpdateProfileAsync
        (
            UserId userId,
            UpdateAccountProfileRequest request,
            CancellationToken cancellationToken
        )
        {
            var user = await FindUserAsync(userId);
            if (user is null)
            {
                return IdentityOperationResult.Failure(UserNotFoundError());
            }

            user.DisplayName = request.DisplayName;

            return IdentityErrorMapper.ToOperationResult(await _userManager.UpdateAsync(user));
        }

        public async Task<IdentityOperationResult<string>> GenerateEmailConfirmationTokenAsync
        (
            UserId userId,
            CancellationToken cancellationToken
        )
        {
            var user = await FindUserAsync(userId);
            if (user is null)
            {
                return IdentityOperationResult<string>.Failure(UserNotFoundError());
            }

            return IdentityOperationResult<string>.Success(await _userManager.GenerateEmailConfirmationTokenAsync(user));
        }

        public async Task<IdentityOperationResult> ConfirmEmailAsync
        (
            UserId userId,
            string token,
            CancellationToken cancellationToken
        )
        {
            var user = await FindUserAsync(userId);

            return user is null
                ? IdentityOperationResult.Failure(UserNotFoundError())
                : IdentityErrorMapper.ToOperationResult(await _userManager.ConfirmEmailAsync(user, token));
        }

        public async Task<IdentityOperationResult<PasswordResetToken>> GeneratePasswordResetTokenAsync
        (
            string email,
            CancellationToken cancellationToken
        )
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                // The handler returns the same generic response and sends no email.
                return IdentityOperationResult<PasswordResetToken>.Success
                (
                    new PasswordResetToken(null, string.Empty)
                );
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            return IdentityOperationResult<PasswordResetToken>.Success
            (
                new PasswordResetToken(UserId.From(user.Id), token)
            );
        }

        public async Task<IdentityOperationResult> ResetPasswordAsync
        (
            PasswordResetRequest request,
            CancellationToken cancellationToken
        )
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is null)
            {
                return IdentityOperationResult.Failure
                (
                    new ApplicationIdentityError
                    (
                        "Identity.InvalidResetRequest",
                        "The password reset request is invalid."
                    )
                );
            }

            return IdentityErrorMapper.ToOperationResult
            (
                await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword)
            );
        }

        public async Task<IdentityOperationResult> ChangePasswordAsync
        (
            UserId userId,
            ChangePasswordRequest request,
            CancellationToken cancellationToken
        )
        {
            var user = await FindUserAsync(userId);

            return user is null
                ? IdentityOperationResult.Failure(UserNotFoundError())
                : IdentityErrorMapper.ToOperationResult(await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword));
        }

        private Task<ApplicationUser?> FindUserAsync(UserId userId)
            => _userManager.FindByIdAsync(userId.Value.ToString());

        private static AuthenticatedUser ToAuthenticatedUser(ApplicationUser user)
            => new(UserId.From(user.Id), user.Email ?? string.Empty, user.DisplayName, user.EmailConfirmed);

        private static ApplicationIdentityError ToSignInError(SignInResult result)
            => result.IsLockedOut
                ? new ApplicationIdentityError("Identity.LockedOut", "The account is temporarily locked.")
                : result.IsNotAllowed
                    ? new ApplicationIdentityError("Identity.NotAllowed", "Sign-in is not allowed for this account.")
                    : result.RequiresTwoFactor
                        ? new ApplicationIdentityError("Identity.TwoFactorRequired", "Two-factor authentication is required.")
                        : new ApplicationIdentityError("Identity.InvalidCredentials", "The email or password is incorrect.");

        private static ApplicationIdentityError UserNotFoundError()
            => new(UserNotFoundCode, "The account could not be found.");

    }
}
