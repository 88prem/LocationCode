namespace Com.Apdcomms.StormPipeline.Tests
{
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

    public class WhenReceivingANewStormMessageNotification : TestBase
    {
        private Storm incidentPushedToPipelineQueue;

        public WhenReceivingANewStormMessageNotification()
        {
            A.CallTo(() => TestBootstrapper.FakeNotificationQueue.Enqueue(A<Storm>._))
                .Invokes((Storm m) => incidentPushedToPipelineQueue = m);
        }

        protected virtual StormMessageNotification StormMessageNotification { get; } = new StormMessageNotification
        {
            Message = "0|||PRI|AJAX1|C|1|-0.365077|53.773017||||000|198|12:50:48 22/07/02|1"
        };

        private Task PublishNotification() => PublishNotification(StormMessageNotification);

        private async Task PublishNotification(StormMessageNotification notification)
        {
            var mediator = ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(notification);
        }

        public class AndTheNotificationIsValid : 
            WhenReceivingANewStormMessageNotification
        {

            [Fact]
            public async Task ThenAStormModelShouldHaveBeenEnqueued()
            {
                await PublishNotification();
                incidentPushedToPipelineQueue.Should().NotBeNull();
            }

            [Fact]
            public async Task ThenTheStormModelShouldHaveSourceSetToStorm()
            {
                await PublishNotification();
                incidentPushedToPipelineQueue.SourceName.Should().Be("Storm");
            }

            [Fact]
            public async Task ThenTheIncidentShouldHaveSourceIdSetToTheUniqueIdentifierField()
            {
                await PublishNotification();

                incidentPushedToPipelineQueue.SourceId.Should().Be("AJAX1");
            }

            [Fact]
            public async Task ThenTheStormMetaDataShouldNotBeNull()
            {
                await PublishNotification();
                incidentPushedToPipelineQueue.StormMetaData.Should().NotBeNull();
            }

            [Fact]
            public async Task ThenTheStormMetaDataShouldHaveTheMessageCodeFromTheNotification()
            {
                await PublishNotification();
                incidentPushedToPipelineQueue.StormMetaData.MessageCode.Should().Be("PRI");
            }

            [Fact]
            public async Task ThenTheStormMetaDataShouldHaveTheIdFromTheNotification()
            {
                await PublishNotification();
                ((Queue.MessageCodes.ProcessResourceInformation)(incidentPushedToPipelineQueue.StormMetaData)).Id.Should().Be("AJAX1");
            }

            [Fact]
            public async Task ThenTheStormMetaDataShouldHaveTheTypeFromTheNotification()
            {
                await PublishNotification();
                ((Queue.MessageCodes.ProcessResourceInformation)(incidentPushedToPipelineQueue.StormMetaData)).Type.Should().Be("C");
            }

            [Fact]
            public async Task ThenTheStormMetaDataShouldHaveTheStatusFromTheNotification()
            {
                await PublishNotification();
                ((Queue.MessageCodes.ProcessResourceInformation)(incidentPushedToPipelineQueue.StormMetaData)).Status.Should().Be("1");
            }

            [Fact]
            public async Task ThenTheStormMetaDataShouldHaveTheEastingFromTheNotification()
            {
                await PublishNotification();
                ((Queue.MessageCodes.ProcessResourceInformation)(incidentPushedToPipelineQueue.StormMetaData)).Easting.Should().Be(-0.365077);
            }

            [Fact]
            public async Task ThenTheStormMetaDataShouldHaveTheNorthingFromTheNotification()
            {
                await PublishNotification();
                ((Queue.MessageCodes.ProcessResourceInformation)(incidentPushedToPipelineQueue.StormMetaData)).Northing.Should().Be(53.773017);
            }

            [Fact]
            public async Task ThenTheStormMetaDataShouldHaveTheSpeedFromTheNotification()
            {
                await PublishNotification();
                ((Queue.MessageCodes.ProcessResourceInformation)(incidentPushedToPipelineQueue.StormMetaData)).Speed.Should().Be(000);
            }

            [Fact]
            public async Task ThenTheStormMetaDataShouldHaveTheBearingFromTheNotification()
            {
                await PublishNotification();
                ((Queue.MessageCodes.ProcessResourceInformation)(incidentPushedToPipelineQueue.StormMetaData)).Bearing.Should().Be(198);
            }

            [Fact]
            public async Task ThenTheStormMetaDataShouldHaveTheUpdateTimeFromTheNotification()
            {
                await PublishNotification();
                ((Queue.MessageCodes.ProcessResourceInformation)(incidentPushedToPipelineQueue.StormMetaData)).UpdateTime.Should().Be("12:50:48 22/07/02");
            }

            [Fact]
            public async Task ThenTheStormMetaDataShouldHaveTheDeviceIdFromTheNotification()
            {
                await PublishNotification();
                ((Queue.MessageCodes.ProcessResourceInformation)(incidentPushedToPipelineQueue.StormMetaData)).DeviceId.Should().Be("1");
            }
        }

        public abstract class AndTheNotificationIsInvalid : 
            WhenReceivingANewStormMessageNotification
        {
            public class BecauseTheMessageDoesNotHaveTheCorrectFormat : 
                AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "FAULTYMESSAGE$12345"
                };
            }

            public class BecauseTheMessageIsNull : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = null
                };
            }

            public class BecauseTheMessageIsEmpty : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = string.Empty
                };
            }

            public class BecauseTheMessageIsWhiteSpace : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "           "
                };
            }

            public class BecauseTheMessageContainsNoMessageCode : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0||||AJAX1|C|1|-0.365077|53.773017||||000|198|22/07/02 12:50:48|1"
                };
            }

            public class BecauseTheMessageIsTooLong : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = TestBootstrapper.GenerateString()
                };
            }

            public class BecauseTheMessageIsTooShortWithoutMessageCode : 
                AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|ABC|1"
                };
            }

            //CI2 since PRI doesnt have unique field validation
            public class BecauseTheMessageHasInvalidUniqueField_SpecialCharacter : 
                AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CI2|@JAX1_CI2|C|-0.365077|53.773017|B_Grade|ResoureX|22/07/02 12:50:48|High|1|PursuitA|ControlArea1"
                };
            }

            public class BecauseTheMessageHasInvalidUniqueField_WithSpace : 
                AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CI2|A   X1_CI2|C|-0.365077|53.773017|B_Grade|ResoureX|22/07/02 12:50:48|High|1|PursuitA|ControlArea1"
                };
            }

            public class BecauseTheMessageHasInvalidUniqueField_MoreThan40Characters : 
                AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CI2|Thisisamessagewithlengthgreaterthan40char|C|-0.365077|53.773017|B_Grade|ResoureX|22/07/02 12:50:48|High|1|PursuitA|ControlArea1\r\n"
                };
            }

            public class BecauseTheMessageHasInvalidUniqueField_EqualTo40Characters : 
                AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CI2|Thisisamessagewithlengthequaltothan40chr|C|-0.365077|53.773017|B_Grade|ResoureX|22/07/02 12:50:48|High|1|PursuitA|ControlArea1"
                };
            }

            public class BecauseTheMessageAttributeIsTooLong : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CI2|AJAX1|C|-0.365077|53.773017|B_Grade|abcdefghinscnsknksnksnckscksclsclsmlmslmsabcdefghinscnsknksnksnckscksclsclsmlmslmsabcdefghinscnsknksnksnckscksclsclsmlmslmshuuukk|22/07/02 12:50:48|High|1|PursuitA|ControlArea1"
                };
            }

            public class BecauseTheRILAMessageTimeStampIsInvalid : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||RILA|Id643278|TomDik|2021:02:29:23:56:53"
                };
            }

            public class BecauseThePILIMessageTimeStampIsInvalid : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||PILI|IncidentID|LogEntryID|SegmentID|OrderID|Username|2021:02:29:23:56:53|Text"
                };
            }

            public class BecauseTheRSCMessageNextEndShiftIsInvalid : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||RSC|Id643278|2021:02:29:23:56:53"
                };
            }
            public class BecauseTheCR2MessageClassFieldIsInvalid : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CR2|#JAX1|C|1|-0.365077"
                };
            }

            public class BecauseTheCI2MessageClassFieldIsInvalid : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||CI2|#JAX1|C|1|-0.365077"
                };
            }

            public class BecauseTheURMessageLabelFieldIsInvalid : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||UR|#JAX1|C|1|-0.365077"
                };
            }

            public class BecauseTheUIMessageLabelFieldIsInvalid : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||UI|@JAX1|C|1|-0.365077"
                };
            }

            public class BecauseThePIIMessageAttributeFieldIsInvalid : 
                AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||PII|AJAX1|C|1|-0.365077|53.773017||||000|19847328923474732848992124547892347566hjkashfduier238877758923875892635692365869823ggfjhgwherghwrgggfgjhd76423764237467234872364619847328923474732848992124547892347566hjkashfduier238877758923875892635692365869823ggfjhgwherghwrgggfgjhd76423764237467234872364619847328923474732848992124547892347566hjkashfduier238877758923875892635692365869823ggfjhgwherghwrgggfgjhd764237642374672348723646|22/07/02 12:50:48|1"
                };
            }

            public class BecauseThePRIMessageAttributeFieldIsInvalid : AndTheNotificationIsInvalid
            {
                protected override StormMessageNotification StormMessageNotification { get; } = new()
                {
                    Message = "0|||PRI|AJAX1|C|1|-0.365077|53.773017||||000|19847328923474732848992124547892347566hjkashfduier238877758923875892635692365869823ggfjhgwherghwrgggfgjhd76423764237467234872364619847328923474732848992124547892347566hjkashfduier238877758923875892635692365869823ggfjhgwherghwrgggfgjhd76423764237467234872364619847328923474732848992124547892347566hjkashfduier238877758923875892635692365869823ggfjhgwherghwrgggfgjhd764237642374672348723646|12:50:48 22/07/02|1"
                };
            }

            [Fact]
            public async Task ThenAWarningShouldBeLogged()
            {
                await PublishNotification();

                A.CallTo(TestBootstrapper.FakeLogger).Where(x => x.Method.Name == nameof(ILogger.Warning))
                    .MustHaveHappened();
            }

            [Fact]
            public async Task ThenNothingShouldEnqueued()
            {
                await PublishNotification();

                incidentPushedToPipelineQueue.Should().BeNull();
            }
        }
    }
}