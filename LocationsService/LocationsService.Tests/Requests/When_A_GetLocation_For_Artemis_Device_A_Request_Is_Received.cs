// ReSharper disable InconsistentNaming
namespace Com.Apdcomms.LocationsService.Tests.Requests
{
    using Com.Apdcomms.DataGateway.LocationsService;
    using Com.Apdcomms.DataGateway.LocationsService.WebApi;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class When_A_GetLocation_Request_Is_Received : TestBase
    {
        private readonly Location location;

        public When_A_GetLocation_Request_Is_Received()
        {
            var mediator = this.ServiceProvider.GetRequiredService<IMediator>();

            A.CallTo(() => this.TestBootstrapper.FakeDatabase.Get("A"))
                .Returns(TestData.LocationDeviceA);

            this.location = mediator
                .Send(new GetLocationRequest { LocationId = "A" })
                .GetAwaiter().GetResult();
        }

        [Fact]
        public void Then_A_Location_For_Device_A_Is_Returned()
        {
            using (new AssertionScope())
            {
                this.location.Should().NotBeNull();
                this.location.Source.Should().Be("TPI");
                this.location.SourceId.Should().Be("DeviceA");
                this.location.LteMetaData.Should().BeNull();
            }
        }
    }
}