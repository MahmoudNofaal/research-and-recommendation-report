using System.ComponentModel.DataAnnotations;

namespace Application.Options
{
    /// <summary>
    /// Application-level AI generation settings. API keys should be supplied via
    /// user secrets, environment variables, or deployment secret storage rather
    /// than committed appsettings files.
    /// </summary>
    public sealed class AiOptions
    {
        public const string SectionName = "Ai";

        [Required]
        public string DefaultProvider { get; init; } = "Fake";

        [Range(1, 10)]
        public int MaxRetries { get; init; } = 2;

        [Range(5, 300)]
        public int TimeoutSeconds { get; init; } = 60;

        public AiProviderOptions Groq { get; init; } = new();

        public AiProviderOptions Gemini { get; init; } = new();
    }
}
