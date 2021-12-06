namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class CreateIncident : StormMetaData
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
    }
}
