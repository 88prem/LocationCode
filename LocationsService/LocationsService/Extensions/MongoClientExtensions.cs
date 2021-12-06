namespace Com.Apdcomms.DataGateway.LocationsService.Extensions
{
    using MongoDB.Driver;

    public static class MongoClientExtensions
    {
        public static IMongoCollection<Location> GetLocationsCollection(this IMongoClient client)
        {
            return client.GetDatabase($"{nameof(Location)}").GetCollection<Location>($"{nameof(Location)}");
        }
    }
}