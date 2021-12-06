namespace Com.Apdcomms.DataGateway.LocationsService.Refresh
{
    using MediatR;

    public class LocationsRefreshRequest : IRequest
    {
        /// <summary>
        /// The target interface for this Request
        /// </summary>
        /// <example>TPI</example>
        public string TargetInterface { get; init; }
    }
}