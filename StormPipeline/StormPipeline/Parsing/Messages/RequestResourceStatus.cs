﻿namespace Com.Apdcomms.StormPipeline.Parsing.Messages
{    
    using Com.Apdcomms.StormPipeline.Queue.MessageCodes;
    using Com.Apdcomms.StormPipeline.Storm;
    using Serilog;
    using System.Collections.Generic;

    public class RequestResourceStatus : IOperate
    {
        private readonly ILogger logger;
        private readonly StormMessageValidator stormMessageValidator;

        public RequestResourceStatus(ILogger logger, StormMessageValidator stormMessageValidator)
        {
            this.logger = logger;
            this.stormMessageValidator = stormMessageValidator;
        }

        public StormMetaData Operate(StormMetaData stormMetaData)
        {
            logger.Information("RRS processed");
            return stormMetaData;
        }

        public void Validate(StormMetaData stormMetaData, string[] stormMessage, Dictionary<string, int> fieldMapping) { } 
    }
}
