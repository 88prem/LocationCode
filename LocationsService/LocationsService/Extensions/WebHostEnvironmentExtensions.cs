namespace Com.Apdcomms.DataGateway.LocationsService.Extensions
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;

    public static class WebHostEnvironmentExtensions
    {
        public static bool IsSwaggerEnvironment(this IWebHostEnvironment webHostEnvironment) =>
            webHostEnvironment.IsDevelopment() ||
            webHostEnvironment.IsEnvironment("Test") ||
            webHostEnvironment.IsEnvironment("DockerDebug");
    }
}