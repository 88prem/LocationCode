namespace Com.Apdcomms.StormPipeline.Parsing.Messages
{
    using Com.Apdcomms.StormPipeline.Queue.MessageCodes;
    using Com.Apdcomms.StormPipeline.Storm;
    using Serilog;
    using System.Collections.Generic;

    public class CreateResourceTwo : IOperate
    {
        private readonly ILogger logger;
        private readonly StormMessageValidator stormMessageValidator;

        public CreateResourceTwo(ILogger logger, StormMessageValidator stormMessageValidator)
        {
            this.logger = logger;
            this.stormMessageValidator = stormMessageValidator;
        }

        public StormMetaData Operate(StormMetaData stormMetaData)
        {
            logger.Information("CR2 processed");
            return stormMetaData;
        }

        public void Validate(StormMetaData stormMetaData, string[] stormMessage, Dictionary<string, int> mappingConfig)
        {
            stormMessageValidator.ValidateUniquField(((Queue.MessageCodes.CreateResourceTwo)stormMetaData).Name);
            stormMessageValidator.ValidateUniquField(((Queue.MessageCodes.CreateResourceTwo)stormMetaData).Class);
            stormMessageValidator.ValidateAttributeField(stormMessage, mappingConfig);
            stormMessageValidator.ValidateDateTimeStampFieldFormat1(((Queue.MessageCodes.CreateResourceTwo)stormMetaData).UpdateTime);
        }
    }
}
