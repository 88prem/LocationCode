namespace Com.Apdcomms.StormPipeline.Parsing.Factory
{
    using Com.Apdcomms.StormPipeline.Storm.Exceptions;
    using Serilog;
    using System;

    public class StormMessageCodeFactory<T> : IStormMesageCodeFactory<T>
    {
        private readonly ILogger logger;

        public StormMessageCodeFactory(ILogger logger)
        {
            this.logger = logger;
        }

        public T CreateCode(string messageCode, string namespaceName)
        {
            try
            {
                var enumMessageCode = (MessageCodes)Enum.Parse(typeof(MessageCodes), messageCode, true);
                var messageCodeClass = Type.GetType(string.Format("{0}.{1}", namespaceName,
                    StormFactoryIndex.messageAbbrievations[enumMessageCode]));
                if (messageCodeClass is null)
                {
                    throw new NonExistentMessageCodeClassException(messageCode);
                }
                return (T)Activator.CreateInstance(messageCodeClass);
            }
            catch (NonExistentMessageClassException)
            {
                logger.Warning($"Corresponding Message code class does not exist for message code {messageCode}");
                throw new NonExistentMessageCodeClassException(messageCode);
            }
            catch (Exception)
            {
                logger.Warning($"Message Code {messageCode} class is not supported");
                throw new NotSupportedException($"Message Code {messageCode} class is not supported");
            }
        }
    }
}
