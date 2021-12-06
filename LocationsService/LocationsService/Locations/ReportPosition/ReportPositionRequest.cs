namespace Com.Apdcomms.DataGateway.LocationsService.ReportPosition
{
    using MediatR;

    public record ReportPositionRequest : IRequest
    {
        /// <summary>
        /// The ResourceId that you would like to report their location.
        /// </summary>
        /// <example>PatrolCar1</example>
        public string SourceId { get; init; }
        
        /// <summary>
        /// The target interface for this Report Position Request
        /// </summary>
        /// <example>TPI</example>
        public string TargetInterface { get; init; }
    }
}