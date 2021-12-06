namespace Com.Apdcomms.DataGateway.LocationsService.WebApi
{
    using System.Collections.Generic;
    using MediatR;
    using Location = Com.Apdcomms.DataGateway.LocationsService.Location;

    public class GetLocationsRequest : IRequest<IEnumerable<Location>>
    {
        
    }
}