namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class CreateDynamicObject : StormMetaData
    {
        public string Id { get; set; }
        public string Class { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
    }
}
