namespace Com.Apdcomms.StormPipeline.Storm.Exceptions
{
    public class StormMessageInvalidUniqueFieldException : StormMessageException
    {
        public StormMessageInvalidUniqueFieldException(string stormMessage)
            : base(stormMessage, "The storm message has invalid unique field.")
        {
        }
    }
}
