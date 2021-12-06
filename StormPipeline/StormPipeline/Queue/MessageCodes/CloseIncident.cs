namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class CloseIncident : StormMetaData
    {
        public string IncidentId { get; set; }
        public string IncidentCode { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
        public string DateTime { get; set; }
    }
}
