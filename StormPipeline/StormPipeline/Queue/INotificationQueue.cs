namespace Com.Apdcomms.StormPipeline.Queue
{
    using System.Threading.Tasks;

    public interface INotificationQueue
    {
        Task Enqueue(Storm incident);
    }
}
