namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class ProcessIncidentLogInformation : StormMetaData
    {
        public string IncidentId { get; set; }
        public string LogEntryId { get; set; }
        public string SegmentId { get; set; }
        public string OrderId { get; set; }
        public string Username { get; set; }
        public string Timestamp { get; set; }
        public string Text { get; set; }
    }
}
