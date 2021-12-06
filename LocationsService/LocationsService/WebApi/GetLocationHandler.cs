namespace Com.Apdcomms.DataGateway.LocationsService.WebApi
{
    using System.Threading;
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService.Database;
    using MediatR;

    public class GetLocationHandler : IRequestHandler<GetLocationRequest, Location>
    {
        private readonly IDatabase locationsDb;

        public GetLocationHandler(IDatabase locationsDb)
        {
            this.locationsDb = locationsDb;
        } 
        
        public async Task<Location> Handle(GetLocationRequest request, CancellationToken cancellationToken)
        {
            var location = await this.locationsDb.Get(request.LocationId);
            
            return location;
        }
    }
}