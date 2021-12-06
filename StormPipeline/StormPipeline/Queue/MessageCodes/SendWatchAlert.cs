namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class SendWatchAlert : StormMetaData
    {
        public string UserId { get; set; }
        public string Fid { get; set; }
        public string Attribute { get; set; }
        public string ValueBefore { get; set; }
        public string ValueAfter { get; set; }
    }
}
