namespace Com.Apdcomms.DataGateway.LocationsService.Lte
{
    using System;

    [Serializable]
    public record LteLocationMetaData
    {
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string ReportType { get; init; }

        public string UserID { get; init; }

        public string UserDisplayName { get; init; }

        public string GroupID { get; init; }

        public string GroupDisplayName { get; init; }
    }
}