namespace Com.Apdcomms.StormPipeline.Storm.Exceptions
{
    using System;

    public class NonExistentPropertyException : Exception
    {
        public NonExistentPropertyException(string propertyName)
            : base($"Property {propertyName} does not exist in model")
        {
        }
    }
}
