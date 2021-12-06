namespace Com.Apdcomms.DataGateway.LocationsService
{
    using System.IO;
    using System.Reflection;
    using Com.Apdcomms.DataGateway.LocationsService.Extensions;
    using Hellang.Middleware.ProblemDetails;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.OpenApi.Models;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocationsService();
            services.AddLocationsServiceDataAccessLayer(Configuration);
            services.AddProblemDetails();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LocationsService", Version = "v1" });
                c.IncludeXmlComments(Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "LocationsService.xml"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsSwaggerEnvironment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "LocationsService v1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseProblemDetails();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            app.ConfigureLocale(Configuration);
            app.UseLocationsService();
            app.LogConfig(Configuration);
        }
    }
}