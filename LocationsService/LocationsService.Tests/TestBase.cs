namespace Com.Apdcomms.LocationsService.Tests
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    public abstract class TestBase
    {
        protected IServiceProvider ServiceProvider { get; }

        protected TestBootstrapper TestBootstrapper { get; } = new();

        public TestBase()
        {
            var serviceCollection = new ServiceCollection();
            TestBootstrapper.Bind(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}