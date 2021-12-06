namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class CreateAssociatedIcon : StormMetaData
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string IconId { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
        public string Url { get; set; }
    }
}
