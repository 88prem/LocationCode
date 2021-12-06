namespace Com.Apdcomms.StormPipeline.Tests
{
    using Microsoft.Extensions.DependencyInjection;
    using System;

    public abstract class TestBase
    {
        public TestBase()
        {
            var serviceCollection = new ServiceCollection();
            TestBootstrapper.Bind(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        protected IServiceProvider ServiceProvider { get; }

        protected TestBootstrapper TestBootstrapper { get; } = new();
    }
}
