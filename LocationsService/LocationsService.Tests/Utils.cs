namespace Com.Apdcomms.LocationsService.Tests
{
    using System;
    using System.Text;

    public static class Utils
    {
        /// <summary>
        /// https://dev.azure.com/apdcommunications/Data%20Gateway/_wiki/wikis/Data-Gateway.wiki/1402/Consumer-API-Definition
        /// </summary>
        public static string GetLocationId(string source, string sourceId) =>
            Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Concat(source, sourceId)));
    }
}