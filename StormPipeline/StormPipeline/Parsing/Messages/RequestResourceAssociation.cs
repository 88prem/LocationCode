namespace Com.Apdcomms.StormPipeline.Parsing.Messages
{    
    using Com.Apdcomms.StormPipeline.Queue.MessageCodes;
    using Com.Apdcomms.StormPipeline.Storm;
    using Serilog;
    using System.Collections.Generic;

    public class RequestResourceAssociation : IOperate
    {
        private readonly ILogger logger;
        private readonly StormMessageValidator stormMessageValidator;

        public RequestResourceAssociation(ILogger logger, StormMessageValidator stormMessageValidator)
        {
            this.logger = logger;
            this.stormMessageValidator = stormMessageValidator;
        }

        public StormMetaData Operate(StormMetaData stormMetaData)
        {
            logger.Information("RRA processed");
            return stormMetaData;
        }

        public void Validate(StormMetaData stormMetaData, string[] stormMessage, Dictionary<string, int> mappingConfig)
        {
            stormMessageValidator.ValidateUniquField(((Queue.MessageCodes.RequestResourceAssociation)stormMetaData).Label);
        }
    }
}
