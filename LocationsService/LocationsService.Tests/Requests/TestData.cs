namespace Com.Apdcomms.LocationsService.Tests.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Com.Apdcomms.DataGateway.LocationsService;
    using Com.Apdcomms.DataGateway.LocationsService.Tpi;

    public static class TestData
    {
        public static List<Location> DataWith5Locations { get; } = CreateLocationData(5);

        private static List<Location> CreateLocationData(int artemisDevices)
        {
            var result = new List<Location>();
            for (var i = 0; i < artemisDevices; i++)
            {
                result.Add(CreateArtemisLocation("device" + i));
            }

            return result;
        }

        private static Location CreateArtemisLocation(string deviceName) => new()
        {
            Source = "TPI",
            SourceId = deviceName,
            Timestamp = new DateTime(2000, 01, 01),
            LocationId = LocationIdProvider.Get("TPI", deviceName),
            TpiMetaData = new TpiLocationMetaData
            {
                Resource = deviceName,
                Bearing = 109,
                Fix = 5,
                Inputs = 12,
                Latitude = 53.12,
                Longitude = -2.3423,
                Outputs = 5,
                Speed = 12,
                Status = 4,
                EventText = "some event text",
                Timestamp = new DateTime(2000, 01, 01).ToString(CultureInfo.InvariantCulture),
                Class = "SomeClass",
                Contents = 4,
                Data = "Some Data",
                EventId = 15,
                GeoData = 3
            },
            LteMetaData = null
        };

        public static Location LocationDeviceA { get; } = CreateArtemisLocation("DeviceA");
    }
}