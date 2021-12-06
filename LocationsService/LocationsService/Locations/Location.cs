namespace Com.Apdcomms.DataGateway.LocationsService
{
    using System;
    using Com.Apdcomms.DataGateway.LocationsService.Lte;
    using Com.Apdcomms.DataGateway.LocationsService.Tpi;
    using MongoDB.Bson.Serialization.Attributes;

    [Serializable]
    [BsonIgnoreExtraElements]
    public record Location
    {
        public string LocationId { get; init; }
        
        public string Source { get; init; }
        
        public string SourceId { get; init; }
        
        public TpiLocationMetaData TpiMetaData { get; init; }

        public LteLocationMetaData LteMetaData { get; init; }

        public DateTime Timestamp { get; init; }

        public bool IsHistoricLocationUpdate { get; init; }
    }
}