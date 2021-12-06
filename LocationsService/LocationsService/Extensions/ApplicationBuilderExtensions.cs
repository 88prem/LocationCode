namespace Com.Apdcomms.DataGateway.LocationsService.Extensions
{
    using Com.Apdcomms.DataGateway.LocationsService.Queue;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using System.Globalization;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLocationsService(
            this IApplicationBuilder applicationBuilder)
        {
            // Force resolution of singletons with no external references.
            applicationBuilder.ApplicationServices.GetRequiredService<IQueue>();

            return applicationBuilder;
        }

        public static IApplicationBuilder LogConfig(
            this IApplicationBuilder applicationBuilder,
            IConfiguration configuration)
        {
            var root = (IConfigurationRoot)configuration;
            var logger = applicationBuilder.ApplicationServices.GetRequiredService<ILogger>();
            logger.Debug("CONFIG: {@Config}", root.GetDebugView());
            return applicationBuilder;
        }

        public static IApplicationBuilder ConfigureLocale(
            this IApplicationBuilder applicationBuilder,
            IConfiguration configuration)
        {
            var locale = configuration["ContainerLocale"];
            var cultureInfo = new CultureInfo(locale);

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            return applicationBuilder;
        }
    }
}