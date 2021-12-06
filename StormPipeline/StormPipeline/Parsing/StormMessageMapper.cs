namespace Com.Apdcomms.StormPipeline.Parsing
{
    using Com.Apdcomms.StormPipeline.Parsing.Factory;
    using Com.Apdcomms.StormPipeline.Parsing.Messages;    
    using Com.Apdcomms.StormPipeline.Queue.MessageCodes;
    using Com.Apdcomms.StormPipeline.Storm.Exceptions;
    using Microsoft.Extensions.Options;
    using Serilog;
    using System;

    public class StormMessageMapper
    {
        private readonly ILogger logger;
        private readonly StormMessageParser stormMessageParser;
        private readonly IStormMessageFactory<IOperate> stormMessageFactory;
        private readonly IStormMesageCodeFactory<StormMetaData> stormMesageCodeFactory;
        private readonly MappingConfig options;

        public StormMessageMapper(ILogger logger, StormMessageParser stormMessageParser, IOptions<MappingConfig> options,
            IStormMessageFactory<IOperate> stormMessageFactory, IStormMesageCodeFactory<StormMetaData> stormMesageCodeFactory)
        {
            this.logger = logger;
            this.stormMessageParser = stormMessageParser;
            this.stormMessageFactory = stormMessageFactory;
            this.stormMesageCodeFactory = stormMesageCodeFactory;
            this.options = options.Value;
        }

        public StormMetaData Map(string[] message, string messageCode)
        {
            try
            {
                logger.Information("Mapping fields for Message Code - {code}", messageCode);
                var stormData = stormMesageCodeFactory.CreateCode(messageCode, "Com.Apdcomms.StormPipeline.Queue.MessageCodes");
                var fieldMapping = options[messageCode];

                if (fieldMapping.Count < message.Length)
                {
                    throw new StormMessageTooManyFieldsException(messageCode);
                }
                stormData = stormData.MapFields(stormMessageParser, fieldMapping, message);
                return stormData;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "--- Storm Pipeline: Error in mapping fields, Error : {ErrorMessage} ---", ex.Message);
                throw;
            }
        }
        public StormMetaData OperateStormMessage(IOperate messageClass, StormMetaData stormData,
            string[] message, string messageCode)
        {
            try
            {
                var fieldMapping = options[messageCode];
                messageClass.Validate(stormData, message, fieldMapping);
            }
            catch (StormMessageException e)
            {
                logger.Warning(e, "Invalid message received from Storm");
                throw;
            }
            var result = messageClass.Operate(stormData);
            return result;
        }

        public (string[], string, IOperate) GetMessageData(string stormMessage)
        {
            var message = StormMessageHelper.GetMessageArray(stormMessage);
            var messageCode = StormMessageHelper.GetMessageCode(message);
            var messageClass = stormMessageFactory.Create(messageCode);

            return (message, messageCode, messageClass);
        }
    }
}
