namespace Com.Apdcomms.StormPipeline.Queue.MessageCodes
{

    using Com.Apdcomms.StormPipeline.Parsing;
    using Com.Apdcomms.StormPipeline.Storm.Exceptions;
    using System.Collections.Generic;
    public class StormMetaData
    {
        public string Mid { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string MessageCode { get; set; }
        public object this[string propertyName]
        {
            get
            {
                if (this.GetType().GetProperty(propertyName) is null)
                {
                    throw new NonExistentPropertyException(propertyName);
                }

                return this.GetType().GetProperty(propertyName).GetValue(this, null);
            }
            set
            {
                if (this.GetType().GetProperty(propertyName) is null)
                {
                    throw new NonExistentPropertyException(propertyName);
                }

                this.GetType().GetProperty(propertyName).SetValue(this, value, null);
            }
        }

        public StormMetaData MapFields(StormMessageParser stormMessageParser, Dictionary<string, int> fieldMapping, string[] message)
        {
            foreach (var item in fieldMapping)
            {
                this[item.Key] = item.Value < message.Length ?
                    stormMessageParser.ParseFields(item.Key, message[item.Value], this) : null;
            }
            return this;
        }
    }
}
