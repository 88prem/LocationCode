namespace Com.Apdcomms.DataGateway.LocationsService.Refresh
{
    using System.Threading;
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService.Queue;
    using MediatR;

    public class LocationsRefreshRequestHandler : AsyncRequestHandler<LocationsRefreshRequest>
    {
        private readonly IQueue queue;

        public LocationsRefreshRequestHandler(IQueue queue)
        {
            this.queue = queue;
        }    
        
        protected override async Task Handle(LocationsRefreshRequest request, CancellationToken cancellationToken) 
        {
            switch (request.TargetInterface)
            {
                case "TPI":
                    await queue.EnqueueLocationsRefreshCommand(request);
                    break;
                default: throw new InvalidTargetInterfaceException(request.TargetInterface);
            }
        }
    }
}