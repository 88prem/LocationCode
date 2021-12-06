namespace Com.Apdcomms.DataGateway.LocationsService.Queue
{
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService.Refresh;
    using Com.Apdcomms.DataGateway.LocationsService.ReportPosition;

    public interface IQueue
    {
        Task EnqueueLocation(Location location);

        Task EnqueueTpiPositionOnRequestCommand(TpiPositionOnRequestCommand tpiPositionOnRequestCommand);

        Task EnqueueLocationsRefreshCommand(LocationsRefreshRequest locationsRefreshRequest);
    }
}