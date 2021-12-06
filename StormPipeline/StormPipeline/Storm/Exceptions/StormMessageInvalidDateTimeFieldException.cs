namespace Com.Apdcomms.StormPipeline.Storm.Exceptions
{
    public class StormMessageInvalidDateTimeFieldException : StormMessageException
    {
        public StormMessageInvalidDateTimeFieldException(string stormMessage)
            : base(stormMessage, "The storm message has invalid date time field.")
        {
        }
    }
}
