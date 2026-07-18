using Domain.Enums;

namespace Application.Abstractions.AI
{
    /// <summary>
    /// Resolves a <c>ReportRequest.PreferredAiProvider</c> — which may itself
    /// be the non-concrete <see cref="AiProviderType.SystemDefault"/> — down
    /// to one specific, currently healthy <see cref="IAiProvider"/>. This is
    /// exactly the resolution <see cref="AiProviderType"/>'s own remarks
    /// describe happening "by the time a run exists": a
    /// <c>ReportGenerationRun</c> may only ever record a concrete provider,
    /// and this factory is where that guarantee is produced.
    /// </summary>
    public interface IAiProviderFactory
    {
        /// <summary>
        /// Returns the resolved provider, or <see langword="null"/> if
        /// neither the preference nor the system default currently reports
        /// itself healthy (see <see cref="IAiProviderHealthCheck"/>) — an
        /// expected, retryable outcome the caller maps to
        /// <c>AiApplicationErrors.NoHealthyProviderAvailable</c> rather than
        /// an exception.
        /// </summary>
        Task<IAiProvider?> ResolveAsync(AiProviderType preferredProvider, CancellationToken cancellationToken);
    }
}
