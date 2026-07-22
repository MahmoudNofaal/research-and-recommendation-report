using Application.Abstractions.Auth;
using Domain.ValueObjects;
using Infrastructure.Data.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using ApplicationIdentityError = Application.Abstractions.Auth.IdentityError;

namespace Infrastructure.Authentication
{
    /// <summary>
    /// Turns a verified external-provider callback into a linked local account and
    /// cookie sign-in. Web owns the redirect challenge and callback parsing.
    /// </summary>
    internal sealed class ExternalAuthenticationService : IExternalAuthenticationService
    {
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ExternalAuthenticationService
        (
            IAuthenticationSchemeProvider schemeProvider,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
        )
        {
            _schemeProvider = schemeProvider;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IReadOnlyList<ExternalAuthenticationProvider>> GetAvailableProvidersAsync
            (CancellationToken cancellationToken)
        {
            var schemes = await _schemeProvider.GetAllSchemesAsync();

            return schemes
                .Where(scheme => !string.IsNullOrWhiteSpace(scheme.DisplayName))
                .Select(scheme => new ExternalAuthenticationProvider(scheme.Name, scheme.DisplayName!))
                .OrderBy(provider => provider.DisplayName, StringComparer.Ordinal)
                .ToArray();
        }

        public async Task<IdentityOperationResult<AuthenticatedUser>> CompleteSignInAsync
        (
            ExternalAuthenticationCallback callback,
            CancellationToken cancellationToken
        )
        {
            if (string.IsNullOrWhiteSpace(callback.Provider)
                || string.IsNullOrWhiteSpace(callback.ProviderKey)
                || string.IsNullOrWhiteSpace(callback.Email))
            {
                return IdentityOperationResult<AuthenticatedUser>.Failure
                (
                    new ApplicationIdentityError
                    (
                        "Identity.InvalidExternalLogin",
                        "The external login callback is incomplete."
                    )
                );
            }

            var user = await _userManager.FindByLoginAsync(callback.Provider, callback.ProviderKey);
            if (user is null)
            {
                if (!callback.IsEmailVerified)
                {
                    return IdentityOperationResult<AuthenticatedUser>.Failure
                    (
                        new ApplicationIdentityError
                        (
                            "Identity.ExternalEmailNotVerified",
                            "The external provider did not verify the email address."
                        )
                    );
                }

                user = await _userManager.FindByEmailAsync(callback.Email);
                if (user is not null)
                {
                    if (!user.EmailConfirmed)
                    {
                        return IdentityOperationResult<AuthenticatedUser>.Failure
                        (
                            new ApplicationIdentityError
                            (
                                "Identity.ExternalLoginNotLinked",
                                "Confirm the existing account email before linking this external login."
                            )
                        );
                    }

                    var linkResult = await _userManager.AddLoginAsync(user, ToUserLoginInfo(callback));

                    if (!linkResult.Succeeded)
                    {
                        return IdentityOperationResult<AuthenticatedUser>
                            .Failure(IdentityErrorMapper
                            .ToErrors(linkResult));
                    }
                }
                else
                {
                    user = new ApplicationUser
                    {
                        UserName = callback.Email,
                        Email = callback.Email,
                        EmailConfirmed = true,
                        DisplayName = string.IsNullOrWhiteSpace(callback.DisplayName)
                            ? callback.Email : callback.DisplayName,
                        CreatedAtUtc = DateTime.UtcNow
                    };

                    var createResult = await _userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        return IdentityOperationResult<AuthenticatedUser>
                            .Failure(IdentityErrorMapper
                            .ToErrors(createResult));
                    }

                    var linkResult = await _userManager.AddLoginAsync(user, ToUserLoginInfo(callback));
                    if (!linkResult.Succeeded)
                    {
                        await _userManager.DeleteAsync(user);

                        return IdentityOperationResult<AuthenticatedUser>
                            .Failure(IdentityErrorMapper
                            .ToErrors(linkResult));
                    }
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: false);

            return IdentityOperationResult<AuthenticatedUser>.Success(ToAuthenticatedUser(user));
        }

        private static UserLoginInfo ToUserLoginInfo(ExternalAuthenticationCallback callback)
            => new(callback.Provider, callback.ProviderKey, callback.Provider);

        private static AuthenticatedUser ToAuthenticatedUser(ApplicationUser user)
            => new(UserId.From(user.Id), user.Email ?? string.Empty, user.DisplayName, user.EmailConfirmed);

    }
}
