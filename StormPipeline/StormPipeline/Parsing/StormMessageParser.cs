namespace Com.Apdcomms.StormPipeline.Parsing
{    
    using Serilog;
    using System;
    using System.Globalization;

    public class StormMessageParser
    {
        private readonly ILogger logger;

        public StormMessageParser(ILogger logger)
        {
            this.logger = logger;
        }

        public dynamic ParseFields(string propertyName, string propertyValue, Object incidentMetaData)
        {
            try
            {
                var isNullableType = Nullable.GetUnderlyingType(incidentMetaData.GetType().GetProperty(propertyName).PropertyType);
                var propertyType = isNullableType ?? incidentMetaData.GetType().GetProperty(propertyName).PropertyType;
                return GetParsedValue(propertyType, propertyValue);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "--- Storm Pipeline: Error in parsing fields, Error : {ErrorMessage} ---", ex.Message);
                throw;
            }
        }

        private dynamic GetParsedValue(Type propertyType, string propertyValue)
        {
            try
            {
                switch (Type.GetTypeCode(propertyType))
                {
                    case TypeCode.Double:
                        _ = double.TryParse(propertyValue, out var doubleValue);
                        return doubleValue;
                    case TypeCode.Int32:
                        _ = int.TryParse(propertyValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intValue);
                        return intValue;
                    default:
                        return propertyValue;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "--- Storm Pipeline: Error in parsing value Error : {ErrorMessage} ---", ex.Message);
                throw;
            }
        }
    }
}
