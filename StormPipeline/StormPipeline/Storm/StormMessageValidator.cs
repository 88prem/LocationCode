namespace Com.Apdcomms.StormPipeline.Storm
{
    using Com.Apdcomms.StormPipeline.Storm.Exceptions;
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class StormMessageValidator
    {
        public void Validate(string stormMessage)
        {
            if (string.IsNullOrWhiteSpace(stormMessage))
            {
                throw new StormMessageNullEmptyOrWhiteSpaceException(stormMessage);
            }

            if (stormMessage.Length > StormConstants.MaxLongFieldLength)
            {
                throw new StormMessageTooLongException(stormMessage);
            }

            var split = stormMessage.Split('|');

            if (split.Length < StormConstants.MessageCode || string.IsNullOrWhiteSpace(split[StormConstants.Indices.MessageCode]))
            {
                throw new StormMessageMissingMessageCodeException(stormMessage);
            }
        }

        public void ValidateUniquField(string stormField)
        {
            if (string.IsNullOrEmpty(stormField))
            {
                throw new StormMessageInvalidUniqueFieldException(stormField);
            }
            if (stormField.Length >= StormConstants.MaxUniqueFieldLength || stormField.Contains(' ') || !CheckFirstLetterAlphaNumeric(stormField))
            {
                throw new StormMessageInvalidUniqueFieldException(stormField);
            }
        }

        public void ValidateDateTimeStampFieldFormat1(string stormField)
        {
            if (!DateTime.TryParseExact(stormField, StormConstants.DateTimeStampFormat1, null, DateTimeStyles.None, out DateTime dateValue))
                throw new StormMessageInvalidDateTimeFieldException(stormField);
        }

        public void ValidateDateTimeStampFieldFormat2(string stormField)
        {
            if (!DateTime.TryParseExact(stormField, StormConstants.DateTimeStampFormat2, null, DateTimeStyles.None, out DateTime dateValue))
                throw new StormMessageInvalidDateTimeFieldException(stormField);
        }

        public void ValidateAttributeField(string[] stormMessage, Dictionary<string, int> mappingConfig)
        {
            var intFinalNorthing = (mappingConfig.TryGetValue("Northing2", out int intNorthing2)) ? intNorthing2 :
                (mappingConfig.TryGetValue("Northing", out int intNorthing)) ? intNorthing : 0;
            intFinalNorthing++;
            for (int i = intFinalNorthing; i < stormMessage.Length; i++)
            {
                var attributeMessage = Convert.ToString(stormMessage[i]);
                if (CheckAttributeField(attributeMessage))
                    throw new StormAttributeMessageTooLongException(attributeMessage);
            }
        }

        private bool CheckAttributeField(string strAttributeField) 
        {
            if (strAttributeField.Length > StormConstants.MaxAttributeFieldLength)
                return true;
            else
                return false;
        }

        private bool CheckFirstLetterAlphaNumeric(string stormField)
        {
            var firtsLetter = (stormField.ToCharArray())[0];
            if (Char.IsDigit(firtsLetter) || Char.IsLetter(firtsLetter))
                return true;
            else
                return false;
        }
    }
}