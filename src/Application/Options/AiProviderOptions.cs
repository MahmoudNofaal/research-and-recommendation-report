using System.ComponentModel.DataAnnotations;

namespace Application.Options
{
    public sealed class AiProviderOptions
    {
        public bool Enabled { get; init; }

        [StringLength(100)]
        public string ModelName { get; init; } = string.Empty;

        [StringLength(500)]
        public string ApiKey { get; init; } = string.Empty;
    }
}
