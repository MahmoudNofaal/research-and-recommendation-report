using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Data.Interceptors
{
    /// <summary>
    /// Safety net that stops a soft-deletable aggregate from ever being
    /// physically removed from the database. In the normal flow, every
    /// soft-deletable aggregate already sets its own <c>DeletedAtUtc</c>
    /// through its own domain method (<see cref="SoftDeletableEntity{TId}.Delete"/>,
    /// or the hand-rolled equivalent on <c>GeneratedReport</c>) before a
    /// handler ever calls <c>SaveChangesAsync</c> — by that point EF Core
    /// already sees the row as <see cref="EntityState.Modified"/>, not
    /// <see cref="EntityState.Deleted"/>, and this interceptor has nothing to
    /// do. It only steps in if something bypasses the domain method entirely —
    /// a repository calling <c>DbSet.Remove(...)</c> directly, or a future
    /// cascade-delete configuration — converting what would otherwise be a
    /// hard delete into a soft delete instead.
    ///
    /// Detection walks the entity's base-type chain for the open generic
    /// <see cref="SoftDeletableEntity{TId}"/> definition, for the same reason
    /// <see cref="AuditableEntitySaveChangesInterceptor"/> does: no marker
    /// interface needs to be added to Domain, and property values are written
    /// through <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.PropertyEntry.CurrentValue"/>
    /// so the entity's private setters are respected without any
    /// <c>InternalsVisibleTo</c> grant. This interceptor does not reach
    /// <c>GeneratedReport</c>, which hand-rolls its own soft-delete fields
    /// instead of extending <see cref="SoftDeletableEntity{TId}"/> — see that
    /// class's remarks for why.
    /// </summary>
    public sealed class SoftDeleteSaveChangesInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges
        (
            DbContextEventData eventData,
            InterceptionResult<int> result
        )
        {
            ConvertHardDeletesToSoftDeletes(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync
        (
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default
        )
        {
            ConvertHardDeletesToSoftDeletes(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void ConvertHardDeletesToSoftDeletes(DbContext? context)
        {
            if (context is null)
            {
                return;
            }

            var utcNow = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State != EntityState.Deleted)
                {
                    continue;
                }

                if (!OpenGenericBaseTypeMatcher.DerivesFromOpenGeneric(entry.Entity.GetType(), typeof(SoftDeletableEntity<>)))
                {
                    continue;
                }

                entry.State = EntityState.Modified;
                entry.Property(nameof(SoftDeletableEntity<Guid>.DeletedAtUtc)).CurrentValue = utcNow;
                entry.Property(nameof(AuditableEntity<Guid>.UpdatedAtUtc)).CurrentValue = utcNow;
            }
        }
    }
}