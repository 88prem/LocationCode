namespace Com.Apdcomms.LocationsService.Tests
{
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService;
    using Com.Apdcomms.DataGateway.LocationsService.ReportPosition;
    using FakeItEasy;
    using FluentAssertions;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public abstract class WhenReceivingAReportPositionRequest : TestBase
    {
        protected abstract ReportPositionRequest Request { get; }
        
        private Task SendRequest() => ServiceProvider.GetRequiredService<IMediator>().Send(Request);

        public abstract class AndTheTargetInterfaceIsInvalid : WhenReceivingAReportPositionRequest
        {
            public class BecauseTheTargetInterfaceIsNull : AndTheTargetInterfaceIsInvalid
            {
                protected override ReportPositionRequest Request { get; } = new()
                {
                    SourceId = "PatrolCar1",
                    TargetInterface = null
                };                
            }

            public class BecauseTheTargetInterfaceIsWhiteSpace : AndTheTargetInterfaceIsInvalid
            {
                protected override ReportPositionRequest Request { get; } = new()
                {
                    SourceId = "PatrolCar1",
                    TargetInterface = "            "
                };
            }
            
            public class BecauseTheTargetInterfaceIsUnknown : AndTheTargetInterfaceIsInvalid
            {
                protected override ReportPositionRequest Request { get; } = new()
                {
                    SourceId = "PatrolCar1",
                    TargetInterface = "X45"
                };
            }

            [Fact]
            public async Task ThenAnInvalidTargetInterfaceExceptionIsThrown()
            {
                await FluentActions.Invoking(() => SendRequest())
                    .Should().ThrowAsync<InvalidTargetInterfaceException>();
            }
        }
        
        
        
        public class AndTheTargetInterfaceIsTpi : WhenReceivingAReportPositionRequest
        {
            protected override ReportPositionRequest Request { get; } = new()
            {
                SourceId = "PatrolCar1",
                TargetInterface = "TPI"
            };

            private TpiPositionOnRequestCommand enqueuedOnRequestCommand;

            public AndTheTargetInterfaceIsTpi()
            {
                A.CallTo(() =>
                        TestBootstrapper.FakeQueue.EnqueueTpiPositionOnRequestCommand(A<TpiPositionOnRequestCommand>._))
                    .Invokes((TpiPositionOnRequestCommand reportPositionCommand) =>
                        enqueuedOnRequestCommand = reportPositionCommand);
            }


            [Fact]
            public async Task ThenAReportPositionCommandShouldBePushedToThePipelineExchange()
            {
                await SendRequest();

                A.CallTo(() =>
                        TestBootstrapper.FakeQueue.EnqueueTpiPositionOnRequestCommand(A<TpiPositionOnRequestCommand>._))
                    .MustHaveHappened();
            }

            [Fact]
            public async Task ThenTheResourceIdShouldMatch()
            {
                await SendRequest();

                enqueuedOnRequestCommand.ResourceId.Should().Be(Request.SourceId);
            }
        }
    }
}