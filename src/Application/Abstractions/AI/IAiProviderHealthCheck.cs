using Domain.Enums;

namespace Application.Abstractions.AI
{
    /// <summary>
    /// A single provider's health as of the last check — backs the ambient
    /// three-dot status cluster in the top bar (see
    /// ui-ux-specification.md, "11.4 Provider health as ambient awareness")
    /// and the pre-selection of a healthy alternative when generation fails
    /// (see "11.3 Failure classification").
    /// </summary>
    public sealed record AiProviderHealth
    (
        AiProviderType Provider,
        bool IsHealthy,
        string? StatusMessage,
        DateTime CheckedAtUtc
    );

    /// <summary>
    /// Checks whether a configured AI provider is currently reachable and
    /// usable, without running a full generation. Used both by
    /// <see cref="IAiProviderFactory"/> to pick a healthy provider and by the
    /// dashboard/provider-status query that renders the health cluster.
    /// </summary>
    public interface IAiProviderHealthCheck
    {
        Task<AiProviderHealth> CheckHealthAsync(AiProviderType provider, CancellationToken cancellationToken);

        /// <summary>Every configured provider's health, for the dashboard's ambient status cluster.</summary>
        Task<IReadOnlyList<AiProviderHealth>> CheckAllAsync(CancellationToken cancellationToken);
    }
}
