namespace Com.Apdcomms.DataGateway.LocationsService.Notifications
{
    using System;
    using Com.Apdcomms.DataGateway.LocationsService.Exceptions;
    using System.Globalization;
    using System.Threading;

    public class NotificationValidator
    {
        public void Validate(Notification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            if (string.IsNullOrWhiteSpace(notification.Source))
            {
                throw new MissingSourceException(notification);
            }

            if (string.IsNullOrWhiteSpace(notification.SourceId))
            {
                throw new MissingSourceIdException(notification);
            }
            
            ValidateMetaData(notification);
        }

        private void ValidateMetaData(Notification notification)
        {
            switch (notification.Source)
            {
                case "TPI":
                    if (notification.TpiMetaData == null)
                    {
                        throw new MissingMetaDataException(notification);
                    }

                    DateTime dateValue;
                    if (!DateTime.TryParse(notification.TpiMetaData.Timestamp,
                        Thread.CurrentThread.CurrentUICulture,
                        DateTimeStyles.None,
                        out dateValue))
                    {
                        throw new InvalidDateTimeFormatException(notification);
                    }

                    break;
                case "LTE":
                    if ( notification.LteMetaData == null)
                    {
                        throw new MissingMetaDataException(notification);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}