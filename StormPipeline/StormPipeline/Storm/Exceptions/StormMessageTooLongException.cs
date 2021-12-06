namespace Com.Apdcomms.StormPipeline.Storm.Exceptions
{
    public class StormMessageTooLongException : StormMessageException
    {
        public StormMessageTooLongException(string stormMessage)
            : base(stormMessage, "The storm message length exceeds 2048 characters.")
        {
        }
    }
}
