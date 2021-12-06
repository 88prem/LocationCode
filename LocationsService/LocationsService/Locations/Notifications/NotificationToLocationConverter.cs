namespace Com.Apdcomms.DataGateway.LocationsService.Notifications
{
    using System;

    public class NotificationToLocationConverter
    {
        public Location Convert(Notification notification) =>
            new()
            {
                LocationId = LocationIdProvider.Get(notification.Source, notification.SourceId),
                Source = notification.Source,
                SourceId = notification.SourceId,
                Timestamp = DateTime.Now,
                TpiMetaData = notification.TpiMetaData,
                LteMetaData = notification.LteMetaData
            };
    }
}