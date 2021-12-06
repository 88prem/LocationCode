namespace Com.Apdcomms.StormPipeline.Parsing
{
    using Com.Apdcomms.StormPipeline.Parsing.Factory;
    using Com.Apdcomms.StormPipeline.Storm;
    using Com.Apdcomms.StormPipeline.Storm.Exceptions;
    using Newtonsoft.Json;
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class StormMessageHelper
    {
        public static string[] GetMessageArray(string message)
        {
            var messageArray = new string(message.Where(c => !char.IsControl(c)).ToArray()).Split('|');
            if (string.IsNullOrEmpty(messageArray[messageArray.Length - 1]))
            {
                Array.Resize(ref messageArray, messageArray.Length - 1);
            }
            if (messageArray.Length > StormConstants.Indices.MessageCode && messageArray[StormConstants.Indices.MessageCode].Equals(MessageCodes.RRA.ToString()))
            {
                return FormatRRAMessage(message);
            }
            return messageArray;
        }
        private static string[] FormatRRAMessage(string message)
        {
            var messageStringBuilder = new StringBuilder(message);
            var objectString = GetStringInBetween(message);
            messageStringBuilder = messageStringBuilder.Replace(objectString, string.Empty);
            var messageRRAArray = new string(messageStringBuilder.ToString().Where(c => !char.IsControl(c)).ToArray()).Split('|');
            Array.Resize(ref messageRRAArray, messageRRAArray.Length);
            messageRRAArray[messageRRAArray.Length - 1] = string.Concat(messageRRAArray[messageRRAArray.Length - 1] + objectString);
            return messageRRAArray;
        }
        private static string GetStringInBetween(string strSource)
        {
            Regex regex = new Regex(@"(?<=\[).*(?=\])", RegexOptions.Compiled);
            return string.Concat("[", regex.Matches(strSource)[0].Value, "]");
        }
        public static string GetMessageCode(string[] messageArray)
        {
            if (messageArray.Length < StormConstants.MessageCode || string.IsNullOrWhiteSpace(messageArray[StormConstants.Indices.MessageCode]))
            {
                throw new StormMessageMissingMessageCodeException("Invalid message since it does not contain message code");
            }

            return messageArray[StormConstants.Indices.MessageCode];
        }

        public static T JsonParserConvert<T>(Object source)
        {
            var json = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string GetUniqueIdentifier(string message)
        {
            var messageArray = GetMessageArray(message);
            if (messageArray.Length < StormConstants.UniqueIdentifier || string.IsNullOrWhiteSpace(messageArray[StormConstants.Indices.UniqueIdentifier]))
            {
                throw new StormMessageMissingUniqueIdentifierException("Invalid message since it does not contain unique identifier");
            }

            return messageArray[StormConstants.Indices.UniqueIdentifier];
        }
    }
}
