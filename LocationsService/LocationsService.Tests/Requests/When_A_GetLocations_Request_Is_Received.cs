// ReSharper disable InconsistentNaming
namespace Com.Apdcomms.LocationsService.Tests.Requests
{
    using System.Collections.Generic;
    using Com.Apdcomms.DataGateway.LocationsService;
    using Com.Apdcomms.DataGateway.LocationsService.WebApi;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class When_A_GetLocations_Request_Is_Received : TestBase
    {
        private readonly IEnumerable<Location> locations;
        private readonly IMediator mediator;

        public When_A_GetLocations_Request_Is_Received()
        {
            this.mediator = this.ServiceProvider.GetRequiredService<IMediator>();

            A.CallTo(() => this.TestBootstrapper.FakeDatabase.GetAll())
                .Returns(TestData.DataWith5Locations);

            var request = new GetLocationsRequest();

            this.locations = this.mediator.Send(request).GetAwaiter().GetResult();
        }

        [Fact]
        public void Then_A_List_Of_Locations_Is_Returned()
        {
            using (new AssertionScope())
            {
                this.locations.Should().HaveCount(5);
            }
        }
    }
}