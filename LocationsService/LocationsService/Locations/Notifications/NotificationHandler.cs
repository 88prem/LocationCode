namespace Com.Apdcomms.DataGateway.LocationsService.Notifications
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService.Database;
    using Com.Apdcomms.DataGateway.LocationsService.Lte;
    using Com.Apdcomms.DataGateway.LocationsService.Queue;
    using Com.Apdcomms.DataGateway.LocationsService.Tpi;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;

    public class NotificationHandler : INotificationHandler<Notification>
    {
        private readonly ILogger<NotificationHandler> logger;
        private readonly IDatabase database;
        private readonly IQueue queue;
        private readonly NotificationValidator notificationValidator;
        private readonly TpiMetaDataMerger tpiMetaDataMerger;
        private readonly LteMetaDataMerger lteMetaDataMerger;
        private readonly NotificationToLocationConverter notificationToLocationConverter;
        private readonly ServiceConfiguration serviceConfiguration;

        public NotificationHandler(
            ILogger<NotificationHandler> logger,
            IDatabase database,
            IQueue queue,
            NotificationValidator notificationValidator,
            TpiMetaDataMerger tpiMetaDataMerger,
            LteMetaDataMerger lteMetaDataMerger,
            NotificationToLocationConverter notificationToLocationConverter,
            IOptions<ServiceConfiguration> options)
        {
            this.logger = logger;
            this.database = database;
            this.queue = queue;
            this.notificationValidator = notificationValidator;
            this.tpiMetaDataMerger = tpiMetaDataMerger;
            this.lteMetaDataMerger = lteMetaDataMerger;
            this.notificationToLocationConverter = notificationToLocationConverter;
            this.serviceConfiguration = options.Value;
        }

        public async Task Handle(
            Notification notification,
            CancellationToken cancellationToken)
        {
            LogNewNotification(notification);

            try
            {
                notificationValidator.Validate(notification);
            }
            catch (Exception e)
            {
                logger.LogError("Failed validation with exception :{@Exception}", e.Message);
                throw;
            }
            

            var locationId = LocationIdProvider.Get(notification.Source, notification.SourceId);
            var location = notificationToLocationConverter.Convert(notification);

            var tasks = new List<Task>();
            if (serviceConfiguration.StoreLocations)
            {
                var existingLocation = await database.Get(locationId);

                if (existingLocation == null)
                {
                    tasks.Add(database.Create(location));
                }
                else
                {
                    location = MergeExistingWithCurrentLocation(existingLocation, location);

                    if (!location.IsHistoricLocationUpdate)
                    {
                        tasks.Add(database.Update(location));
                    }
                }
            }

            if (serviceConfiguration.EnqueueLocations)
            {
                tasks.Add(queue.EnqueueLocation(location));
            }
            
            await Task.WhenAll(tasks);
            
            LogNotificationHandled(location);
        }

        private Location MergeExistingWithCurrentLocation(Location existingLocation, Location location)
        {
            switch (location.Source)
            {
                case "TPI":
                    try
                    {
                        var mergedTpiMetaData = tpiMetaDataMerger.Merge(existingLocation.TpiMetaData, location.TpiMetaData);
                        location = location with
                        {
                            TpiMetaData = mergedTpiMetaData,
                            IsHistoricLocationUpdate = DateTime.Parse(existingLocation.TpiMetaData.Timestamp, Thread.CurrentThread.CurrentUICulture) > DateTime.Parse(location.TpiMetaData.Timestamp, Thread.CurrentThread.CurrentUICulture)
                        };
                        break;
                    }
                    catch (Exception e)
                    {
                        logger.LogError("Failed to merge existing location update with current location update with exception :{@Exception}", e.Message);
                        logger.LogInformation("Current culture is {@CultureNativeName}", Thread.CurrentThread.CurrentUICulture.NativeName);
                        throw;
                    }
                case "LTE":
                    var mergedLteMetaData = lteMetaDataMerger.Merge(existingLocation.LteMetaData, location.LteMetaData);
                    location = location with { LteMetaData = mergedLteMetaData };
                    break;
            }
            return location;
        }

        private void LogNewNotification(Notification notification)
        {
            logger.LogInformation("Processing a new location notification");
            logger.LogDebug("Location notification: {@LocationNotification}", notification);
        }

        private void LogNotificationHandled(Location updatedLocation)
        {
            logger.LogInformation("Finished processing location with id {LocationId}", updatedLocation.LocationId);
            logger.LogDebug("Processed Location: {@Location}", updatedLocation);
        }
    }
}