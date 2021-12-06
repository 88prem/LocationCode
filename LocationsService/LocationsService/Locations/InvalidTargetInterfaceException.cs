namespace Com.Apdcomms.DataGateway.LocationsService
{
    using System;

    public class InvalidTargetInterfaceException : Exception
    {
        public InvalidTargetInterfaceException(string targetInterface) : base(
            $"Provided an invalid target interface - '{targetInterface}'")
        {
        }
    }
}