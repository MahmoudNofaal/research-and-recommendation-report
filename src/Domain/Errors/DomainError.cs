namespace Domain.Errors
{
    /// <summary>
    /// A machine-readable code paired with a human-readable message describing why
    /// a domain operation could not proceed. Carried by domain exceptions so the
    /// Application layer can map failures to validation results or ProblemDetails
    /// without parsing exception messages.
    /// </summary>
    /// <param name="Code">A stable, dotted identifier such as "ReportRequest.MinimumTopicsRequired".</param>
    /// <param name="Message">A human-readable explanation suitable for surfacing to a caller.</param>
    public sealed record DomainError(string Code, string Message)
    {
        public override string ToString() => $"{Code}: {Message}";
    }
}
