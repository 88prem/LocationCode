namespace Com.Apdcomms.StormPipeline.Storm
{
    using MediatR;
    using Polly;
    using Serilog;
    using SimpleTcp;
    using System;
    using System.Text;

    public class StormTcpInterface
    {
        private readonly ILogger logger;
        private readonly IMediator mediator;
        private readonly SimpleTcpClient tcpClient;
        private readonly Policy connectPolicy;

        public StormTcpInterface(string ipPort, ILogger logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;

            tcpClient = new SimpleTcpClient(ipPort);
            connectPolicy = Policy.Handle<Exception>().RetryForever(HandleException);

            tcpClient.Events.Connected += HandleConnected;
            tcpClient.Events.Disconnected += HandleDisconnected;
            tcpClient.Events.DataReceived += HandleDataReceived;
        }

        private void HandleException(Exception exception)
        {
            logger.Error(exception, "Failed to connect to Storm Server");
        }

        public void Connect()
        {
            connectPolicy.Execute(() =>
            {
                tcpClient.Connect();
            });
        }

        public void Disconnect()
        {
            tcpClient.Disconnect();
        }

        private async void HandleDataReceived(object sender, DataReceivedEventArgs e)
        {
            var data = Encoding.UTF8.GetString(e.Data);
            logger.Debug("Received message from Storm Server: {@Data}", data);

            var messages = data.Split("\r\n", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            logger.Debug("Split into {MessageCount} messages.", messages.Length);

            foreach (var message in messages)
            {
                await mediator.Publish(new StormMessageNotification
                {
                    Message = message
                });
            }            
        }

        private void HandleDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            logger.Warning("Disconnected from Storm Server");

            logger.Information("Attempting to reconnect to Storm Server");
            Connect();
        }

        private void HandleConnected(object sender, ClientConnectedEventArgs e)
        {
            logger.Information("Successfully connected to the Storm Server");
        }
    }
}