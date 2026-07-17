using Domain.Enums;

namespace Application.Common.Errors
{
    /// <summary>
    /// Named catalogue of Application-level AI generation failures, matching
    /// the failure classification the UI presents to users almost verbatim
    /// (see ui-ux-specification.md, "11.3 Failure classification → friendly
    /// copy"). Every case here is an expected, retryable outcome of calling an
    /// AI provider — never a programming error — so each is surfaced as an
    /// <see cref="ApplicationError"/> rather than thrown.
    /// </summary>
    public static class AiApplicationErrors
    {
        public static ApplicationError ProviderTimeout(AiProviderType provider) => ApplicationError.Failure
        (
            "Ai.ProviderTimeout",
            $"The {provider} provider took too long to respond."
        );

        public static ApplicationError EmptyOutput(AiProviderType provider) => ApplicationError.Failure
        (
            "Ai.EmptyOutput",
            $"The {provider} provider didn't return usable content this time."
        );

        public static ApplicationError ProviderUnavailable(AiProviderType provider) => ApplicationError.Failure
        (
            "Ai.ProviderUnavailable",
            $"{provider} isn't available right now."
        );

        public static ApplicationError RateLimited(AiProviderType provider) => ApplicationError.Failure
        (
            "Ai.RateLimited",
            $"{provider} is temporarily busy."
        );

        public static ApplicationError Unknown(AiProviderType provider) => ApplicationError.Failure
        (
            "Ai.Unknown",
            "Something unexpected happened generating your report."
        );

        /// <summary>
        /// Raised by the provider factory when no configured provider —
        /// neither the user's preference nor the system default — currently
        /// reports itself healthy (see <c>IAiProviderHealthCheck</c>).
        /// </summary>
        public static ApplicationError NoHealthyProviderAvailable => ApplicationError.Failure
        (
            "Ai.NoHealthyProviderAvailable",
            "No AI provider is currently available to generate this report."
        );
    }
}
