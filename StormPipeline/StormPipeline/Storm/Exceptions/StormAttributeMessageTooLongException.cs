namespace Com.Apdcomms.StormPipeline.Storm.Exceptions
{
    public class StormAttributeMessageTooLongException : StormMessageException
    {
        public StormAttributeMessageTooLongException(string stormMessage)
            : base(stormMessage, "The storm attribute message length exceeds 128 characters.")
        {
        }
    }
}
