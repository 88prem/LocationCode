namespace Com.Apdcomms.StormPipeline.Storm.Exceptions
{
    public class StormMessageMissingMessageCodeException : StormMessageException
    {
        public StormMessageMissingMessageCodeException(string stormMessage)
            : base(stormMessage, "The storm message does not have a message code.")
        {
        }
    }
}
