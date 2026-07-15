using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data.Identity
{
    /// <summary>
    /// Version 1 has no admin dashboard and no role-gated features — every
    /// registered user has identical capabilities over their own report
    /// history (see <c>project-vision-statement.md</c>, "Excluded from
    /// Version 1"). This type exists so the Identity schema and
    /// <see cref="IdentitySeeder"/> are ready for role-based authorization
    /// later (for example, an admin role for template/preset management)
    /// without a breaking schema change when that feature is added.
    /// </summary>
    public sealed class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName)
            : base(roleName)
        {
        }
    }
}