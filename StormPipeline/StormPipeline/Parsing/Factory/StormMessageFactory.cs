namespace Com.Apdcomms.StormPipeline.Parsing.Factory
{
    using Com.Apdcomms.StormPipeline.Storm;
    using Com.Apdcomms.StormPipeline.Storm.Exceptions;
    using Serilog;
    using System;

    public class StormMessageFactory<T> : IStormMessageFactory<T>
    {
        private static readonly string messageNamespace = "Com.Apdcomms.StormPipeline.Parsing.Messages";
        private readonly ILogger logger;
        private readonly StormMessageValidator stormMessageValidator;

        public StormMessageFactory(ILogger logger, StormMessageValidator stormMessageValidator)
        {
            this.logger = logger;
            this.stormMessageValidator = stormMessageValidator;
        }

        public T Create(string messageCode)
        {
            try
            {
                var enumMessageCode = (MessageCodes)Enum.Parse(typeof(MessageCodes), messageCode, true);
                var messageClass = Type.GetType(string.Format("{0}.{1}", messageNamespace, StormFactoryIndex.messageAbbrievations[enumMessageCode]));
                if (messageClass is null)
                {
                    throw new NonExistentMessageClassException(messageCode);
                }
                return (T)Activator.CreateInstance(messageClass, logger, stormMessageValidator);
            }
            catch (NonExistentMessageClassException)
            {
                logger.Warning($"Corresponding Message class does not exist for message code {messageCode}");
                throw new NonExistentMessageClassException(messageCode);
            }
            catch (Exception)
            {
                logger.Warning($"Message Code {messageCode} is not supported");
                throw new NotSupportedException($"Message Code {messageCode} is not supported");
            }
        }
    }
}
