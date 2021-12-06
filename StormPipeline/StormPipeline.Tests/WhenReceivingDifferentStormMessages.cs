namespace Com.Apdcomms.StormPipeline.Tests
{
    using Com.Apdcomms.StormPipeline.Parsing.Factory;
    using Com.Apdcomms.StormPipeline.Parsing.Messages;
    using Com.Apdcomms.StormPipeline.Queue;    
    using Com.Apdcomms.StormPipeline.Storm;    
    using FakeItEasy;
    using FluentAssertions;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;    
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class WhenReceivingDifferentStormMessages : TestBase
    {
        private Storm incidentPushedToPipelineQueue;

        public IStormMessageFactory<IOperate> FakeFactory { get; } = A.Fake<IStormMessageFactory<IOperate>>();

        private static ILogger logger { get; }

        private static StormMessageValidator stormMessageValidator { get; }

        public WhenReceivingDifferentStormMessages()
        {
            A.CallTo(() => TestBootstrapper.FakeNotificationQueue.Enqueue(A<Storm>._))
                .Invokes((Storm m) => incidentPushedToPipelineQueue = m);
        }

        protected virtual StormMessageNotification StormMessageNotification { get; } = new StormMessageNotification
        {
            Message = "0|||PRI|AJAX1|C|1|-0.365077|53.773017||||000|198|12:50:48 22/07/02|1"
        };

        protected virtual IOperate Operate { get; set; }

        private Task PublishNotification() => PublishNotification(StormMessageNotification);

        private async Task PublishNotification(StormMessageNotification notification)
        {
            var mediator = ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(notification);
        }

        public abstract class AndTheMessageIsValid : 
            WhenReceivingDifferentStormMessages
        {
            public class BecauseTheMessageHasValidUniqueField_LessThan40Characters : 
                AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CI2|Thisisamessagewithlengthlessthan40chara|C|-0.365077|53.773017|B_Grade|ResoureX|22/07/02 12:50:48|High|1|PursuitA|ControlArea1"
                };

                protected override IOperate Operate { get; set; } = new CreateIncidentTwo(logger, stormMessageValidator);
            }

            public class ForPRIMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||PRI|AJAX1|C|1|-0.365077|53.773017||||000|198|12:50:48 22/07/02|1"
                };

                protected override IOperate Operate { get; set; } = new ProcessResourceInformation(logger, stormMessageValidator);
            }

            public class ForCI2Message : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CI2|AJAX1_CI2|C|-0.365077|53.773017|B_Grade|ResoureX|22/07/02 12:50:48|High|1|PursuitA|ControlArea1"
                };

                protected override IOperate Operate { get; set; } = new CreateIncidentTwo(logger, stormMessageValidator);
            }

            public class ForDIMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||DI|AJAX1"
                };

                protected override IOperate Operate { get; set; } = new DeleteIncident(logger, stormMessageValidator);
            }

            public class ForSDAMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||SDA|Ambulance|Color|Red"
                };

                protected override IOperate Operate { get; set; } = new SetDataAttribute(logger, stormMessageValidator);
            }

            public class ForUIMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||UI|AJAX1|C|1|-0.365077|53.773017"
                };

                protected override IOperate Operate { get; set; } = new UpdateIncident(logger, stormMessageValidator);
            }

            public class ForURMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||UR|AJAX1|C|1|-0.365077|53.773017||||000|198|12:50:48 22/07/02|1"
                };

                protected override IOperate Operate { get; set; } = new UpdateResource(logger, stormMessageValidator);
            }

            public class ForPSIMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||PSI|AJAX1_PSI|C|-0.365077|53.773017|-0.377899|54.552416|B_Grade|ResoureX|22/07/02 12:50:48|High|1|PursuitA|ControlArea1"
                };

                protected override IOperate Operate { get; set; } = new ProcessStationInformation(logger, stormMessageValidator);
            }

            public class ForCIMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CI|AJAX1|C|1|-0.365077|53.773017"
                };

                protected override IOperate Operate { get; set; } = new CreateIncident(logger, stormMessageValidator);
            }

            public class ForCRMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CR|AJAX1|C|1|-0.365077|53.773017||||000|198|12:50:48 22/07/02|1"
                };

                protected override IOperate Operate { get; set; } = new CreateResource(logger, stormMessageValidator);
            }

            public class ForDCOMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||DCO|AJAX1|CLASS1|-0.365077|53.773017"
                };

                protected override IOperate Operate { get; set; } = new CreateDynamicObject(logger, stormMessageValidator);
            }

            public class ForPIIMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||PII|AJAX1|C|1|-0.365077|53.773017||||000|198|22/07/02 12:50:48|1"
                };

                protected override IOperate Operate { get; set; } = new ProcessIncidentInformation(logger, stormMessageValidator);
            }

            public class ForRSCMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||RSC|Id643278|2021:02:28:23:56:53"
                };

                protected override IOperate Operate { get; set; } = new ResourceShiftChange(logger, stormMessageValidator);
            }

            public class ForPILIMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||PILI|IncidentID|LogEntryID|SegmentID|OrderID|Username|2021:02:28:23:56:53|Text"
                };

                protected override IOperate Operate { get; set; } = new ProcessIncidentLogInformation(logger, stormMessageValidator);
            }

            public class ForRILAMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||RILA|Id643278|TomDik|2021:02:28:23:56:53"
                };

                protected override IOperate Operate { get; set; } = new RequestIncidentLogAddition(logger, stormMessageValidator);
            }

            public class ForRRAMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||RRA|LABEL|OBJ[|LABEL|OBJ]"
                };

                protected override IOperate Operate { get; set; } = new RequestResourceAssociation(logger, stormMessageValidator);

                [Fact]
                public async Task ThenTheObjectFieldShouldMatch()
                {
                    await PublishNotification();
                    var messageCode = StormMessageNotification.Message.Split('|')[StormConstants.Indices.MessageCode];
                    ((Queue.MessageCodes.RequestResourceAssociation)(incidentPushedToPipelineQueue.StormMetaData)).Obj.Should().Be("OBJ[|LABEL|OBJ]");
                }
            }
            
            public class ForCR2Message : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CR2|AJAX|FIRST|1|0.245894|51.287129||||000|198|12:50:48 03/03/21|1"
                };

                protected override IOperate Operate { get; set; } = new CreateResourceTwo(logger, stormMessageValidator);
            }

            public class ForDDOMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||DDO|ID123"
                };

                protected override IOperate Operate { get; set; } = new DeleteDynamicObject(logger, stormMessageValidator);
            }

            public class ForRFSMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||RFS|Name6878"
                };

                protected override IOperate Operate { get; set; } = new RemoveFireStation(logger, stormMessageValidator);
            }

            public class ForDILMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||DIL|ID638|IDentry34"
                };

                protected override IOperate Operate { get; set; } = new DeleteIncidentLog(logger, stormMessageValidator);
            }

            public class ForDRMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||DR|AJAX1"
                };

                protected override IOperate Operate { get; set; } = new DeleteResource(logger, stormMessageValidator);
            }

            public class ForRRSMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||RRS|AJAX1|1"
                };

                protected override IOperate Operate { get; set; } = new RequestResourceStatus(logger, stormMessageValidator);
            }

            public class ForALMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||AL|AJAX1"
                };

                protected override IOperate Operate { get; set; } = new VehicleAlert(logger, stormMessageValidator);
            }

            public class ForCLIMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CLI|INC_ID|INC_CODE|-0.365077|53.773017|2021:02:28:23:56:53"
                };

                protected override IOperate Operate { get; set; } = new CloseIncident(logger, stormMessageValidator);
            }

            public class ForFWAMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||FWA|AJAX1|xyz|ATTRIBUTE|10|11"
                };

                protected override IOperate Operate { get; set; } = new SendWatchAlert(logger, stormMessageValidator);
            }

            public class ForCSUMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CSU|STATUS|UNIT|RATE"
                };

                protected override IOperate Operate { get; set; } = new ChangeStatusUpdate(logger, stormMessageValidator);
            }

            public class ForCAIMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CAI|AJAX|C|iconID|35|78|w"
                };

                protected override IOperate Operate { get; set; } = new CreateAssociatedIcon(logger, stormMessageValidator);
            }

            public class ForZAIMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||ZAI|AJAX|D"
                };

                protected override IOperate Operate { get; set; } = new ZoomAssociatedIcon(logger, stormMessageValidator);
            }

            public class ForDAIMessage : AndTheMessageIsValid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||DAI|AJAX|E"
                };

                protected override IOperate Operate { get; set; } = new DeleteAssociatedIcon(logger, stormMessageValidator);
            }

            [Fact]
            public async Task ThenTheMessageCodeShouldMatch()
            {
                await PublishNotification();
                var messageCode = StormMessageNotification.Message.Split('|')[StormConstants.Indices.MessageCode];
                incidentPushedToPipelineQueue.StormMetaData.MessageCode.Should().Be(messageCode, "The message code should match");
            }

            [Fact]
            public async Task ThenTheMessageClassObjectIsCreatedFromFactory()
            {
                await PublishNotification();
                A.CallTo(() => FakeFactory.Create(A<string>._)).Returns(Operate);
                Operate.Should().BeAssignableTo<IOperate>("The message object is inherited from IOperate");
            }

            [Fact]
            public async Task ThenAProcessedMessageIsLoggedInOperateMethod()
            {
                await PublishNotification();
                A.CallTo(TestBootstrapper.FakeLogger).Where(x => x.Method.Name == nameof(ILogger.Information)
                    && x.Arguments.Get<string>(0).Contains("processed"))
                    .MustHaveHappened();
            }

            [Fact]
            public async Task ThenAInfoMessageShouldBeLogged()
            {
                await PublishNotification();
                A.CallTo(TestBootstrapper.FakeLogger).Where(x => x.Method.Name == nameof(ILogger.Information)
                && x.Arguments.Get<string>(0).Contains("Successfully handled Storm message"))
                    .MustHaveHappened();
            }
        }

        public abstract class AndTheMessageIsInvalid : WhenReceivingDifferentStormMessages
        {
            public class BecauseTheMessageIsNotSupported : AndTheMessageIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||XYZ|AJAX1"
                };

                [Fact]
                public async Task ThenANotSupportedExceptionIsThrown()
                {
                    await PublishNotification();
                    A.CallTo(TestBootstrapper.FakeLogger).Where(x => x.Method.Name == nameof(ILogger.Warning))
                    .MustHaveHappened();
                }
            }

            public class BecauseTheMessageContainsMoreFieldsThanMapping : AndTheMessageIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||DI|AJAX1|23.65|Control1"
                };

                [Fact]
                public async Task ThenAStormMessageTooManyFieldsExceptionIsThrown()
                {
                    await PublishNotification();
                    A.CallTo(TestBootstrapper.FakeLogger).Where(x => x.Method.Name == nameof(ILogger.Warning))
                    .MustHaveHappened();
                }
            }

            public class BecauseTheMessageContainsInvalidDateFormat1 : AndTheMessageIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CR|L00|I|0|0.245894|51.287129||||000|198|12:50:48 03/03|1|"
                };

                [Fact]
                public async Task ThenAStormMessageInvalidDateTimeFieldExceptionIsThrown()
                {
                    await PublishNotification();
                    A.CallTo(TestBootstrapper.FakeLogger).Where(x => x.Method.Name == nameof(ILogger.Warning)
                    && x.Arguments.Get<string>(1).Contains("Invalid message received from Storm"))
                    .MustHaveHappened();
                }
            }

            public class BecauseTheMessageContainsInvalidDateFormat2 : AndTheMessageIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||RSC|Id643278|2021:02:28:56:53"
                };

                [Fact]
                public async Task ThenAStormMessageInvalidDateTimeFieldExceptionIsThrown()
                {
                    await PublishNotification();
                    A.CallTo(TestBootstrapper.FakeLogger).Where(x => x.Method.Name == nameof(ILogger.Warning)
                    && x.Arguments.Get<string>(1).Contains("Invalid message received from Storm"))
                    .MustHaveHappened();
                }
            }
        }
    }
}
