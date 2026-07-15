namespace Domain.Enums
{
    /// <summary>
    /// Identifies an AI provider within the domain model. This single enum serves
    /// two different roles depending on where it is used:
    ///
    /// <list type="bullet">
    /// <item>
    /// As <c>ReportRequest.PreferredAiProvider</c>, it expresses the user's stated
    /// <em>preference</em> at request time, where <see cref="SystemDefault"/> is a
    /// first-class, explicit "let the system decide" choice rather than a nullable
    /// sentinel — avoiding a nullable enum and the scattered null-checks that come
    /// with it.
    /// </item>
    /// <item>
    /// As <c>ReportGenerationRun.AiProvider</c>, it records which provider actually
    /// executed a generation attempt. By the time a run exists, any
    /// <see cref="SystemDefault"/> preference has already been resolved to a
    /// concrete provider by the Application layer's provider factory, so a run may
    /// only ever record <see cref="Groq"/>, <see cref="Gemini"/>, or
    /// <see cref="Fake"/> — never <see cref="SystemDefault"/>. This is enforced by
    /// <see cref="AiProviderTypeExtensions.IsConcreteProvider"/> at the point a run
    /// is created.
    /// </item>
    /// </list>
    /// </summary>
    public enum AiProviderType
    {
        /// <summary>No explicit preference; the system selects a provider on the user's behalf.</summary>
        SystemDefault = 0,

        Groq = 1,

        Gemini = 2,

        /// <summary>
        /// The in-memory test/development provider. A legitimate value for a
        /// generation run's recorded provider (the audit trail must reflect what
        /// actually ran, including in local development and automated tests), but
        /// never a value a real user would state as a preference.
        /// </summary>
        Fake = 3
    }

    /// <summary>
    /// Domain-level rules about which <see cref="AiProviderType"/> values are
    /// concrete, executable providers versus preference-only placeholders.
    /// </summary>
    public static class AiProviderTypeExtensions
    {
        /// <summary>
        /// True for every provider that can actually execute a generation
        /// (<see cref="AiProviderType.Groq"/>, <see cref="AiProviderType.Gemini"/>,
        /// <see cref="AiProviderType.Fake"/>); false for
        /// <see cref="AiProviderType.SystemDefault"/>, which only ever exists as a
        /// resolved-away preference and must never appear as the recorded provider
        /// of a <c>ReportGenerationRun</c>.
        /// </summary>
        public static bool IsConcreteProvider(this AiProviderType providerType) =>
            providerType != AiProviderType.SystemDefault;
    }
}
