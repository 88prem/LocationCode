namespace Com.Apdcomms.DataGateway.LocationsService.Exceptions
{
    using System;
    using Com.Apdcomms.DataGateway.LocationsService.Notifications;

    public class InvalidNotificationException : Exception
    {
        public Notification Notification { get; }
        
        public InvalidNotificationException(Notification notification)
        {
            Notification = notification;
        }
    }
}