namespace Com.Apdcomms.StormPipeline.Storm.Exceptions
{
    using System;

    public class StormMessageTooManyFieldsException : Exception
    {
        public StormMessageTooManyFieldsException(string messageCode)
            : base($"Number of message fields are greater than mapped fields for {messageCode}")
        {
        }
    }
}
