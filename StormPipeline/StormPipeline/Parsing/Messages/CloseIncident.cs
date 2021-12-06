namespace Com.Apdcomms.StormPipeline.Parsing.Messages
{
    using Com.Apdcomms.StormPipeline.Queue.MessageCodes;
    using Com.Apdcomms.StormPipeline.Storm;
    using Serilog;
    using System.Collections.Generic;

    public class CloseIncident : IOperate
    {
        private readonly ILogger logger;
        private readonly StormMessageValidator stormMessageValidator;

        public CloseIncident(ILogger logger, StormMessageValidator stormMessageValidator)
        {
            this.logger = logger;
            this.stormMessageValidator = stormMessageValidator;
        }

        public StormMetaData Operate(StormMetaData stormMetaData)
        {
            logger.Information("CLI processed");
            return stormMetaData;
        }

        public void Validate(StormMetaData stormMetaData, string[] stormMessage, Dictionary<string, int> fieldMapping)
        {
            stormMessageValidator.ValidateDateTimeStampFieldFormat2(((Queue.MessageCodes.CloseIncident)stormMetaData).DateTime);
        }
    }
}
