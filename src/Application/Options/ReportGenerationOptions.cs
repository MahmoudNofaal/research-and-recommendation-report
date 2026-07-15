using System.ComponentModel.DataAnnotations;

namespace Application.Options
{
    public sealed class ReportGenerationOptions
    {
        public const string SectionName = "ReportGeneration";

        [Range(1, 8)]
        public int MaximumTopics { get; init; } = 8;

        [Range(1, 20)]
        public int MaximumCriteria { get; init; } = 12;

        [Range(500, 50000)]
        public int MaximumMarkdownCharacters { get; init; } = 20000;

        public bool EnableLiveResearch { get; init; } = true;

        public bool EnableVisualExplanations { get; init; } = true;
    }
}
