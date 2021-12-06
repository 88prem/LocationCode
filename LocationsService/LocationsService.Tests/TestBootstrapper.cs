namespace Com.Apdcomms.LocationsService.Tests
{
    using Com.Apdcomms.DataGateway.LocationsService;
    using Com.Apdcomms.DataGateway.LocationsService.Database;
    using Com.Apdcomms.DataGateway.LocationsService.Extensions;
    using Com.Apdcomms.DataGateway.LocationsService.Queue;
    using FakeItEasy;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using System.Globalization;

    public class TestBootstrapper
    {
        public IDatabase FakeDatabase { get; } = A.Fake<IDatabase>();

        public IQueue FakeQueue { get; } = A.Fake<IQueue>();

        public readonly ServiceConfiguration ServiceConfiguration = new()
        {
            EnqueueLocations = true, 
            StoreLocations = true
        };

        public void Bind(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLocationsService();

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            serviceCollection.AddSingleton(_ => FakeQueue);
            serviceCollection.AddSingleton(_ => FakeDatabase);
			serviceCollection.AddSingleton(Options.Create(this.ServiceConfiguration));
        }
    }
}