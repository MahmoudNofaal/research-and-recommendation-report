using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Infrastructure.Data.Interceptors
{
    /// <summary>
    /// Stamps <see cref="AuditableEntity{TId}.CreatedAtUtc"/> and
    /// <see cref="AuditableEntity{TId}.UpdatedAtUtc"/> on every tracked
    /// auditable aggregate immediately before changes are persisted. These are
    /// generic row-insertion/modification timestamps with no business meaning
    /// of their own — see the type-level remarks on <see cref="AuditableEntity{TId}"/>
    /// for why some aggregates (like <c>GeneratedReport</c>) deliberately do
    /// not use this base type and therefore fall outside this interceptor's
    /// reach entirely; they stamp their own business-meaningful timestamps via
    /// their own domain methods instead.
    ///
    /// Detection walks the entity's base-type chain for the open generic
    /// <see cref="AuditableEntity{TId}"/> definition (see
    /// <see cref="OpenGenericBaseTypeMatcher"/>) rather than requiring a
    /// marker interface on Domain. Because <c>CreatedAtUtc</c>/<c>UpdatedAtUtc</c>
    /// only expose <c>internal</c> setters, values are written through
    /// <see cref="Microsoft.EntityFrameworkCore.ChangeTracking.PropertyEntry.CurrentValue"/>
    /// rather than the CLR property setter: EF Core's change tracker always
    /// resolves property access through its own compiled accessors, which are
    /// not subject to C# accessibility at all, so this works with no
    /// <c>InternalsVisibleTo</c> grant needed between Domain and Infrastructure.
    /// </summary>
    public sealed class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges
        (
            DbContextEventData eventData,
            InterceptionResult<int> result
        )
        {
            StampAuditableEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync
        (
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default
        )
        {
            StampAuditableEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void StampAuditableEntities(DbContext? context)
        {
            if (context is null)
            {
                return;
            }

            var utcNow = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (!OpenGenericBaseTypeMatcher.DerivesFromOpenGeneric(entry.Entity.GetType(), typeof(AuditableEntity<>)))
                {
                    continue;
                }

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property(nameof(AuditableEntity<Guid>.CreatedAtUtc)).CurrentValue = utcNow;
                        entry.Property(nameof(AuditableEntity<Guid>.UpdatedAtUtc)).CurrentValue = utcNow;
                        break;

                    case EntityState.Modified:
                        entry.Property(nameof(AuditableEntity<Guid>.UpdatedAtUtc)).CurrentValue = utcNow;
                        break;
                }
            }
        }
    }
}