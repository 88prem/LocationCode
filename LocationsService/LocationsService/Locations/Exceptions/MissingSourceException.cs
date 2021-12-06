namespace Com.Apdcomms.DataGateway.LocationsService.Exceptions
{
    using Com.Apdcomms.DataGateway.LocationsService.Notifications;

    public class MissingSourceException : InvalidNotificationException
    {
        public MissingSourceException(Notification notification) : base(notification)
        {
        }
    }
}