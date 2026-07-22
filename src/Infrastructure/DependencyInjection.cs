using Application.Abstractions.Data;
using Application.Options;
using Infrastructure.Data;
using Application.Abstractions.Auth;
using Infrastructure.Authentication;
using Infrastructure.Data.Identity;
using Infrastructure.Data.Interceptors;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    /// <summary>
    /// Composition root for everything Infrastructure owns. Called once from
    /// Web's <c>Program.cs</c>:
    ///
    /// <code>
    /// builder.Services.AddInfrastructure(builder.Configuration);
    /// </code>
    ///
    /// Currently wires up EF Core (<see cref="ApplicationDbContext"/> plus its
    /// two interceptors), <see cref="IUnitOfWork"/>, and the Identity stores.
    /// AI provider registration, export renderer registration, and repository
    /// registration belong here too and are expected to grow alongside this
    /// method as those pieces get built — they don't exist yet, so this only
    /// registers what has actually been implemented so far.
    /// </summary>
    public static class DependencyInjection
    {
        private const string ConnectionStringName = "DefaultConnection";

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPersistence(configuration);
            services.AddIdentityServices();
            services.AddAuthenticationServices(configuration);

            return services;
        }

        private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            // Registered individually — rather than newed-up inline inside
            // AddInterceptors below — so each interceptor can take its own
            // dependencies via DI later (an IDateTimeProvider, for example)
            // without this registration method needing to change.
            services.AddScoped<AuditableEntitySaveChangesInterceptor>();
            services.AddScoped<SoftDeleteSaveChangesInterceptor>();

            services.AddDbContext<ApplicationDbContext>((serviceProvider, optionsBuilder) =>
            {
                var connectionString = configuration.GetConnectionString(ConnectionStringName);
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException
                    (
                        $"Connection string '{ConnectionStringName}' was not found in configuration."
                    );
                }

                optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);

                    // Gives IUnitOfWork.ExecuteInTransactionAsync's execution
                    // strategy something to actually retry; without this,
                    // transient SQL Server faults fail immediately instead.
                    sqlOptions.EnableRetryOnFailure();
                });

                optionsBuilder.AddInterceptors
                (
                    serviceProvider.GetRequiredService<AuditableEntitySaveChangesInterceptor>(),
                    serviceProvider.GetRequiredService<SoftDeleteSaveChangesInterceptor>()
                );
            });

            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        private static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireDigit = true;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.Lockout.MaxFailedAccessAttempts = 5;
                    options.Lockout.AllowedForNewUsers = true;

                    options.User.RequireUniqueEmail = true;

                    // Email confirmation can be enabled after SMTP settings
                    // are supplied through user secrets or deployment config.
                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }

        private static IServiceCollection AddAuthenticationServices
        (
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.Configure<EmailOptions>(configuration.GetSection(EmailOptions.SectionName));
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IExternalAuthenticationService, ExternalAuthenticationService>();
            services.AddScoped<IEmailSender, EmailSender>();

            var googleSection = configuration.GetSection(GoogleAuthenticationOptions.SectionName);
            var googleOptions = googleSection.Get<GoogleAuthenticationOptions>();

            if (googleOptions is not null && googleOptions.Enabled)
            {
                services
                    .AddAuthentication()
                    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                    {
                        options.ClientId = googleOptions.ClientId;
                        options.ClientSecret = googleOptions.ClientSecret;
                        options.CallbackPath = googleOptions.CallbackPath;

                        options.Scope.Add("email");
                        options.Scope.Add("profile");

                        options.SaveTokens = true;
                    });
            }

            return services;
        }
    }
}
