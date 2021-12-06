namespace Com.Apdcomms.StormPipeline.Storm.Exceptions
{
    public class StormMessageMissingUniqueIdentifierException : StormMessageException
    {
        public StormMessageMissingUniqueIdentifierException(string stormMessage)
            : base(stormMessage, "The storm message does not have a unique identifier.")
        {
        }
    }
}
