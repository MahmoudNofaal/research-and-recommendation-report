using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Abstractions.AI
{
    /// <summary>
    /// The composed system and user prompt text sent to an AI provider for a
    /// single generation attempt — already fully substituted with the
    /// request's topics, criteria, and audience by
    /// <see cref="IAiPromptComposer"/>. Carried here, rather than under a
    /// separate DTOs folder, because it exists purely to shape
    /// <see cref="IAiProvider.GenerateAsync"/>'s contract, exactly as
    /// <see cref="Domain.ValueObjects.ReportContent"/> bundles Markdown and
    /// summary together for one purpose.
    /// </summary>
    public sealed record AiGenerationRequest(string SystemPrompt, string UserPrompt);

    /// <summary>
    /// A provider's raw response to one <see cref="AiGenerationRequest"/>,
    /// together with the metadata a <c>ReportGenerationRun</c> needs to
    /// record the attempt. <see cref="TokenUsage"/> is the Domain value
    /// object itself, not a parallel DTO —
    /// <c>ReportGenerationRun.Succeed</c> takes exactly this shape, so there
    /// is nothing to translate at the handler boundary.
    /// </summary>
    public sealed record AiGenerationResult(string RawResponse, string ModelName, TokenUsage TokenUsage);

    /// <summary>
    /// A single AI backend capable of turning a composed prompt into report
    /// content — implemented once per provider in Infrastructure
    /// (<c>GroqAiProvider</c>, <c>GeminiAiProvider</c>, and the in-memory
    /// <c>FakeAiProvider</c> used for tests/local development; see
    /// architecture-plan.md, "AI Design"). Application code calls only this
    /// interface, obtained through <see cref="IAiProviderFactory"/> — never a
    /// concrete provider type directly — so adding a new provider never
    /// touches a command handler.
    /// </summary>
    public interface IAiProvider
    {
        /// <summary>
        /// Always a concrete provider (see
        /// <see cref="AiProviderTypeExtensions.IsConcreteProvider"/>) — never
        /// <see cref="AiProviderType.SystemDefault"/>, since an instance of
        /// this interface is, by construction, one specific provider.
        /// </summary>
        AiProviderType ProviderType { get; }

        Task<AiGenerationResult> GenerateAsync(AiGenerationRequest request, CancellationToken cancellationToken);
    }
}
