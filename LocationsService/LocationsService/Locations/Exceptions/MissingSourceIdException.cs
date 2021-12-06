namespace Com.Apdcomms.DataGateway.LocationsService.Exceptions
{
    using Com.Apdcomms.DataGateway.LocationsService.Notifications;

    public class MissingSourceIdException : InvalidNotificationException
    {
        public MissingSourceIdException(Notification notification) : base(notification)
        {
        }
    }
}