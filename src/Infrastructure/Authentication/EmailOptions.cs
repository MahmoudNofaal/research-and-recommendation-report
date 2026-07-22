namespace Infrastructure.Authentication
{
    /// <summary>
    /// SMTP settings. Credentials must be supplied via user secrets, environment
    /// variables, or deployment secret storage.
    /// </summary>
    public sealed class EmailOptions
    {
        public const string SectionName = "Authentication:Email";

        public string Host { get; init; } = string.Empty;

        public int Port { get; init; } = 587;

        public string FromAddress { get; init; } = string.Empty;

        public string FromName { get; init; } = "Research Report Generator";

        public string Username { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;

        public bool EnableSsl { get; init; } = true;
    }
}
