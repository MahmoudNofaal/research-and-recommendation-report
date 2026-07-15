using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    /// <summary>
    /// Lets EF Core tooling (<c>dotnet ef migrations add</c>,
    /// <c>dotnet ef database update</c>) construct an
    /// <see cref="ApplicationDbContext"/> at design time, when the Web
    /// composition root's dependency injection container is not running.
    ///
    /// Never used at application runtime — <c>Program.cs</c> registers
    /// <see cref="ApplicationDbContext"/> through <c>AddDbContext</c> instead,
    /// which is also where the two <c>SaveChangesInterceptor</c>s get wired
    /// in. This factory only needs enough configuration to build a valid
    /// schema, not the interceptors that affect runtime save behavior.
    /// </summary>
    public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        private const string DefaultConnectionString =
            "Server=.;Database=ResearchReportGenerator;Trusted_Connection=True;MultipleActiveResultSets=true";

        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                connectionString = DefaultConnectionString;
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer
            (
                connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
            );

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}