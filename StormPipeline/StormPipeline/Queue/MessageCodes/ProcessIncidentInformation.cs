namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class ProcessIncidentInformation : StormMetaData
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
        public string GradeOfResponse { get; set; }
        public string RelativeResource { get; set; }
        public string IncidentDate { get; set; }
        public string IncidentCategory { get; set; }
        public string PriorityFlag { get; set; }
        public string Pursuit { get; set; }
        public string ControlArea { get; set; }
    }
}
