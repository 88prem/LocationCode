namespace Com.Apdcomms.StormPipeline.Parsing.Messages
{
    using Com.Apdcomms.StormPipeline.Queue.MessageCodes;
    using System.Collections.Generic;

    public interface IOperate
    {
        StormMetaData Operate(StormMetaData stormMetaData);
        void Validate(StormMetaData stormMetaData, string[] stormMessage, Dictionary<string,int> fieldMapping);
    }
}
