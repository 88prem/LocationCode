namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class CreateIncidentTwo : StormMetaData
    {
        public string Name { get; set; }
        public string Class { get; set; }
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
