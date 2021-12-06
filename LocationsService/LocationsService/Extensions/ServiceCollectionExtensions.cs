namespace Com.Apdcomms.DataGateway.LocationsService.Extensions
{
    using Com.Apdcomms.DataGateway.LocationsService.Database;
    using Com.Apdcomms.DataGateway.LocationsService.Lte;
    using Com.Apdcomms.DataGateway.LocationsService.Notifications;
    using Com.Apdcomms.DataGateway.LocationsService.Queue;
    using Com.Apdcomms.DataGateway.LocationsService.Tpi;
    using MediatR;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using MongoDB.Driver;
    using Serilog;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLocationsService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMediatR(typeof(Startup));
            serviceCollection.AddLogging();
            serviceCollection.AddSingleton<NotificationValidator>();
            serviceCollection.AddSingleton<TpiMetaDataMerger>();
            serviceCollection.AddSingleton<LteMetaDataMerger>();
            serviceCollection.AddSingleton<LocationToQueueLocationMapper>();
            serviceCollection.AddSingleton<NotificationToLocationConverter>();
            
            return serviceCollection;
        }

        public static IServiceCollection AddLocationsServiceDataAccessLayer(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton(Log.Logger);
            
            services.RegisterEasyNetQ(configuration.GetConnectionString("DataGatewayMq"));
            services.AddSingleton(new MongoClient(configuration.GetConnectionString("MongoDb")));
            services.AddSingleton<IDatabase, LocationsMongoDatabase>();
            services.AddSingleton<IQueue, DataGatewayMq>();
			services.AddOptions<ServiceConfiguration>().Bind(configuration.GetSection("ServiceConfiguration"));
            
            return services;
        }
    }
}