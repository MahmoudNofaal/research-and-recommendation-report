namespace Infrastructure.Data.Interceptors
{
    /// <summary>
    /// Reflection helper shared by <see cref="AuditableEntitySaveChangesInterceptor"/>
    /// and <see cref="SoftDeleteSaveChangesInterceptor"/> so both can recognize
    /// an entity as "auditable" or "soft-deletable" by walking its base-type
    /// chain for an open generic Domain base class
    /// (<c>Domain.Common.AuditableEntity&lt;TId&gt;</c>,
    /// <c>Domain.Common.SoftDeletableEntity&lt;TId&gt;</c>), rather than
    /// requiring Domain to expose a marker interface purely for
    /// Infrastructure's benefit. Every current and future aggregate that
    /// derives from one of those bases is picked up automatically.
    /// </summary>
    internal static class OpenGenericBaseTypeMatcher
    {
        public static bool DerivesFromOpenGeneric(Type type, Type openGenericDefinition)
        {
            ArgumentNullException.ThrowIfNull(type);
            ArgumentNullException.ThrowIfNull(openGenericDefinition);

            for (var current = type; current is not null && current != typeof(object); current = current.BaseType)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == openGenericDefinition)
                {
                    return true;
                }
            }

            return false;
        }
    }
}