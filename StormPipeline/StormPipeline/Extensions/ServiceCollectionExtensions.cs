namespace Com.Apdcomms.StormPipeline.Extensions
{
    using Com.Apdcomms.StormPipeline.Parsing;
    using Com.Apdcomms.StormPipeline.Parsing.Factory;
    using Com.Apdcomms.StormPipeline.Parsing.Messages;
    using Com.Apdcomms.StormPipeline.Queue;
    using Com.Apdcomms.StormPipeline.Storm;
    using MediatR;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddStormPipeline(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMediatR(typeof(Program));
            serviceCollection.AddTransient<StormMessageValidator>();
            serviceCollection.AddTransient<StormMessageParser>();
            serviceCollection.AddTransient<StormMessageMapper>();
            serviceCollection.AddTransient<IStormMessageFactory<IOperate>, StormMessageFactory<IOperate>>();
            serviceCollection.AddTransient<IStormMesageCodeFactory<Queue.MessageCodes.StormMetaData>,
                StormMessageCodeFactory<Queue.MessageCodes.StormMetaData>>();
            return serviceCollection;
        }

        public static IServiceCollection AddStormPipelineDal(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            var stormConfiguration = configuration.GetSection("Storm");
            var stormHostName = stormConfiguration["HostName"];
            var stormPort = stormConfiguration["Port"];
            var mappingConfig = configuration.GetSection("Mappings");

            serviceCollection.Configure<MappingConfig>(configuration.GetSection("Mappings"));

            serviceCollection.AddSingleton(sp =>
                new StormTcpInterface($"{stormHostName}:{stormPort}",
                sp.GetRequiredService<ILogger>(),
                sp.GetRequiredService<IMediator>()));
            serviceCollection.AddSingleton<INotificationQueue, EasyNetNotificationQueue>();
            serviceCollection.RegisterEasyNetQ(configuration.GetConnectionString("PipelineMq"));

            return serviceCollection;
        }
    }
}