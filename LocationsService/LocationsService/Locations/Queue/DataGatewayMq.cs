namespace Com.Apdcomms.DataGateway.LocationsService.Queue
{
    using System;
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService.Lte;
    using Com.Apdcomms.DataGateway.LocationsService.Notifications;
    using Com.Apdcomms.DataGateway.LocationsService.Refresh;
    using Com.Apdcomms.DataGateway.LocationsService.ReportPosition;
    using Com.Apdcomms.DataGateway.LocationsService.Tpi;
    using Com.Apdcomms.DataGateway.QueueTypes.Pipeline.Locations.Commands;
    using EasyNetQ;
    using MediatR;
    using Microsoft.Extensions.Logging;

    public class DataGatewayMq : IQueue, IDisposable
    {
        private readonly IBus bus;
        private readonly LocationToQueueLocationMapper locationToQueueLocationMapper;
        private readonly ILogger logger;
        private readonly IMediator mediator;

        public DataGatewayMq(
            IBus bus,
            LocationToQueueLocationMapper locationToQueueLocationMapper,
            ILogger<DataGatewayMq> logger,
            IMediator mediator)
        {
            this.bus = bus;
            this.locationToQueueLocationMapper = locationToQueueLocationMapper;
            this.logger = logger;
            this.mediator = mediator;

            // TODO(Steve): Left this for now, the queue must be running in order for this code to work.
            // Tried following instructions: https://github.com/EasyNetQ/EasyNetQ/wiki/The-Advanced-API#events
            // But then EasyNetQ won't connect at all and no callback is received anyway
            bus.PubSub.Subscribe<Com.Apdcomms.DataGateway.QueueTypes.Pipeline.Locations.Location>("LocationsService", OnLocationReceived, x => x.WithTopic(nameof(Com.Apdcomms.DataGateway.QueueTypes.Pipeline.Locations.Location)));
            
            this.bus.Advanced.Connected += OnConnected;
        }

        public async Task EnqueueLocation(Location location)
        {
            var queueLocation = locationToQueueLocationMapper.Map(location);
            
            logger.LogInformation("Publishing Location - {@Location}", queueLocation);
            
            await bus.PubSub.PublishAsync(queueLocation, handler => handler.WithTopic(nameof(QueueTypes.Pipeline.Locations.Location)));
        }

        public async Task EnqueueTpiPositionOnRequestCommand(TpiPositionOnRequestCommand tpiPositionOnRequestCommand)
        {
            var queueTpiPorCommand = new QueueTypes.Pipeline.Locations.TpiPositionOnRequestCommand
            {
                ResourceId = tpiPositionOnRequestCommand.ResourceId
            };
            
            logger.LogInformation("Publishing TPI PoR Command - {@TpiPositionOnRequestCommand}", queueTpiPorCommand);

            await bus.PubSub.PublishAsync(queueTpiPorCommand, handler => handler.WithTopic(nameof(QueueTypes.Pipeline.Locations.TpiPositionOnRequestCommand)));
        }
        
        public async Task EnqueueLocationsRefreshCommand(LocationsRefreshRequest locationsRefreshRequest)
        {
            var command = new LocationsRefresh();
            
            this.logger.LogInformation("Publishing {@Name} command - {@Command}", nameof(LocationsRefresh), command);

            await bus.PubSub.PublishAsync(command, handler => handler.WithTopic(nameof(LocationsRefresh)));
        }
        
        private void OnConnected(object sender, ConnectedEventArgs e)
        {
            bus.PubSub.Subscribe<Com.Apdcomms.DataGateway.QueueTypes.Pipeline.Locations.Location>("LocationsService", OnLocationReceived, x => x.WithTopic(nameof(Com.Apdcomms.DataGateway.QueueTypes.Pipeline.Locations.Location)));
        }

        private void OnLocationReceived(
            Com.Apdcomms.DataGateway.QueueTypes.Pipeline.Locations.Location location)
        {
            logger.LogDebug("Received location from pipeline queue: {@Location}", location);

            try
            {
                mediator.Publish(new Notification
                {
                    Source = location.Source,
                    SourceId = location.SourceId,
                    TpiMetaData = location.Source == "TPI" ? MapTpiMetaData(location.TpiMetaData) : null,
                    LteMetaData = location.Source == "LTE" ? MapLteMetaData(location.LteMetaData) : null
                });
            }
            catch (Exception e)
            {
                logger.LogWarning(
                    e, 
                    "Failed to process location from pipeline! Location ignored: {@Location}",
                    location);
            }

            logger.LogDebug("Finished processing location from pipeline queue");
        }

        private TpiLocationMetaData MapTpiMetaData(
            Com.Apdcomms.DataGateway.QueueTypes.Pipeline.Locations.TpiLocationMetaData metaData) =>
            new()
            {
                Bearing = metaData.Bearing,
                Class = metaData.Class,
                Contents = metaData.Contents,
                Data = metaData.Data,
                Fix = metaData.Fix,
                Inputs = metaData.Inputs,
                Latitude = metaData.Latitude,
                Longitude = metaData.Longitude,
                Outputs = metaData.Outputs,
                Resource = metaData.Resource,
                Speed = metaData.Speed,
                Status = metaData.Status,
                Timestamp = metaData.Timestamp,
                EventId = metaData.EventId,
                EventText = metaData.EventText,
                GeoData = metaData.GeoData
            };


        private LteLocationMetaData MapLteMetaData(
            Com.Apdcomms.DataGateway.QueueTypes.Pipeline.Locations.LteLocationMetaData metaData) =>
            new()
            {
                GroupDisplayName = metaData.GroupDisplayName,
                GroupID = metaData.GroupID,
                ReportType = metaData.ReportType,
                UserDisplayName = metaData.UserDisplayName,
                UserID = metaData.UserID,
                Latitude = metaData.Latitude,
                Longitude = metaData.Longitude
            };

        public void Dispose()
        {
            bus?.Dispose();
        }
    }
}