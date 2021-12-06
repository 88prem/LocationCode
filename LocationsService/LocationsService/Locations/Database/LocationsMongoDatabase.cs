namespace Com.Apdcomms.DataGateway.LocationsService.Database
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService.Extensions;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;

    public class LocationsMongoDatabase : IDatabase
    {
        private readonly MongoClient mongoClient;
        private readonly ILogger<LocationsMongoDatabase> logger;

        public LocationsMongoDatabase(
            MongoClient mongoClient,
            ILogger<LocationsMongoDatabase> logger)
        {
            this.mongoClient = mongoClient;
            this.logger = logger;
        }

        public async Task<Location> Get(string locationId)
        {
            logger.LogTrace("Received request for location {LocationId}", locationId);
            var locations = mongoClient.GetLocationsCollection();
            var location = await locations.Find(
                    Builders<Location>.Filter
                        .Where(x => x.LocationId == locationId))
                .SingleOrDefaultAsync();
            return location;
        }

        public async Task<IEnumerable<Location>> GetAll()
        {
            this.logger.LogTrace("Received request for all locations");
			var collection = this.mongoClient.GetLocationsCollection();
			var items = await collection
				.Find(_ => true)
				.ToListAsync();

			return items;
        }

        public async Task Update(Location location)
        {
            logger.LogTrace(
                "Updating mongodb entry for {LocationId}. Replacing with {@Location}",
                location.LocationId,
                location);
            
            var locations = mongoClient.GetLocationsCollection();
            await locations.FindOneAndReplaceAsync(
                Builders<Location>.Filter
                    .Where(x => x.LocationId == location.LocationId),
                location);
            
            await ApplyIndices();
        }

        public async Task Create(Location location)
        {
            logger.LogTrace("Created a new location {@Location}", location);
            var locations = mongoClient.GetLocationsCollection();
            await locations.InsertOneAsync(location);
            await ApplyIndices();
        }

        private async Task ApplyIndices()
        {
            var locations = mongoClient.GetLocationsCollection();
            var indexDefinition = Builders<Location>.IndexKeys.Hashed(x => x.LocationId);
            var indexModel = new CreateIndexModel<Location>(indexDefinition);
            await locations.Indexes.CreateOneAsync(indexModel);
        }
    }
}