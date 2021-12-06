namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class UpdateResource : StormMetaData
    {
        public string Label { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
        public string StationId { get; set; }
        public string IncidentId { get; set; }
        public string StandbyId { get; set; }
        public double? Speed { get; set; }
        public double? Bearing { get; set; }
        public string UpdateTime { get; set; }
        public string DeviceId { get; set; }
        public string UrId { get; set; }
        public string TagIndicator { get; set; }
        public string Mode { get; set; }
        public string Proposal { get; set; }
        public string Group { get; set; }
        public string ControlArea { get; set; }
    }
}
