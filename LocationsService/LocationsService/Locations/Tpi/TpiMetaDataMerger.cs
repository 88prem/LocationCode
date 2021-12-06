namespace Com.Apdcomms.DataGateway.LocationsService.Tpi
{
    public class TpiMetaDataMerger
    {
        public TpiLocationMetaData Merge(
            TpiLocationMetaData existingMetaData,
            TpiLocationMetaData newMetaData)
        {
            return new()
            {
                Bearing = newMetaData.Bearing ?? existingMetaData?.Bearing,
                Fix = newMetaData.Fix ?? existingMetaData?.Fix,
                Inputs = newMetaData.Inputs ?? existingMetaData?.Inputs,
                Latitude = newMetaData.Latitude ?? existingMetaData?.Latitude,
                Longitude = newMetaData.Longitude ?? existingMetaData?.Longitude,
                Outputs = newMetaData.Outputs ?? existingMetaData?.Outputs,
                Speed = newMetaData.Speed ?? existingMetaData?.Speed,
                Status = newMetaData.Status ?? existingMetaData?.Status,
                Timestamp = newMetaData.Timestamp,
                EventText = newMetaData.EventText,
                Resource = newMetaData.Resource,
                Class = newMetaData.Class,
                Contents = newMetaData.Contents,
                Data = newMetaData.Data,
                EventId = newMetaData.EventId,
                GeoData = newMetaData.GeoData ?? existingMetaData?.GeoData
            };
        }
    }
}