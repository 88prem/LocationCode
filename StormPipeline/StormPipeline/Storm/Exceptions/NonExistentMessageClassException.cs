namespace Com.Apdcomms.StormPipeline.Storm.Exceptions
{
    using System;

    public class NonExistentMessageClassException : Exception
    {
        public NonExistentMessageClassException(string messageCode)
            : base($"Corresponding Message class does not exist for message code {messageCode}")
        {
        }
    }
}
