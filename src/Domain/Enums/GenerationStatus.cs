namespace Domain.Enums
{
    /// <summary>
    /// The state machine of a single <c>ReportGenerationRun</c>. Progression is
    /// strictly linear and one-directional: <see cref="Pending"/> to
    /// <see cref="InProgress"/> to a terminal state of either
    /// <see cref="Succeeded"/> or <see cref="Failed"/>. Terminal states cannot be
    /// re-entered or transitioned away from.
    /// </summary>
    public enum GenerationStatus
    {
        Pending = 0,
        InProgress = 1,
        Succeeded = 2,
        Failed = 3
    }
}
