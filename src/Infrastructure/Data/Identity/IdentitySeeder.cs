using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data.Identity
{
    /// <summary>
    /// Ensures the small set of reference Identity roles exist. Intended to be
    /// called once at application startup, after migrations have been applied
    /// (see <c>Program.cs</c>).
    ///
    /// <see cref="Roles"/> is empty by design: version 1 excludes the admin
    /// dashboard and self-service template management, so there is no role
    /// yet that needs to exist. The seeder is still wired up so that adding a
    /// role later — for example, an "Administrator" role once template/preset
    /// management moves out of "manually managed by the project owner" — is a
    /// one-line change here rather than a new startup concern.
    /// </summary>
    public static class IdentitySeeder
    {
        public static readonly IReadOnlyList<string> Roles = [];

        public static async Task SeedAsync
        (
            RoleManager<ApplicationRole> roleManager,
            ILogger logger,
            CancellationToken cancellationToken = default
        )
        {
            ArgumentNullException.ThrowIfNull(roleManager);
            ArgumentNullException.ThrowIfNull(logger);

            foreach (var roleName in Roles)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (await roleManager.RoleExistsAsync(roleName))
                {
                    continue;
                }

                var result = await roleManager.CreateAsync(new ApplicationRole(roleName));
                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    logger.LogError("Failed to seed role '{RoleName}': {Errors}", roleName, errors);
                    continue;
                }

                logger.LogInformation("Seeded role '{RoleName}'.", roleName);
            }
        }
    }
}