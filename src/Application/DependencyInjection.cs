using Application.Common.Behaviors;
using Application.Options;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication
        (
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            services.AddValidatedOptions<AiOptions>(configuration, AiOptions.SectionName);
            services.AddValidatedOptions<ReportGenerationOptions>
            (
                configuration,
                ReportGenerationOptions.SectionName
            );
            services.AddValidatedOptions<ExportOptions>(configuration, ExportOptions.SectionName);

            services.AddMediatRPipeline();
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }

        private static OptionsBuilder<TOptions> AddValidatedOptions<TOptions>
        (
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName
        ) where TOptions : class
        {
            return services
                .AddOptions<TOptions>()
                .Bind(configuration.GetRequiredSection(sectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }

        /// <summary>
        /// Registers MediatR against every command/query handler in this
        /// assembly, plus the cross-cutting pipeline behaviors from
        /// <see cref="Application.Common.Behaviors"/>, in the order they must
        /// run: unhandled-exception logging wraps everything; request/outcome
        /// logging and slow-request detection run next; then authorization
        /// (is there a signed-in user at all); and finally input validation,
        /// immediately before the real handler — so a request that fails
        /// authorization never pays for validation work first.
        /// </summary>
        private static IServiceCollection AddMediatRPipeline(this IServiceCollection services)
        {
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

                configuration.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
                configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
                configuration.AddOpenBehavior(typeof(PerformanceBehavior<,>));
                configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
                configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            return services;
        }


    }
}
