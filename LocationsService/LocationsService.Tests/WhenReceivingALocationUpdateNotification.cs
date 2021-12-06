namespace Com.Apdcomms.LocationsService.Tests
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService;
    using Com.Apdcomms.DataGateway.LocationsService.Lte;
    using Com.Apdcomms.DataGateway.LocationsService.Notifications;
    using Com.Apdcomms.DataGateway.LocationsService.Tpi;
    using FakeItEasy;
    using FluentAssertions;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public abstract class WhenReceivingALocationUpdateNotification : TestBase
    {
        protected virtual Notification Notification { get; } = new()
        {
            Source = "TPI",
            SourceId = "Driver1",
            TpiMetaData = new TpiLocationMetaData
            {
                Resource = "Driver1",
                Bearing = 109,
                Fix = 5,
                Inputs = 12,
                Latitude = 53.12,
                Longitude = -2.3423,
                Outputs = 5,
                Speed = 12,
                Status = 4,
                EventText = "some event text",
                Timestamp = (DateTime.Now - TimeSpan.FromMinutes(1)).ToString(CultureInfo.InvariantCulture),
                Class = "SomeClass",
                Contents = 4,
                Data = "Some Data",
                EventId = 15,
                GeoData = 3
            }
        };

        private LteLocationMetaData lteLocationMetaData { get; } = new()
        {
            GroupDisplayName = "APD 1",
            GroupID = "sip:3.97@poc1p.mcx.esn.gov.uk",
            ReportType = "Emergency",
            UserDisplayName = "John Smith 123",
            UserID = "tel:+912132432122 ",
            Longitude = -2.3423,
            Latitude = 53.12
        };

        private Location createdLocation;
        private Location updatedLocation;
        private Location enqueuedLocation;

        public WhenReceivingALocationUpdateNotification()
        {
            A.CallTo(() => TestBootstrapper.FakeDatabase.Get(A<string>._))
                .Returns((Location)null);

            A.CallTo(() => TestBootstrapper.FakeDatabase.Create(A<Location>._))
                .Invokes((Location location) => createdLocation = location);

            A.CallTo(() => TestBootstrapper.FakeDatabase.Update(A<Location>._))
                .Invokes((Location location) => updatedLocation = location);

            A.CallTo(() => TestBootstrapper.FakeQueue.EnqueueLocation(A<Location>._))
                .Invokes((Location location) => enqueuedLocation = location);
        }

        private Location LocationSentToDatabase => createdLocation ?? updatedLocation;

        private Task SendNotification() => SendNotification(Notification);

        private async Task SendNotification(Notification notification)
        {
            var mediator = ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(notification);
        }


        public abstract class
            AndTheLocationsServiceIsConfiguredToStoreLocations : WhenReceivingALocationUpdateNotification
        {
            public AndTheLocationsServiceIsConfiguredToStoreLocations()
            {
                this.TestBootstrapper.ServiceConfiguration.EnqueueLocations = false;
                this.TestBootstrapper.ServiceConfiguration.StoreLocations = true;
            }
            
            [Theory]
            [InlineData("LTE", "tel:+97123725123")]
            [InlineData("TPI", "Driver1")]
            [InlineData("D20", "12345")]
            public async Task ThenTheIdPassedToTheDatabaseShouldMatchThePublicAlgorithm(
                string source,
                string sourceId)
            {
                var notification = Notification with { Source = source, SourceId = sourceId };
                notification = source == "LTE" ? notification with { LteMetaData = lteLocationMetaData, TpiMetaData = null } : notification;

                await SendNotification(notification);

                LocationSentToDatabase.LocationId.Should()
                    .Be(Utils.GetLocationId(notification.Source, notification.SourceId));
            }

            [Theory]
            [InlineData("LTE")]
            [InlineData("D20")]
            [InlineData("TPI")]
            public async Task ThenTheSourcePassedToTheDatabaseShouldMatchTheNotification(
                string source)
            {
                var notification = Notification with {Source = source};
                notification = source == "LTE" ? notification with { LteMetaData = lteLocationMetaData, TpiMetaData = null } : notification;

                await SendNotification(notification);

                LocationSentToDatabase.Source.Should().Be(source);
            }

            [Theory]
            [InlineData("tel:+912832452")]
            [InlineData("sip:32.1@apd.com")]
            [InlineData("12345")]
            [InlineData("Driver1")]
            public async Task ThenTheSourceIdPassedToTheDatabaseShouldMatchTheNotification(
                string sourceId)
            {
                await SendNotification(Notification with { SourceId = sourceId });

                LocationSentToDatabase.SourceId.Should().Be(sourceId);
            }

            [Fact]
            public async Task ThenTheTimestampOnTheLocationPushedToTheDatabaseShouldBeAfterTheTestStartTime()
            {
                var testStartTime = DateTime.Now;

                await SendNotification();

                LocationSentToDatabase.Timestamp.Should().BeAfter(testStartTime);
            }

            [Fact]
            public async Task ThenTheTimestampOnTheLocationPushedToTheDatabaseShouldBeBeforeTheTestEndTime()
            {
                await SendNotification();

                LocationSentToDatabase.Timestamp.Should().BeBefore(DateTime.Now);
            }

            public class AndTheLocationIsFromTheTpi : AndTheLocationsServiceIsConfiguredToStoreLocations
            {
                protected override Notification Notification { get; } = new()
                {
                    Source = "TPI",
                    SourceId = "Driver1",
                    TpiMetaData = new TpiLocationMetaData
                    {
                        Resource = "Driver1",
                        Bearing = 109,
                        Fix = 5,
                        Inputs = 12,
                        Latitude = 53.12,
                        Longitude = -2.3423,
                        Outputs = 5,
                        Speed = 12,
                        Status = 4,
                        EventText = "some event text",
                        Timestamp = (DateTime.Now - TimeSpan.FromMinutes(1)).ToString(CultureInfo.InvariantCulture),
                        Class = "SomeClass",
                        Contents = 12,
                        Data = "SomeData",
                        EventId = 15,
                        GeoData = 2
                    }
                };

                [Fact]
                public async Task ThenTheTpiMetaDataPassedToTheDatabaseShouldNotBeNull()
                {
                    await SendNotification();

                    LocationSentToDatabase.TpiMetaData.Should().NotBeNull();
                }

                [Theory]
                [InlineData("Driver1")]
                [InlineData("Unit15")]
                public async Task ThenTheResourcePassedToTheDatabaseShouldMatchTheNotification(
                    string resourceName)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Resource = resourceName }, SourceId = resourceName
                    });

                    LocationSentToDatabase.TpiMetaData.Resource.Should().Be(resourceName);
                }

                [Theory]
                [InlineData(127.1234)]
                [InlineData(-13.249)]
                [InlineData(null)]
                public async Task ThenTheBearingPassedToTheDatabaseShouldMatchTheNotification(
                    double? bearing)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Bearing = bearing }
                    });

                    LocationSentToDatabase.TpiMetaData.Bearing.Should().Be(bearing);
                }

                [Theory]
                [InlineData(0x00)]
                [InlineData(0x01)]
                [InlineData(0x02)]
                public async Task ThenTheFixPassedToTheDatabaseShouldMatchTheNotification(
                    int fix)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Fix = fix }
                    });

                    LocationSentToDatabase.TpiMetaData.Fix.Should().Be(fix);
                }

                [Theory]
                [InlineData(19)]
                [InlineData(27)]
                [InlineData(1)]
                [InlineData(null)]
                public async Task ThenTheInputsPassedToTheDatabaseShouldMatchTheNotification(
                    int? inputs)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Inputs = inputs }
                    });

                    LocationSentToDatabase.TpiMetaData.Inputs.Should().Be(inputs);
                }

                [Theory]
                [InlineData(3)]
                [InlineData(5)]
                [InlineData(12)]
                [InlineData(null)]
                public async Task ThenTheOutputsPassedToTheDatabaseShouldMatchTheNotification(
                    int? outputs)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Outputs = outputs }
                    });

                    LocationSentToDatabase.TpiMetaData.Outputs.Should().Be(outputs);
                }

                [Theory]
                [InlineData(31.92830123)]
                [InlineData(-5.12315123)]
                [InlineData(12)]
                [InlineData(null)]
                public async Task ThenTheLatitudePassedToTheDatabaseShouldMatchTheNotification(
                    double? latitude)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Latitude = latitude }
                    });

                    LocationSentToDatabase.TpiMetaData.Latitude.Should().Be(latitude);
                }

                [Theory]
                [InlineData(31.92830123)]
                [InlineData(-5.12315123)]
                [InlineData(12)]
                [InlineData(null)]
                public async Task ThenTheLongitudePassedToTheDatabaseShouldMatchTheNotification(
                    double? longitude)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Longitude = longitude }
                    });

                    LocationSentToDatabase.TpiMetaData.Longitude.Should().Be(longitude);
                }

                [Theory]
                [InlineData(35.30123)]
                [InlineData(-3.123123)]
                [InlineData(14.00001)]
                [InlineData(null)]
                public async Task ThenTheSpeedPassedToTheDatabaseShouldMatchTheNotification(
                    double? speed)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Speed = speed }
                    });

                    LocationSentToDatabase.TpiMetaData.Speed.Should().Be(speed);
                }

                [Theory]
                [InlineData(1)]
                [InlineData(4)]
                [InlineData(0)]
                [InlineData(null)]
                public async Task ThenTheStatusPassedToTheDatabaseShouldMatchTheNotification(
                    int? status)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Status = status }
                    });

                    LocationSentToDatabase.TpiMetaData.Status.Should().Be(status);
                }

                [Theory]
                [InlineData("Hello world")]
                [InlineData("Some event text")]
                [InlineData("")]
                [InlineData(null)]
                public async Task ThenTheEventTextPassedToTheDatabaseShouldMatchTheNotification(
                    string eventText)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { EventText = eventText }
                    });

                    LocationSentToDatabase.TpiMetaData.EventText.Should().Be(eventText);
                }

                [Fact]
                public async Task ThenTheTimestampPassedToTheDatabaseShouldMatchTheNotification()
                {
                    await SendNotification();

                    LocationSentToDatabase.TpiMetaData.Timestamp.Should().Be(Notification.TpiMetaData.Timestamp);
                }

                [Fact]
                public async Task ThenTheClassPassedToTheDatabaseShouldMatchTheNotification()
                {
                    await SendNotification();

                    LocationSentToDatabase.TpiMetaData.Class.Should().Be(Notification.TpiMetaData.Class);
                }

                [Fact]
                public async Task ThenTheContentsPassedToTheDatabaseShouldMatchTheNotification()
                {
                    await SendNotification();

                    LocationSentToDatabase.TpiMetaData.Contents.Should().Be(Notification.TpiMetaData.Contents);
                }

                [Fact]
                public async Task ThenTheEventIdPassedToTheDatabaseShouldMatchTheNotification()
                {
                    await SendNotification();

                    LocationSentToDatabase.TpiMetaData.EventId.Should().Be(Notification.TpiMetaData.EventId);
                }

                [Fact]
                public async Task ThenTheDataPassedToTheDatabaseShouldMatchTheNotification()
                {
                    await SendNotification();

                    LocationSentToDatabase.TpiMetaData.Data.Should().Be(Notification.TpiMetaData.Data);
                }

                [Fact]
                public async Task ThenTheGeoDataPassedToTheDatabaseShouldMatchTheNotification()
                {
                    await SendNotification();

                    LocationSentToDatabase.TpiMetaData.GeoData.Should().Be(Notification.TpiMetaData.GeoData);
                }
            }
        }

        public abstract class
            AndTheLocationsServiceIsConfiguredToEnqueueLocations : WhenReceivingALocationUpdateNotification
        {
            public AndTheLocationsServiceIsConfiguredToEnqueueLocations()
            {
                this.TestBootstrapper.ServiceConfiguration.EnqueueLocations = true;
                this.TestBootstrapper.ServiceConfiguration.StoreLocations = false;
            }
            
            [Fact]
            public async Task ThenTheLocationShouldBePushedToTheQueue()
            {
                await SendNotification();

                enqueuedLocation.Should().NotBeNull();
            }

            [Theory]
            [InlineData("LTE", "tel:+97123725123")]
            [InlineData("TPI", "Driver1")]
            [InlineData("D20", "12345")]
            public async Task ThenTheIdPassedToTheQueueShouldMatchThePublicAlgorithm(
                string source,
                string sourceId)
            {
                var notification = Notification with { Source = source, SourceId = sourceId};
                notification = source == "LTE" ? notification with { LteMetaData = lteLocationMetaData, TpiMetaData = null} : notification;

                await SendNotification(notification);

                enqueuedLocation.LocationId.Should()
                    .Be(Utils.GetLocationId(notification.Source, notification.SourceId));
            }

            [Theory]
            [InlineData("LTE")]
            [InlineData("D20")]
            [InlineData("TPI")]
            public async Task ThenTheSourcePassedToTheQueueShouldMatchTheNotification(
                string source)
            {
                var notification = Notification with { Source = source};
                notification = source == "LTE" ? notification with { LteMetaData = lteLocationMetaData, TpiMetaData = null } : notification;

                await SendNotification(notification);

                enqueuedLocation.Source.Should().Be(source);
            }

            [Theory]
            [InlineData("tel:+912832452")]
            [InlineData("sip:32.1@apd.com")]
            [InlineData("12345")]
            [InlineData("Driver1")]
            public async Task ThenTheSourceIdPassedToTheQueueShouldMatchTheNotification(
                string sourceId)
            {
                await SendNotification(Notification with { SourceId = sourceId });

                enqueuedLocation.SourceId.Should().Be(sourceId);
            }

            [Fact]
            public async Task ThenTheTimestampOnTheLocationPushedToTheQueueShouldBeAfterTheTestStartTime()
            {
                var testStartTime = DateTime.Now;

                await SendNotification();

                enqueuedLocation.Timestamp.Should().BeAfter(testStartTime);
            }

            [Fact]
            public async Task ThenTheTimestampOnTheLocationPushedToTheQueueShouldBeBeforeTheTestEndTime()
            {
                await SendNotification();

                enqueuedLocation.Timestamp.Should().BeBefore(DateTime.Now);
            }

            public class AndTheLocationIsFromTheTpi : AndTheLocationsServiceIsConfiguredToEnqueueLocations
            {
                protected override Notification Notification { get; } = new()
                {
                    Source = "TPI",
                    SourceId = "Driver1",
                    TpiMetaData = new TpiLocationMetaData
                    {
                        Resource = "Driver1",
                        Bearing = 109,
                        Fix = 5,
                        Inputs = 12,
                        Latitude = 53.12,
                        Longitude = -2.3423,
                        Outputs = 5,
                        Speed = 12,
                        Status = 4,
                        EventText = "some event text",
                        Timestamp = (DateTime.Now - TimeSpan.FromMinutes(1)).ToString(CultureInfo.InvariantCulture),
                        Class = "SomeClass",
                        Contents = 12,
                        Data = "SomeData",
                        EventId = 15,
                        GeoData = 2
                    }
                };

                [Fact]
                public async Task ThenTheTpiMetaDataPassedToTheQueueShouldNotBeNull()
                {
                    await SendNotification();

                    enqueuedLocation.TpiMetaData.Should().NotBeNull();
                }

                [Theory]
                [InlineData("Driver1")]
                [InlineData("Unit15")]
                public async Task ThenTheResourcePassedToTheQueueShouldMatchTheNotification(
                    string resourceName)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Resource = resourceName }, SourceId = resourceName
                    });

                    enqueuedLocation.TpiMetaData.Resource.Should().Be(resourceName);
                }

                [Theory]
                [InlineData(127.1234)]
                [InlineData(-13.249)]
                [InlineData(null)]
                public async Task ThenTheBearingPassedToTheQueueShouldMatchTheNotification(
                    double? bearing)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Bearing = bearing }
                    });

                    enqueuedLocation.TpiMetaData.Bearing.Should().Be(bearing);
                }

                [Theory]
                [InlineData(1)]
                [InlineData(2)]
                [InlineData(3)]
                public async Task ThenTheFixPassedToTheQueueShouldMatchTheNotification(
                    byte fix)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Fix = fix }
                    });

                    enqueuedLocation.TpiMetaData.Fix.Should().Be(fix);
                }

                [Theory]
                [InlineData(19)]
                [InlineData(27)]
                [InlineData(1)]
                [InlineData(null)]
                public async Task ThenTheInputsPassedToTheQueueShouldMatchTheNotification(
                    int? inputs)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Inputs = inputs }
                    });

                    enqueuedLocation.TpiMetaData.Inputs.Should().Be(inputs);
                }

                [Theory]
                [InlineData(3)]
                [InlineData(5)]
                [InlineData(12)]
                [InlineData(null)]
                public async Task ThenTheOutputsPassedToTheQueueShouldMatchTheNotification(
                    int? outputs)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Outputs = outputs }
                    });

                    enqueuedLocation.TpiMetaData.Outputs.Should().Be(outputs);
                }

                [Theory]
                [InlineData(-12.81588231234)]
                [InlineData(2.82371231)]
                [InlineData(12)]
                [InlineData(null)]
                public async Task ThenTheLatitudePassedToTheQueueShouldMatchTheNotification(
                    double? latitude)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Latitude = latitude }
                    });

                    enqueuedLocation.TpiMetaData.Latitude.Should().Be(latitude);
                }

                [Theory]
                [InlineData(-12.81588231234)]
                [InlineData(2.82371231)]
                [InlineData(12)]
                [InlineData(null)]
                public async Task ThenTheLongitudePassedToTheQueueShouldMatchTheNotification(
                    double? longitude)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Longitude = longitude }
                    });

                    enqueuedLocation.TpiMetaData.Longitude.Should().Be(longitude);
                }

                [Theory]
                [InlineData(-5.81588231234)]
                [InlineData(21.23231)]
                [InlineData(5)]
                [InlineData(null)]
                public async Task ThenTheSpeedPassedToTheQueueShouldMatchTheNotification(
                    double? speed)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Speed = speed }
                    });

                    enqueuedLocation.TpiMetaData.Speed.Should().Be(speed);
                }

                [Theory]
                [InlineData(5)]
                [InlineData(2)]
                [InlineData(1)]
                [InlineData(null)]
                public async Task ThenTheStatusPassedToTheQueueShouldMatchTheNotification(
                    int? status)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { Status = status }
                    });

                    enqueuedLocation.TpiMetaData.Status.Should().Be(status);
                }

                [Theory]
                [InlineData("Hello world")]
                [InlineData("Some more event text")]
                [InlineData("")]
                [InlineData(null)]
                public async Task ThenTheEventTextPassedToTheQueueShouldMatchTheNotification(
                    string eventText)
                {
                    await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with { EventText = eventText }
                    });

                    enqueuedLocation.TpiMetaData.EventText.Should().Be(eventText);
                }

                [Fact]
                public async Task ThenTheTimestampPassedToTheQueueShouldMatchTheNotification()
                {
                    await SendNotification();

                    enqueuedLocation.TpiMetaData.Timestamp.Should().Be(Notification.TpiMetaData.Timestamp);
                }

                [Fact]
                public async Task ThenTheClassPassedToTheQueueShouldMatchTheNotification()
                {
                    await SendNotification();

                    enqueuedLocation.TpiMetaData.Class.Should().Be(Notification.TpiMetaData.Class);
                }
                
                [Fact]
                public async Task ThenTheContentsPassedToTheQueueShouldMatchTheNotification()
                {
                    await SendNotification();

                    enqueuedLocation.TpiMetaData.Contents.Should().Be(Notification.TpiMetaData.Contents);
                }

                [Fact]
                public async Task ThenTheEventIdPassedToTheQueueShouldMatchTheNotification()
                {
                    await SendNotification();

                    enqueuedLocation.TpiMetaData.EventId.Should().Be(Notification.TpiMetaData.EventId);
                }

                [Fact]
                public async Task ThenTheDataPassedToTheQueueShouldMatchTheNotification()
                {
                    await SendNotification();

                    enqueuedLocation.TpiMetaData.Data.Should().Be(Notification.TpiMetaData.Data);
                }

                [Fact]
                public async Task ThenTheGeoDataPassedToTheQueueShouldMatchTheNotification()
                {
                    await SendNotification();

                    enqueuedLocation.TpiMetaData.GeoData.Should().Be(Notification.TpiMetaData.GeoData);
                }
            }
            public class AndTheLocationIsFromLte : AndTheLocationsServiceIsConfiguredToEnqueueLocations
            {
                protected Notification lteNotification { get; } = new()
                {
                    Source = "LTE",
                    SourceId = "Driver1",
                    LteMetaData = new LteLocationMetaData
                    {
                        GroupDisplayName = "APD 1",
                        GroupID = "sip:3.97@poc1p.mcx.esn.gov.uk",
                        ReportType = "Emergency",
                        UserDisplayName = "John Smith 123",
                        UserID = "tel:+912132432122 ",
                        Longitude = -2.3423,
                        Latitude = 53.12
                    }
                };

                [Fact]
                public async Task ThenTheLteMetaDataPassedToTheQueueShouldNotBeNull()
                {
                    await SendNotification(lteNotification);
                    enqueuedLocation.LteMetaData.Should().NotBeNull();
                }

                [Theory]
                [InlineData("tel:+912132432122")]
                [InlineData("tel:+111111111111")]
                [InlineData("tel:+012132432123")]
                public async Task ThenTheUserIDPassedToTheDatabaseShouldMatchTheNotification(
                    string userID)
                {
                    await SendNotification(Notification with
                    {
                        LteMetaData = lteNotification.LteMetaData with { UserID = userID }
                    });

                    enqueuedLocation.LteMetaData.UserID.Should().Be(userID);
                }

                [Theory]
                [InlineData("John Smith 123")]
                [InlineData("Albert Smith 124")]
                [InlineData("Boris Smith 125")]
                public async Task ThenTheUserDisplayNamePassedToTheDatabaseShouldMatchTheNotification(
                    string userDisplayName)
                {
                    await SendNotification(lteNotification with
                    {
                        LteMetaData = lteNotification.LteMetaData with { UserDisplayName = userDisplayName }
                    });

                    enqueuedLocation.LteMetaData.UserDisplayName.Should().Be(userDisplayName);
                }

                [Theory]
                [InlineData("APD 3")]
                [InlineData("Group name")]
                [InlineData("APD 2")]
                public async Task ThenTheGroupDisplayNamePassedToTheDatabaseShouldMatchTheNotification(
                    string groupName)
                {
                    await SendNotification(lteNotification with
                    {
                        LteMetaData = lteNotification.LteMetaData with { GroupDisplayName = groupName }
                    });

                    enqueuedLocation.LteMetaData.GroupDisplayName.Should().Be(groupName);
                }

                [Theory]
                [InlineData("Non Emergency")]
                [InlineData("Emergency")]
                public async Task ThenTheReportTypePassedToTheDatabaseShouldMatchTheNotification(
                    string reportType)
                {
                    await SendNotification(lteNotification with
                    {
                        LteMetaData = lteNotification.LteMetaData with { ReportType = reportType }
                    });

                    enqueuedLocation.LteMetaData.ReportType.Should().Be(reportType);
                }

                [Theory]
                [InlineData("sip:3.97@poc1p.mcx.esn.gov.uk")]
                [InlineData("sip:4.97@poc1p.mcx.esn.gov.uk")]
                [InlineData("sip:5.97@poc1p.mcx.esn.gov.uk")]
                public async Task ThenTheGroupIDPassedToTheDatabaseShouldMatchTheNotification(
                    string groupID)
                {
                    await SendNotification(lteNotification with
                    {
                        LteMetaData = lteNotification.LteMetaData with { GroupID = groupID }
                    });

                    enqueuedLocation.LteMetaData.GroupID.Should().Be(groupID);
                }
            }

        }

        public class AndTheLocationsServiceIsNotConfiguredToStoreLocations : WhenReceivingALocationUpdateNotification
        {
            public AndTheLocationsServiceIsNotConfiguredToStoreLocations()
            {
                this.TestBootstrapper.ServiceConfiguration.EnqueueLocations = false;
                this.TestBootstrapper.ServiceConfiguration.StoreLocations = false;
            }
            
            [Fact]
            public async Task ThenNoCallToTheDatabaseIsInvoked()
            {
                await SendNotification();

                A.CallTo(() => TestBootstrapper.FakeDatabase.Create(A<Location>._))
                    .MustNotHaveHappened();

                A.CallTo(() => TestBootstrapper.FakeDatabase.Get(A<string>._))
                    .MustNotHaveHappened();

                A.CallTo(() => TestBootstrapper.FakeDatabase.Update(A<Location>._))
                    .MustNotHaveHappened();
            }
        }

        public class AndTheLocationsServiceIsConfiguredToStoreAndEnqueue : WhenReceivingALocationUpdateNotification
        {
            public AndTheLocationsServiceIsConfiguredToStoreAndEnqueue()
            {
                this.TestBootstrapper.ServiceConfiguration.EnqueueLocations = true;
                this.TestBootstrapper.ServiceConfiguration.StoreLocations = true;
            }
            
            [Fact]
            public async Task ThenTheTimestampOnTheRecordPushedToTheDatabaseAndTheElementPushedToTheQueueShouldMatch()
            {
                await SendNotification();

                LocationSentToDatabase.Timestamp.Should().Be(enqueuedLocation.Timestamp);
            }
        }

        public class AndTheLocationUpdateIsHistoric : WhenReceivingALocationUpdateNotification
        {
            protected override Notification Notification { get; } = new()
            {
                Source = "TPI",
                SourceId = "Driver1",
                TpiMetaData = new TpiLocationMetaData
                {
                    Resource = "Driver1",
                    Bearing = 109,
                    Fix = 5,
                    Inputs = 12,
                    Latitude = 53.12,
                    Longitude = -2.3423,
                    Outputs = 5,
                    Speed = 12,
                    Status = 4,
                    EventText = "some event text",
                    Timestamp = (new DateTime(1970, 10, 15, 0, 0, 0)).ToString(CultureInfo.InvariantCulture),
                    Class = "SomeClass",
                    Contents = 12,
                    Data = "SomeData",
                    EventId = 15,
                    GeoData = 2
                }
            };

            public AndTheLocationUpdateIsHistoric()
            {
                this.TestBootstrapper.ServiceConfiguration.EnqueueLocations = true;
                this.TestBootstrapper.ServiceConfiguration.StoreLocations = true;

                A.CallTo(() =>
                        TestBootstrapper.FakeDatabase.Get(
                            Utils.GetLocationId(Notification.Source, Notification.SourceId)))
                    .Returns(new Location { TpiMetaData = new TpiLocationMetaData() { Timestamp = DateTime.Now.ToString(CultureInfo.InvariantCulture) }, Source = "TPI", SourceId = "Driver1", LocationId = Utils.GetLocationId("TPI", "Driver1") });
            }

            [Fact]
            public async Task ThenTheHistoricDataShouldNotOverwriteTheCurrentLocation()
            {
                await SendNotification();

                A.CallTo(() => TestBootstrapper.FakeDatabase.Update(A<Location>._)).MustNotHaveHappened();
            }

            [Fact]
            public async Task ThenTheHistoricDataShouldBeEnqueued()
            {
                await SendNotification();

                A.CallTo(() => TestBootstrapper.FakeQueue.EnqueueLocation(A<Location>._)).MustHaveHappened();
            }

            [Fact]
            public async Task ThenTheHistoricDataWithTimeStampInUTCShouldNotOverwriteTheCurrentLocation()
            {
                await SendNotification(Notification with
                    {
                      TpiMetaData = Notification.TpiMetaData with
                      {
                          Timestamp = (new DateTime(1970, 10, 15, 0, 0, 0)).ToString("O")
                      }
                    }
                );

                A.CallTo(() => TestBootstrapper.FakeDatabase.Update(A<Location>._)).MustNotHaveHappened();
            }

            [Fact]
            public async Task ThenTheHistoricDataWithTimeStampInUTCShouldHaveBeenEnqueued()
            {
                await SendNotification(Notification with
                    {
                        TpiMetaData = Notification.TpiMetaData with
                        {
                            Timestamp = (new DateTime(1970, 10, 15, 0, 0, 0)).ToString("O")
                        }
                    }
                );

                A.CallTo(() => TestBootstrapper.FakeQueue.EnqueueLocation(A<Location>._)).MustHaveHappened();
            }
        }

        public class AndTheLocationsServiceIsNotConfiguredToEnqueueLocations : WhenReceivingALocationUpdateNotification
        {
            public AndTheLocationsServiceIsNotConfiguredToEnqueueLocations()
            {
                this.TestBootstrapper.ServiceConfiguration.EnqueueLocations = false;
                this.TestBootstrapper.ServiceConfiguration.StoreLocations = false;
            }
            
            [Fact]
            public async Task ThenEnqueueIsNotInvoked()
            {
                await SendNotification();

                A.CallTo(() => TestBootstrapper.FakeQueue.EnqueueLocation(A<Location>._))
                    .MustNotHaveHappened();
            }
        }

        public class AndTheNotificationIsForANewLocation : WhenReceivingALocationUpdateNotification
        {

            [Fact]
            public async Task ThenACallToCreateTheEntryShouldBeMade()
            {
                await SendNotification();

                createdLocation.Should().NotBeNull();
                updatedLocation.Should().BeNull();
            }
        }

        public sealed class AndTheNotificationIsForAnExistingLocation : WhenReceivingALocationUpdateNotification
        {
            public AndTheNotificationIsForAnExistingLocation()
            {
                A.CallTo(() =>
                        TestBootstrapper.FakeDatabase.Get(
                            Utils.GetLocationId(Notification.Source, Notification.SourceId)))
                    .Returns(new Location
                    {
                        Source = Notification.Source, 
                        SourceId = Notification.SourceId,
                        TpiMetaData = new TpiLocationMetaData()
                        {
                            Timestamp = (DateTime.Now -TimeSpan.FromMinutes(2)).ToString(CultureInfo.InvariantCulture)
                        }
                    });
            }

            [Fact]
            public async Task ThenACallToUpdateTheEntryShouldBeMade()
            {
                await SendNotification();

                updatedLocation.Should().NotBeNull();
                createdLocation.Should().BeNull();
            }
        }

        public class AndTheLocationIsFromTheTpi : WhenReceivingALocationUpdateNotification
        {
            protected override Notification Notification { get; } = new()
            {
                Source = "TPI",
                SourceId = "Driver1",
                TpiMetaData = new TpiLocationMetaData
                {
                    Resource = "Driver1",
                    Bearing = 109,
                    Fix = 5,
                    Inputs = 12,
                    Latitude = 53.12,
                    Longitude = -2.3423,
                    Outputs = 5,
                    Speed = 12,
                    Status = 4,
                    EventText = "some event text",
                    Timestamp = (DateTime.Now - TimeSpan.FromMinutes(1)).ToString(CultureInfo.InvariantCulture),
                    Class = "SomeClass",
                    Contents = 12,
                    Data = "SomeData",
                    EventId = 15,
                    GeoData = 2
                }
            };

            [Fact]
            public async Task ThenTheTpiMetaDataPassedToTheDatabaseShouldNotBeNull()
            {
                await SendNotification();

                LocationSentToDatabase.TpiMetaData.Should().NotBeNull();
            }

            [Fact]
            public async Task ThenTheTpiMetaDataPassedToTheQueueShouldNotBeNull()
            {
                await SendNotification();

                enqueuedLocation.TpiMetaData.Should().NotBeNull();
            }

            [Theory]
            [InlineData("Driver1")]
            [InlineData("Unit15")]
            public async Task ThenTheResourcePassedToTheDatabaseShouldMatchTheNotification(
                string resourceName)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Resource = resourceName }, SourceId = resourceName
                });

                LocationSentToDatabase.TpiMetaData.Resource.Should().Be(resourceName);
            }

            [Theory]
            [InlineData("Driver1")]
            [InlineData("Unit15")]
            public async Task ThenTheResourcePassedToTheQueueShouldMatchTheNotification(
                string resourceName)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Resource = resourceName }, SourceId = resourceName
                });

                enqueuedLocation.TpiMetaData.Resource.Should().Be(resourceName);
            }

            [Theory]
            [InlineData(127.1234)]
            [InlineData(-13.249)]
            [InlineData(null)]
            public async Task ThenTheBearingPassedToTheDatabaseShouldMatchTheNotification(
                double? bearing)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Bearing = bearing }
                });

                LocationSentToDatabase.TpiMetaData.Bearing.Should().Be(bearing);
            }

            [Theory]
            [InlineData(127.1234)]
            [InlineData(-13.249)]
            [InlineData(null)]
            public async Task ThenTheBearingPassedToTheQueueShouldMatchTheNotification(
                double? bearing)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Bearing = bearing }
                });

                enqueuedLocation.TpiMetaData.Bearing.Should().Be(bearing);
            }

            [Theory]
            [InlineData(0x00)]
            [InlineData(0x01)]
            [InlineData(0x02)]
            public async Task ThenTheFixPassedToTheDatabaseShouldMatchTheNotification(
                int fix)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Fix = fix }
                });

                LocationSentToDatabase.TpiMetaData.Fix.Should().Be(fix);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            public async Task ThenTheFixPassedToTheQueueShouldMatchTheNotification(
                byte fix)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Fix = fix }
                });

                enqueuedLocation.TpiMetaData.Fix.Should().Be(fix);
            }

            [Theory]
            [InlineData(19)]
            [InlineData(27)]
            [InlineData(1)]
            [InlineData(null)]
            public async Task ThenTheInputsPassedToTheDatabaseShouldMatchTheNotification(
                int? inputs)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Inputs = inputs }
                });

                LocationSentToDatabase.TpiMetaData.Inputs.Should().Be(inputs);
            }

            [Theory]
            [InlineData(19)]
            [InlineData(27)]
            [InlineData(1)]
            [InlineData(null)]
            public async Task ThenTheInputsPassedToTheQueueShouldMatchTheNotification(
                int? inputs)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Inputs = inputs }
                });

                enqueuedLocation.TpiMetaData.Inputs.Should().Be(inputs);
            }

            [Theory]
            [InlineData(3)]
            [InlineData(5)]
            [InlineData(12)]
            [InlineData(null)]
            public async Task ThenTheOutputsPassedToTheDatabaseShouldMatchTheNotification(
                int? outputs)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Outputs = outputs }
                });

                LocationSentToDatabase.TpiMetaData.Outputs.Should().Be(outputs);
            }

            [Theory]
            [InlineData(3)]
            [InlineData(5)]
            [InlineData(12)]
            [InlineData(null)]
            public async Task ThenTheOutputsPassedToTheQueueShouldMatchTheNotification(
                int? outputs)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Outputs = outputs }
                });

                enqueuedLocation.TpiMetaData.Outputs.Should().Be(outputs);
            }

            [Theory]
            [InlineData(31.92830123)]
            [InlineData(-5.12315123)]
            [InlineData(12)]
            [InlineData(null)]
            public async Task ThenTheLatitudePassedToTheDatabaseShouldMatchTheNotification(
                double? latitude)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Latitude = latitude }
                });

                LocationSentToDatabase.TpiMetaData.Latitude.Should().Be(latitude);
            }

            [Theory]
            [InlineData(-12.81588231234)]
            [InlineData(2.82371231)]
            [InlineData(12)]
            [InlineData(null)]
            public async Task ThenTheLatitudePassedToTheQueueShouldMatchTheNotification(
                double? latitude)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Latitude = latitude }
                });

                enqueuedLocation.TpiMetaData.Latitude.Should().Be(latitude);
            }

            [Theory]
            [InlineData(31.92830123)]
            [InlineData(-5.12315123)]
            [InlineData(12)]
            [InlineData(null)]
            public async Task ThenTheLongitudePassedToTheDatabaseShouldMatchTheNotification(
                double? longitude)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Longitude = longitude }
                });

                LocationSentToDatabase.TpiMetaData.Longitude.Should().Be(longitude);
            }

            [Theory]
            [InlineData(-12.81588231234)]
            [InlineData(2.82371231)]
            [InlineData(12)]
            [InlineData(null)]
            public async Task ThenTheLongitudePassedToTheQueueShouldMatchTheNotification(
                double? longitude)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Longitude = longitude }
                });

                enqueuedLocation.TpiMetaData.Longitude.Should().Be(longitude);
            }

            [Theory]
            [InlineData(35.30123)]
            [InlineData(-3.123123)]
            [InlineData(14.00001)]
            [InlineData(null)]
            public async Task ThenTheSpeedPassedToTheDatabaseShouldMatchTheNotification(
                double? speed)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Speed = speed }
                });

                LocationSentToDatabase.TpiMetaData.Speed.Should().Be(speed);
            }

            [Theory]
            [InlineData(-5.81588231234)]
            [InlineData(21.23231)]
            [InlineData(5)]
            [InlineData(null)]
            public async Task ThenTheSpeedPassedToTheQueueShouldMatchTheNotification(
                double? speed)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Speed = speed }
                });

                enqueuedLocation.TpiMetaData.Speed.Should().Be(speed);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(4)]
            [InlineData(0)]
            [InlineData(null)]
            public async Task ThenTheStatusPassedToTheDatabaseShouldMatchTheNotification(
                int? status)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Status = status }
                });

                LocationSentToDatabase.TpiMetaData.Status.Should().Be(status);
            }

            [Theory]
            [InlineData(5)]
            [InlineData(2)]
            [InlineData(1)]
            [InlineData(null)]
            public async Task ThenTheStatusPassedToTheQueueShouldMatchTheNotification(
                int? status)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { Status = status }
                });

                enqueuedLocation.TpiMetaData.Status.Should().Be(status);
            }

            [Theory]
            [InlineData("Hello world")]
            [InlineData("Some event text")]
            [InlineData("")]
            [InlineData(null)]
            public async Task ThenTheEventTextPassedToTheDatabaseShouldMatchTheNotification(
                string eventText)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { EventText = eventText }
                });

                LocationSentToDatabase.TpiMetaData.EventText.Should().Be(eventText);
            }

            [Theory]
            [InlineData("Hello world")]
            [InlineData("Some more event text")]
            [InlineData("")]
            [InlineData(null)]
            public async Task ThenTheEventTextPassedToTheQueueShouldMatchTheNotification(
                string eventText)
            {
                await SendNotification(Notification with
                {
                    TpiMetaData = Notification.TpiMetaData with { EventText = eventText }
                });

                enqueuedLocation.TpiMetaData.EventText.Should().Be(eventText);
            }

            [Fact]
            public async Task ThenTheTimestampPassedToTheDatabaseShouldMatchTheNotification()
            {
                await SendNotification();

                LocationSentToDatabase.TpiMetaData.Timestamp.Should().Be(Notification.TpiMetaData.Timestamp);
            }

            [Fact]
            public async Task ThenTheTimestampPassedToTheQueueShouldMatchTheNotification()
            {
                await SendNotification();

                enqueuedLocation.TpiMetaData.Timestamp.Should().Be(Notification.TpiMetaData.Timestamp);
            }

            [Fact]
            public async Task ThenTheClassPassedToTheDatabaseShouldMatchTheNotification()
            {
                await SendNotification();

                LocationSentToDatabase.TpiMetaData.Class.Should().Be(Notification.TpiMetaData.Class);
            }

            [Fact]
            public async Task ThenTheClassPassedToTheQueueShouldMatchTheNotification()
            {
                await SendNotification();

                enqueuedLocation.TpiMetaData.Class.Should().Be(Notification.TpiMetaData.Class);
            }

            [Fact]
            public async Task ThenTheContentsPassedToTheDatabaseShouldMatchTheNotification()
            {
                await SendNotification();

                LocationSentToDatabase.TpiMetaData.Contents.Should().Be(Notification.TpiMetaData.Contents);
            }

            [Fact]
            public async Task ThenTheContentsPassedToTheQueueShouldMatchTheNotification()
            {
                await SendNotification();

                enqueuedLocation.TpiMetaData.Contents.Should().Be(Notification.TpiMetaData.Contents);
            }

            [Fact]
            public async Task ThenTheEventIdPassedToTheDatabaseShouldMatchTheNotification()
            {
                await SendNotification();

                LocationSentToDatabase.TpiMetaData.EventId.Should().Be(Notification.TpiMetaData.EventId);
            }

            [Fact]
            public async Task ThenTheEventIdPassedToTheQueueShouldMatchTheNotification()
            {
                await SendNotification();

                enqueuedLocation.TpiMetaData.EventId.Should().Be(Notification.TpiMetaData.EventId);
            }

            [Fact]
            public async Task ThenTheDataPassedToTheDatabaseShouldMatchTheNotification()
            {
                await SendNotification();

                LocationSentToDatabase.TpiMetaData.Data.Should().Be(Notification.TpiMetaData.Data);
            }

            [Fact]
            public async Task ThenTheDataPassedToTheQueueShouldMatchTheNotification()
            {
                await SendNotification();

                enqueuedLocation.TpiMetaData.Data.Should().Be(Notification.TpiMetaData.Data);
            }

            [Fact]
            public async Task ThenTheGeoDataPassedToTheDatabaseShouldMatchTheNotification()
            {
                await SendNotification();

                LocationSentToDatabase.TpiMetaData.GeoData.Should().Be(Notification.TpiMetaData.GeoData);
            }

            [Fact]
            public async Task ThenTheGeoDataPassedToTheQueueShouldMatchTheNotification()
            {
                await SendNotification();

                enqueuedLocation.TpiMetaData.GeoData.Should().Be(Notification.TpiMetaData.GeoData);
            }
        }

        public class AndTheLocationIsFromLte : WhenReceivingALocationUpdateNotification
        {
            protected override Notification Notification { get; } = new()
            {
                Source = "LTE",
                SourceId = "Driver1",
                LteMetaData = new LteLocationMetaData
                {
                    GroupDisplayName = "APD 1",
                    GroupID = "sip:3.97@poc1p.mcx.esn.gov.uk",
                    ReportType = "Emergency",
                    UserDisplayName = "John Smith 123",
                    UserID = "tel:+912132432122 ",
                    Longitude = -2.3423,
                    Latitude = 53.12
                }
            };

            [Fact]
            public async Task ThenTheLteMetaDataPassedToTheQueueShouldNotBeNull()
            {
                await SendNotification(Notification);
                LocationSentToDatabase.LteMetaData.Should().NotBeNull();
            }

            [Theory]
            [InlineData("tel:+912132432122")]
            [InlineData("tel:+111111111111")]
            [InlineData("tel:+012132432123")]
            public async Task ThenTheUserIDPassedToTheDatabaseShouldMatchTheNotification(
                string userID)
            {
                await SendNotification(Notification with
                {
                    LteMetaData = Notification.LteMetaData with { UserID = userID }
                });

                LocationSentToDatabase.LteMetaData.UserID.Should().Be(userID);
            }

            [Theory]
            [InlineData("John Smith 123")]
            [InlineData("Albert Smith 124")]
            [InlineData("Boris Smith 125")]
            public async Task ThenTheUserDisplayNamePassedToTheDatabaseShouldMatchTheNotification(
                string userDisplayName)
            {
                await SendNotification(Notification with
                {
                    LteMetaData = Notification.LteMetaData with { UserDisplayName = userDisplayName }
                });

                LocationSentToDatabase.LteMetaData.UserDisplayName.Should().Be(userDisplayName);
            }

            [Theory]
            [InlineData("APD 3")]
            [InlineData("Group name")]
            [InlineData("APD 2")]
            public async Task ThenTheGroupDisplayNamePassedToTheDatabaseShouldMatchTheNotification(string groupName)
            {
                await SendNotification(Notification with
                {
                    LteMetaData = Notification.LteMetaData with { GroupDisplayName = groupName }
                });

                LocationSentToDatabase.LteMetaData.GroupDisplayName.Should().Be(groupName);
            }

            [Theory]
            [InlineData("Non Emergency")]
            [InlineData("Emergency")]
            public async Task ThenTheReportTypePassedToTheDatabaseShouldMatchTheNotification(
                string reportType)
            {
                await SendNotification(Notification with
                {
                    LteMetaData = Notification.LteMetaData with { ReportType = reportType }
                });

                LocationSentToDatabase.LteMetaData.ReportType.Should().Be(reportType);
            }

            [Theory]
            [InlineData("sip:3.97@poc1p.mcx.esn.gov.uk")]
            [InlineData("sip:4.97@poc1p.mcx.esn.gov.uk")]
            [InlineData("sip:5.97@poc1p.mcx.esn.gov.uk")]
            public async Task ThenTheGroupIDPassedToTheDatabaseShouldMatchTheNotification(
                string groupID)
            {
                await SendNotification(Notification with
                {
                    LteMetaData = Notification.LteMetaData with { GroupID = groupID }
                });

                LocationSentToDatabase.LteMetaData.GroupID.Should().Be(groupID);
            }
        }
    }
}