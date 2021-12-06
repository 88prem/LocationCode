namespace Com.Apdcomms.DataGateway.LocationsService.Lte
{
    public class LteMetaDataMerger
    {
        public LteLocationMetaData Merge(
            LteLocationMetaData existingMetaData,
            LteLocationMetaData newMetaData)
        {
            return new()
            {
                ReportType = newMetaData.ReportType ?? existingMetaData.ReportType,
                GroupDisplayName = newMetaData.GroupDisplayName,
                GroupID = newMetaData.GroupID,
                UserDisplayName = newMetaData.UserDisplayName,
                UserID = newMetaData.UserID,
                Latitude = newMetaData.Latitude ?? existingMetaData.Latitude,
                Longitude = newMetaData.Longitude ?? existingMetaData.Longitude
            };
        }
    }
}