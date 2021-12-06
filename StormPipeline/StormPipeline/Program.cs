namespace Com.Apdcomms.StormPipeline
{
    using Com.Apdcomms.StormPipeline.Extensions;
    using Com.Apdcomms.StormPipeline.Storm;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using System;
    using System.IO;
    using System.Reflection;

    internal class Program
    {
        private static void Main()
        {
            try
            {
                Initialize();
            }
            catch (Exception e)
            {
                Log.Logger.Fatal(e, "Fatal exception occurred during initialization");
                return;
            }

            Console.WriteLine("Waiting for 'exit'");
            string input;
            do
            {
                input = Console.ReadLine();
            } while (input?.ToLower() != "exit");
        }

        private static void Initialize()
        {
            var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(Path.Join(assemblyLocation, "log.txt"))
                .CreateLogger();

            Log.Logger.Information("Storm Pipeline initializing");

            var environment = Environment.GetEnvironmentVariable("DATAGATEWAY_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{environment}.json", true)
                .AddJsonFile("mapping.json", false, true)
                .Build();

            Log.Logger.Debug("CONFIG: {@Config}", configuration.GetDebugView());

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(Log.Logger);
            serviceCollection.AddStormPipeline();
            serviceCollection.AddStormPipelineDal(configuration);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var tcpInterface = serviceProvider.GetRequiredService<StormTcpInterface>();
            tcpInterface.Connect();

            Log.Logger.Information("Storm Pipeline initialized successfully");
        }
    }
}