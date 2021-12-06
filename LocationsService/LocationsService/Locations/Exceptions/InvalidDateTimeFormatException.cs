namespace Com.Apdcomms.DataGateway.LocationsService.Exceptions
{
    using Com.Apdcomms.DataGateway.LocationsService.Notifications;

    public class InvalidDateTimeFormatException : InvalidNotificationException
    {
        public InvalidDateTimeFormatException(Notification notification) : base(notification)
        {
        }
    }
}