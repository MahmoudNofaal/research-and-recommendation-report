namespace Application.Abstractions.Auth
{
    /// <summary>
    /// Resolves external-provider sign-in callbacks after Web has completed the
    /// provider-specific redirect/challenge. It contains no ASP.NET Core
    /// authentication properties, claims principals, or provider SDK types.
    /// </summary>
    public interface IExternalAuthenticationService
    {
        Task<IReadOnlyList<ExternalAuthenticationProvider>> GetAvailableProvidersAsync
            (CancellationToken cancellationToken);

        /// <summary>
        /// Finds the local account linked to the provider key, or creates and
        /// links one when the callback represents a first-time sign-in.
        /// </summary>
        Task<IdentityOperationResult<AuthenticatedUser>> CompleteSignInAsync
        (
            ExternalAuthenticationCallback callback,
            CancellationToken cancellationToken
        );
    }

    public sealed record ExternalAuthenticationProvider(string Name, string DisplayName);

    /// <summary>
    /// Normalized, verified facts extracted by the Web callback adapter from an
    /// external provider. <see cref="ProviderKey"/> is the stable provider-side
    /// subject identifier, never a display name or email address.
    /// </summary>
    public sealed record ExternalAuthenticationCallback
    (
        string Provider,
        string ProviderKey,
        string Email,
        string DisplayName,
        bool IsEmailVerified
    );
}
