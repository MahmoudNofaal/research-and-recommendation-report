using Domain.Common;

namespace Domain.ValueObjects
{
    /// <summary>
    /// The timing envelope of a single AI generation attempt: when it started,
    /// when it finished (if it has), and the duration between the two. Immutable
    /// like every value object — <see cref="Complete"/> returns a new instance
    /// rather than mutating this one; the owning <c>ReportGenerationRun</c>
    /// replaces its own reference.
    /// </summary>
    public sealed class GenerationTiming : ValueObject
    {
        public DateTime StartedAtUtc { get; }

        public DateTime? CompletedAtUtc { get; }

        public TimeSpan? Duration => CompletedAtUtc.HasValue ? CompletedAtUtc.Value - StartedAtUtc : null;

        private GenerationTiming(DateTime startedAtUtc, DateTime? completedAtUtc)
        {
            StartedAtUtc = startedAtUtc;
            CompletedAtUtc = completedAtUtc;
        }

        public static GenerationTiming Start(DateTime startedAtUtc) => new(startedAtUtc, completedAtUtc: null);

        /// <summary>
        /// Returns a new <see cref="GenerationTiming"/> with the same start time
        /// and the given completion time recorded.
        /// </summary>
        public GenerationTiming Complete(DateTime completedAtUtc)
        {
            if (completedAtUtc < StartedAtUtc)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(completedAtUtc),
                    "Completion time cannot be earlier than the start time.");
            }

            return new GenerationTiming(StartedAtUtc, completedAtUtc);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return StartedAtUtc;
            yield return CompletedAtUtc;
        }
    }
}
