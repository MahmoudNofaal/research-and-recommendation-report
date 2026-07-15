using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The number of input and output tokens consumed by a single AI generation
    /// attempt, as reported by the provider.
    /// </summary>
    public sealed class TokenUsage : ValueObject
    {
        public int InputTokens { get; }

        public int OutputTokens { get; }

        public int Total => InputTokens + OutputTokens;

        private TokenUsage(int inputTokens, int outputTokens)
        {
            InputTokens = inputTokens;
            OutputTokens = outputTokens;
        }

        public static TokenUsage Create(int inputTokens, int outputTokens)
        {
            if (inputTokens < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(inputTokens), "Input token count cannot be negative.");
            }

            if (outputTokens < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(outputTokens), "Output token count cannot be negative.");
            }

            return new TokenUsage(inputTokens, outputTokens);
        }

        public static TokenUsage Zero { get; } = new(0, 0);

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return InputTokens;
            yield return OutputTokens;
        }

        public override string ToString()
            => $"{InputTokens} in / {OutputTokens} out ({Total} total)";
    }
}
