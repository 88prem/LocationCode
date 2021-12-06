namespace Com.Apdcomms.StormPipeline.Storm
{
    using MediatR;

    public record StormMessageNotification : INotification
    {
        public string Message { get; init; }
    }
}