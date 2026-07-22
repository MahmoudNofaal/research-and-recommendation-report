using Application.Abstractions.Auth;
using Microsoft.AspNetCore.Identity;
using ApplicationIdentityError = Application.Abstractions.Auth.IdentityError;

namespace Infrastructure.Authentication
{
    /// <summary>
    /// Single translation point between ASP.NET Core Identity errors and the
    /// framework-independent error values returned by Application contracts.
    /// </summary>
    internal static class IdentityErrorMapper
    {
        public static IdentityOperationResult ToOperationResult(IdentityResult result)
            => result.Succeeded
                ? IdentityOperationResult.Success()
                : IdentityOperationResult.Failure(ToErrors(result));

        public static ApplicationIdentityError[] ToErrors(IdentityResult result)
            => result.Errors.Select(error => new ApplicationIdentityError(error.Code, error.Description)).ToArray();
    }
}
