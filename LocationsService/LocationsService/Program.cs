namespace Com.Apdcomms.DataGateway.LocationsService
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Hosting;
    using Serilog;

    public class Program
    {
        public static int Main(string[] args)
        {
            var asmLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(Path.Join(asmLocation, "log.txt"))
                .CreateLogger();
            
            try
            {
                Log.Logger.Information("Starting host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Log.Logger.Fatal(e, "Fatal shutdown");
                return 1;
            }

            Log.Logger.Information("Graceful shutdown");
            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .UseSerilog();
    }
}