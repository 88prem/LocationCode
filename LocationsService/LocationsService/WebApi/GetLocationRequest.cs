namespace Com.Apdcomms.DataGateway.LocationsService.WebApi
{
    using MediatR;

    public class GetLocationRequest : IRequest<Location>
    {
        public string LocationId { get; set; }
    }
}