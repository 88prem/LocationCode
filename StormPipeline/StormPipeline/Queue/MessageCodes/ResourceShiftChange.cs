namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{
    public class ResourceShiftChange : StormMetaData
    {
        public string CallSign { get; set; }
        public string NextEndShift { get; set; }
    }
}
