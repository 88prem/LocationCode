namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class RequestIncidentLogAddition : StormMetaData
    {
        public string IncidentId { get; set; }
        public string Username { get; set; }
        public string Timestamp { get; set; }
    }
}
