namespace Com.Apdcomms.StormPipeline.Storm.Exceptions
{
    using System;

    public class NonExistentMessageCodeClassException : Exception
    {
        public NonExistentMessageCodeClassException(string messageCode)
            : base($"Corresponding Message code class does not exist for message code {messageCode}")
        {
        }
    }
}
