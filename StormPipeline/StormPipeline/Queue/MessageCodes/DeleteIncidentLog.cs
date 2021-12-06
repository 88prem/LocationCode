namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class DeleteIncidentLog : StormMetaData
    {
        public string IncidentId { get; set; }
        public string LogEntryId { get; set; }
    }
}
