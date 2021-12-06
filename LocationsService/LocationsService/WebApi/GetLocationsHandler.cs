namespace Com.Apdcomms.DataGateway.LocationsService.WebApi
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService.Database;
    using MediatR;

    public class GetLocationsHandler : IRequestHandler<GetLocationsRequest, IEnumerable<Location>>
    {
        private readonly IDatabase locationsDb;

        public GetLocationsHandler(IDatabase locationsDb)
        {
            this.locationsDb = locationsDb;
        } 
        
        public async Task<IEnumerable<Location>> Handle(GetLocationsRequest request, CancellationToken cancellationToken)
        {
            var locations = await this.locationsDb.GetAll();

            return locations;
        }
    }
}