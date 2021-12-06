namespace Com.Apdcomms.DataGateway.LocationsService.Queue
{
    using Com.Apdcomms.DataGateway.LocationsService.Lte;
    using Com.Apdcomms.DataGateway.LocationsService.Tpi;

    public class LocationToQueueLocationMapper
    {
        public Com.Apdcomms.DataGateway.QueueTypes.Surface.Locations.Location Map(Location location) =>
            new()
            {
                LocationId = location.LocationId,
                Source = location.Source,
                Timestamp = location.Timestamp,
                SourceId = location.SourceId,
                TpiMetaData = location.Source == "TPI" ? MapTpiMetaData(location.TpiMetaData):null,
                LteMetaData = location.Source == "LTE" ? MapLteMetaData(location.LteMetaData): null,
                IsHistoricLocationUpdate = location.IsHistoricLocationUpdate
            };

        private Com.Apdcomms.DataGateway.QueueTypes.Surface.Locations.TpiLocationMetaData MapTpiMetaData(
            TpiLocationMetaData metaData) =>
            new()
            {
                Bearing = metaData.Bearing,
                Fix = metaData.Fix,
                Inputs = metaData.Inputs,
                Latitude = metaData.Latitude,
                Longitude = metaData.Longitude,
                Outputs = metaData.Outputs,
                Speed = metaData.Speed,
                Status = metaData.Status,
                Timestamp = metaData.Timestamp,
                EventText = metaData.EventText,
                Resource = metaData.Resource,
                Class = metaData.Class,
                Contents = metaData.Contents,
                Data = metaData.Data,
                EventId = metaData.EventId,
                GeoData = metaData.GeoData
            };

        private Com.Apdcomms.DataGateway.QueueTypes.Surface.Locations.LteLocationMetaData MapLteMetaData(
            LteLocationMetaData metaData) =>
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
    }
}