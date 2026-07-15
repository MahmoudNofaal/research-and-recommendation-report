using Application.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    /// <summary>
    /// Commits changes tracked on the shared, per-request <see cref="ApplicationDbContext"/>.
    /// Deliberately thin: EF Core's change tracker is already the unit of work
    /// for the common case, so this mostly just exposes
    /// <see cref="ApplicationDbContext.SaveChangesAsync(CancellationToken)"/>
    /// through the Application-facing port, adding an explicit-transaction
    /// path only for the less common case of a handler needing to group more
    /// than one commit atomically.
    /// </summary>
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);

        public async Task<TResult> ExecuteInTransactionAsync<TResult>
        (
            Func<CancellationToken, Task<TResult>> operation,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentNullException.ThrowIfNull(operation);

            // SQL Server connections may be configured with EnableRetryOnFailure,
            // which forbids manually opened transactions unless they run inside
            // the provider's own execution strategy — otherwise a transient
            // fault mid-transaction throws instead of retrying. Routing through
            // CreateExecutionStrategy() here means callers never need to know
            // whether retry-on-failure is turned on.
            var strategy = _context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    var result = await operation(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }
    }
}