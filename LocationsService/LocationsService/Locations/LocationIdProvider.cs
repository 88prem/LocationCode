namespace Com.Apdcomms.DataGateway.LocationsService
{
    using System;
    using System.Text;

    public static class LocationIdProvider
    {
        public static string Get(string source, string sourceId) =>
            Convert.ToBase64String(
                Encoding.UTF8.GetBytes(string.Concat(source, sourceId)));
    }
}