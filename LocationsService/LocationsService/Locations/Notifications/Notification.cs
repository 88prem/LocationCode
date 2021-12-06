namespace Com.Apdcomms.DataGateway.LocationsService.Notifications
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Com.Apdcomms.DataGateway.LocationsService.Lte;
    using Com.Apdcomms.DataGateway.LocationsService.Tpi;
    using MediatR;

    [Serializable]
    public record Notification : INotification
    {
        [Required] public string Source { get; init; }
        
        [Required] public string SourceId { get; init; }
        
        public TpiLocationMetaData TpiMetaData { get; init; }

        public LteLocationMetaData LteMetaData { get; init; }
    }
}