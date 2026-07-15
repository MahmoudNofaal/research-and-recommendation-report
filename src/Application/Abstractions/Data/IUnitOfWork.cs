namespace Application.Abstractions.Data
{
    /// <summary>
    /// Commits the changes a command handler has made to aggregates loaded
    /// through the write-side repository ports, as a single atomic unit. This
    /// is the only way Application ever persists a change — no handler calls
    /// <c>SaveChanges</c> on a context directly, because no handler has a
    /// context to call it on.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Persists every change tracked since the last commit. A single call
        /// to this method is already atomic — all pending changes succeed or
        /// fail together — which covers the overwhelming majority of use cases
        /// in this application (a handler loads one or two aggregates, mutates
        /// them, and saves once).
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Runs <paramref name="operation"/> inside an explicit database
        /// transaction, using the underlying provider's execution strategy so
        /// transient faults (for example, a SQL Server connection retry) are
        /// retried safely rather than left half-committed. Reach for this only
        /// when a use case must group more than one <see cref="SaveChangesAsync"/>
        /// call into a single atomic outcome — most use cases should just call
        /// <see cref="SaveChangesAsync"/> once and never need this at all.
        /// </summary>
        Task<TResult> ExecuteInTransactionAsync<TResult>
        (
            Func<CancellationToken, Task<TResult>> operation,
            CancellationToken cancellationToken = default
        );
    }
}