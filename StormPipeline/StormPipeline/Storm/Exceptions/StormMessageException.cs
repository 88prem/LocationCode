namespace Com.Apdcomms.StormPipeline.Storm.Exceptions
{
    using System;

    public class StormMessageException : Exception
    {
        public string StormMessage { get; }

        public StormMessageException(string stormMessage, string message)
            : base(message)
        {
            StormMessage = stormMessage;
        }

        public StormMessageException(string tpiMessage)
            : this(tpiMessage, "Failed to process Storm message.")
        {
        }
    }
}
