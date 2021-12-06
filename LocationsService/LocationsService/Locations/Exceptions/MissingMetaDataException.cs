namespace Com.Apdcomms.DataGateway.LocationsService.Exceptions
{
    using Com.Apdcomms.DataGateway.LocationsService.Notifications;

    public class MissingMetaDataException : InvalidNotificationException
    {
        public MissingMetaDataException(Notification notification) : base(notification)
        {
        }
    }
}