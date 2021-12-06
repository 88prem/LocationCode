namespace Com.Apdcomms.StormPipeline.Storm.Exceptions
{
    public class StormMessageNullEmptyOrWhiteSpaceException : StormMessageException
    {
        public StormMessageNullEmptyOrWhiteSpaceException(string stormMessage)
            : base(stormMessage, "The storm message is null, empty or whitespace.")
        {
        }
    }
}
