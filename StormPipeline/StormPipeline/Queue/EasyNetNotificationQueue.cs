namespace Com.Apdcomms.StormPipeline.Queue
{
    using DataGateway.QueueTypes.Pipeline.Resources;
    using Com.Apdcomms.StormPipeline.Parsing;
    using DataGateway.QueueTypes.Pipeline.Incidents;
    using EasyNetQ;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    public class EasyNetNotificationQueue : INotificationQueue
    {
        private readonly IBus bus;
        private readonly StormMessageParser stormMessageParser;

        public EasyNetNotificationQueue(
            IBus bus,
            StormMessageParser stormMessageParser)
        {
            this.bus = bus;
            this.stormMessageParser = stormMessageParser;
        }

        public async Task Enqueue(Storm stormData)
        {

            switch (stormData.StormMetaData.MessageCode)
            {
                case "CI":
                    await bus.PubSub.PublishAsync(AssignSourceFields<CreateIncident>
                (stormData, StormMessageHelper.JsonParserConvert<CreateIncident>(stormData.StormMetaData)), nameof(CreateIncident));
                    break;
                case "DI":
                    await bus.PubSub.PublishAsync(AssignSourceFields<DeleteIncident>
                (stormData, StormMessageHelper.JsonParserConvert<DeleteIncident>(stormData.StormMetaData)), nameof(DeleteIncident));
                    break;
                case "UI":
                    await bus.PubSub.PublishAsync(AssignSourceFields<UpdateIncident>
                (stormData, StormMessageHelper.JsonParserConvert<UpdateIncident>(stormData.StormMetaData)), nameof(UpdateIncident));
                    break;
                case "PRI":
                    await bus.PubSub.PublishAsync(AssignSourceFields<ProcessResourceInformation>
                (stormData, StormMessageHelper.JsonParserConvert<ProcessResourceInformation>(stormData.StormMetaData)), nameof(ProcessResourceInformation));
                    break;
                case "CI2":
                    await bus.PubSub.PublishAsync(AssignSourceFields<CreateIncidentTwo>
                (stormData, StormMessageHelper.JsonParserConvert<CreateIncidentTwo>(stormData.StormMetaData)), nameof(CreateIncidentTwo));
                    break;
                case "UR":
                    await bus.PubSub.PublishAsync(AssignSourceFields<UpdateResource>
                (stormData, StormMessageHelper.JsonParserConvert<UpdateResource>(stormData.StormMetaData)), nameof(UpdateResource));
                    break;
                case "CR":
                    await bus.PubSub.PublishAsync(AssignSourceFields<CreateResource>
                (stormData, StormMessageHelper.JsonParserConvert<CreateResource>(stormData.StormMetaData)), nameof(CreateResource));
                    break;
                case "CLI":
                    await bus.PubSub.PublishAsync(AssignSourceFields<CloseIncident>
                (stormData, StormMessageHelper.JsonParserConvert<CloseIncident>(stormData.StormMetaData)), nameof(CloseIncident));
                    break;
                case "CAI":
                    await bus.PubSub.PublishAsync(AssignSourceFields<CreateAssociatedIcon>
                (stormData, StormMessageHelper.JsonParserConvert<CreateAssociatedIcon>(stormData.StormMetaData)), nameof(CreateAssociatedIcon));
                    break;
                case "DAI":
                    await bus.PubSub.PublishAsync(AssignSourceFields<DeleteAssociatedIcon>
                (stormData, StormMessageHelper.JsonParserConvert<DeleteAssociatedIcon>(stormData.StormMetaData)), nameof(DeleteAssociatedIcon));
                    break;
                case "DIL":
                    await bus.PubSub.PublishAsync(AssignSourceFields<DeleteIncidentLog>
                (stormData, StormMessageHelper.JsonParserConvert<DeleteIncidentLog>(stormData.StormMetaData)), nameof(DeleteIncidentLog));
                    break;
                case "PII":
                    await bus.PubSub.PublishAsync(AssignSourceFields<ProcessIncidentInformation>
                (stormData, StormMessageHelper.JsonParserConvert<ProcessIncidentInformation>(stormData.StormMetaData)), nameof(ProcessIncidentInformation));
                    break;
                case "PILI":
                    await bus.PubSub.PublishAsync(AssignSourceFields<ProcessIncidentLogInformation>
                (stormData, StormMessageHelper.JsonParserConvert<ProcessIncidentLogInformation>(stormData.StormMetaData)), nameof(ProcessIncidentLogInformation));
                    break;
                case "RILA":
                    await bus.PubSub.PublishAsync(AssignSourceFields<RequestIncidentLogAddition>
                (stormData, StormMessageHelper.JsonParserConvert<RequestIncidentLogAddition>(stormData.StormMetaData)), nameof(RequestIncidentLogAddition));
                    break;
                case "ZAI":
                    await bus.PubSub.PublishAsync(AssignSourceFields<ZoomAssociatedIcon>
                (stormData, StormMessageHelper.JsonParserConvert<ZoomAssociatedIcon>(stormData.StormMetaData)), nameof(ZoomAssociatedIcon));
                    break;
                case "CR2":
                    await bus.PubSub.PublishAsync(AssignSourceFields<CreateResourceTwo>
                (stormData, StormMessageHelper.JsonParserConvert<CreateResourceTwo>(stormData.StormMetaData)), nameof(CreateResourceTwo));
                    break;
                case "DR":
                    await bus.PubSub.PublishAsync(AssignSourceFields<DeleteResource>
                (stormData, StormMessageHelper.JsonParserConvert<DeleteResource>(stormData.StormMetaData)), nameof(DeleteResource));
                    break;
                case "RRA":
                    await bus.PubSub.PublishAsync(AssignSourceFields<RequestResourceAssociation>
                (stormData, StormMessageHelper.JsonParserConvert<RequestResourceAssociation>(stormData.StormMetaData)), nameof(RequestResourceAssociation));
                    break;
                case "RRS":
                    await bus.PubSub.PublishAsync(AssignSourceFields<RequestResourceStatus>
                (stormData, StormMessageHelper.JsonParserConvert<RequestResourceStatus>(stormData.StormMetaData)), nameof(RequestResourceStatus));
                    break;
                case "RSC":
                    await bus.PubSub.PublishAsync(AssignSourceFields<ResourceShiftChange>
                (stormData, StormMessageHelper.JsonParserConvert<ResourceShiftChange>(stormData.StormMetaData)), nameof(ResourceShiftChange));
                    break;
                case "AL":
                    await bus.PubSub.PublishAsync(AssignSourceFields<VehicleAlert>
                (stormData, StormMessageHelper.JsonParserConvert<VehicleAlert>(stormData.StormMetaData)), nameof(VehicleAlert));
                    break;
                default:
                    break;
            }
        }

        private static T AssignSourceFields<T>(Storm stormData, object message)
        {
            var incidentMessage = (T)message;

            Type type = incidentMessage.GetType();
            PropertyInfo sourceName = type.GetProperty("SourceName");
            sourceName.SetValue(incidentMessage, stormData.SourceName, null);

            PropertyInfo sourceId = type.GetProperty("SourceId");
            sourceId.SetValue(incidentMessage, stormData.SourceId, null);
            return incidentMessage;
        }
    }
}