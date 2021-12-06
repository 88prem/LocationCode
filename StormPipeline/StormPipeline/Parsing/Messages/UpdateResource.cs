namespace Com.Apdcomms.StormPipeline.Parsing.Messages
{
    using Com.Apdcomms.StormPipeline.Queue.MessageCodes;
    using Com.Apdcomms.StormPipeline.Storm;
    using Serilog;
    using System.Collections.Generic;

    public class UpdateResource : IOperate
    {
        private readonly ILogger logger;
        private readonly StormMessageValidator stormMessageValidator;

        public UpdateResource(ILogger logger, StormMessageValidator stormMessageValidator)
        {
            this.logger = logger;
            this.stormMessageValidator = stormMessageValidator;
        }

        public StormMetaData Operate(StormMetaData stormMetaData)
        {
            logger.Information("UR processed");
            return stormMetaData;
        }

        public void Validate(StormMetaData stormMetaData, string[] stormMessage, Dictionary<string, int> mappingConfig)
        {
            stormMessageValidator.ValidateUniquField(((Queue.MessageCodes.UpdateResource)stormMetaData).Label);
            stormMessageValidator.ValidateUniquField(((Queue.MessageCodes.UpdateResource)stormMetaData).Type);
            stormMessageValidator.ValidateAttributeField(stormMessage, mappingConfig);
            stormMessageValidator.ValidateDateTimeStampFieldFormat1(((Queue.MessageCodes.UpdateResource)stormMetaData).UpdateTime);
        }
    }
}
