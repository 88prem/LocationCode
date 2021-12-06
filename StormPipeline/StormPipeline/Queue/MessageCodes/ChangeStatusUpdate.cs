namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class ChangeStatusUpdate : StormMetaData
    {
        public string Status { get; set; }
        public string Unit { get; set; }
        public string Rate { get; set; }
    }
}
