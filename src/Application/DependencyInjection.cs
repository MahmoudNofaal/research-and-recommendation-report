using Application.Options;
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

            return services;
        }

        private static OptionsBuilder<TOptions> AddValidatedOptions<TOptions>
        (
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName
        )
            where TOptions : class
        {
            return services
                .AddOptions<TOptions>()
                .Bind(configuration.GetRequiredSection(sectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();
        }
    }
}
