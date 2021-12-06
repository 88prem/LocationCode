// ReSharper disable InconsistentNaming
namespace Com.Apdcomms.LocationsService.Tests.Commands
{
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService;
    using Com.Apdcomms.DataGateway.LocationsService.Refresh;
    using FakeItEasy;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class When_A_Refresh_Command_Is_Received_From_TPI : TestBase
    {
        private LocationsRefreshRequest publishedMsg;
        private readonly IMediator mediator;

        public When_A_Refresh_Command_Is_Received_From_TPI()
        {
            this.mediator = this.ServiceProvider.GetRequiredService<IMediator>();
            
            A.CallTo(() => this.TestBootstrapper.FakeQueue.EnqueueLocationsRefreshCommand(A<LocationsRefreshRequest>._))
                .Invokes((LocationsRefreshRequest msg) => publishedMsg = msg);
        }

        public class And_Has_A_Valid_Target_Interface : When_A_Refresh_Command_Is_Received_From_TPI
        {
            public And_Has_A_Valid_Target_Interface()
            {
                var request = new LocationsRefreshRequest
                {
                    TargetInterface = "TPI"
                };

                mediator.Send(request);
            }
            
            [Fact]
            public void Then_Is_Published_To_Pipeline_Queue()
            {
                using (new AssertionScope())
                {
                    this.publishedMsg.Should().NotBeNull();
                    this.publishedMsg.TargetInterface.Should().Be("TPI");
                }
            }
        }
        
        public class And_Has_A_Invalid_Target_Interface : When_A_Refresh_Command_Is_Received_From_TPI
        {
            private LocationsRefreshRequest request;

            public And_Has_A_Invalid_Target_Interface()
            {
                this.request = new LocationsRefreshRequest
                {
                    TargetInterface = "XXX"
                };
            }
            
            [Fact]
            public async Task Then_An_InvalidTargetInterfaceException_Is_Thrown()
            {
                await FluentActions.Invoking(() => this.mediator.Send(request))
                    .Should().ThrowAsync<InvalidTargetInterfaceException>();
            }
        }
    }
}