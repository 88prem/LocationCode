namespace Com.Apdcomms.DataGateway.LocationsService.ReportPosition
{
    using System.Threading;
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService.Queue;
    using MediatR;

    public class ReportPositionRequestHandler : IRequestHandler<ReportPositionRequest>
    {
        private readonly IQueue queue;

        public ReportPositionRequestHandler(
            IQueue queue)
        {
            this.queue = queue;
        }

        public async Task<Unit> Handle(
            ReportPositionRequest request,
            CancellationToken cancellationToken)
        {
            switch (request.TargetInterface)
            {
                case "TPI":
                    await queue.EnqueueTpiPositionOnRequestCommand(new TpiPositionOnRequestCommand
                    {
                        ResourceId = request.SourceId
                    });
                    break;
                default: throw new InvalidTargetInterfaceException(request.TargetInterface);
            }
            
            return Unit.Value;
        }
    }
}