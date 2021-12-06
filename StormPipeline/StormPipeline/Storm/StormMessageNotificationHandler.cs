namespace Com.Apdcomms.StormPipeline.Storm
{
    using Com.Apdcomms.StormPipeline.Parsing;    
    using Com.Apdcomms.StormPipeline.Queue;    
    using Com.Apdcomms.StormPipeline.Storm.Exceptions;
    using MediatR;
    using Serilog;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class StormMessageNotificationHandler : INotificationHandler<StormMessageNotification>
    {
        private readonly ILogger logger;
        private readonly StormMessageValidator stormMessageValidator;
        private readonly INotificationQueue notificationQueue;
        private readonly StormMessageMapper stormMessageMapper;        

        public StormMessageNotificationHandler(ILogger logger, StormMessageValidator stormMessageValidator,
            INotificationQueue notificationQueue, StormMessageMapper stormMessageMapper)
        {
            this.logger = logger;
            this.stormMessageValidator = stormMessageValidator;
            this.notificationQueue = notificationQueue;
            this.stormMessageMapper = stormMessageMapper;            
        }

        public async Task Handle(StormMessageNotification notification, CancellationToken cancellationToken)
        {
            logger.Information("Received new Storm message");
            logger.Debug("Notification={@Notification}", notification);

            try
            {
                stormMessageValidator.Validate(notification.Message);
                var (message, messageCode, messageClass) = stormMessageMapper.GetMessageData(notification.Message);
                var stormMetaData = stormMessageMapper.Map(message, messageCode);
                stormMetaData = stormMessageMapper.OperateStormMessage(messageClass, stormMetaData, message, messageCode);
                await notificationQueue.Enqueue(new Storm
                {
                    SourceName = "Storm",
                    SourceId = StormMessageHelper.GetUniqueIdentifier(notification.Message),
                    StormMetaData = stormMetaData
                });
                logger.Information("Successfully handled Storm message");
            }
            catch (StormMessageException ex)
            {
                logger.Warning(ex, "Invalid message received from Storm");
                return;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, ex.Message);
                return;
            }
        }
    }
}