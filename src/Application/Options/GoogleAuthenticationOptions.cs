using System.ComponentModel.DataAnnotations;

namespace Application.Options
{
    /// <summary>
    /// Google OAuth 2.0 settings bound from the <c>Authentication:Google</c>
    /// configuration section. The <see cref="ClientId"/> and
    /// <see cref="ClientSecret"/> should be supplied via user secrets,
    /// environment variables, or deployment secret storage rather than
    /// committed appsettings files.
    /// </summary>
    public sealed class GoogleAuthenticationOptions
    {
        public const string SectionName = "Authentication:Google";

        /// <summary>
        /// When <c>false</c>, the Google authentication scheme is not
        /// registered at all — useful for environments that do not have
        /// OAuth credentials configured yet.
        /// </summary>
        public bool Enabled { get; init; }

        /// <summary>
        /// OAuth 2.0 Client ID obtained from the Google Cloud Console.
        /// Required when <see cref="Enabled"/> is <c>true</c>.
        /// </summary>
        [Required]
        public string ClientId { get; init; } = string.Empty;

        /// <summary>
        /// OAuth 2.0 Client Secret obtained from the Google Cloud Console.
        /// Required when <see cref="Enabled"/> is <c>true</c>.
        /// </summary>
        [Required]
        public string ClientSecret { get; init; } = string.Empty;

        /// <summary>
        /// The relative path the middleware redirects to after Google returns
        /// an authorization code. Defaults to the standard ASP.NET Core
        /// Google handler callback path.
        /// </summary>
        public string CallbackPath { get; init; } = "/signin-google";
    }
}
