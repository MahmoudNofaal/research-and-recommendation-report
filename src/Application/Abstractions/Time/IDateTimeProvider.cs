namespace Application.Abstractions.Time
{
    /// <summary>
    /// The single source of "now" for every Application use case that needs
    /// to stamp a business timestamp — <c>GenerationTiming.Start</c>,
    /// <c>ReportExport.Create</c>'s <c>exportedAtUtc</c>,
    /// <c>GeneratedReport.Delete</c>'s <c>deletedAtUtc</c>, and so on.
    /// Handlers depend on this instead of calling
    /// <see cref="DateTime.UtcNow"/> directly, so tests can supply a fixed,
    /// known instant (see the not-yet-built <c>FakeDateTimeProvider</c> test
    /// fake) instead of asserting against a moving target.
    ///
    /// This is unrelated to <c>AuditableEntity</c>'s <c>CreatedAtUtc</c>/
    /// <c>UpdatedAtUtc</c>, which are stamped by an EF Core
    /// <c>SaveChangesInterceptor</c> in Infrastructure and never touch this
    /// abstraction at all — see that interceptor's own remarks for why.
    /// </summary>
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}
