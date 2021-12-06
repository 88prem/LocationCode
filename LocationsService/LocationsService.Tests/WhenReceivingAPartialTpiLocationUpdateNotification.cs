namespace Com.Apdcomms.LocationsService.Tests
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService;
    using Com.Apdcomms.DataGateway.LocationsService.Notifications;
    using Com.Apdcomms.DataGateway.LocationsService.Tpi;
    using FakeItEasy;
    using FluentAssertions;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public abstract class WhenReceivingAPartialTpiLocationUpdateNotification : TestBase
    {
        private const string Source = "TPI";
        private const string SourceId = "Driver1";
        
        private readonly Location existingLocationInDatabase = new()
        {
            LocationId = Utils.GetLocationId(Source, SourceId),
            Source = Source,
            SourceId = SourceId,
            Timestamp = DateTime.Now - TimeSpan.FromHours(1),
            TpiMetaData = new TpiLocationMetaData
            {
                Bearing = 14.32834,
                Fix = 3,
                Inputs = 4,
                Outputs = 6,
                Latitude = -2.5232123,
                Longitude = 18.98234,
                Speed = 47.125116,
                Status = 1,
                Timestamp = (DateTime.Now - TimeSpan.FromHours(1) - TimeSpan.FromMinutes(1)).ToString(CultureInfo.InvariantCulture),
                EventText = "Something happened an hour ago",
                Resource = SourceId,
                Class = "AClassName",
                Contents = 0xAB,
                Data = "HERE IS SOME CAPITALIZED DATA FOR EXAMPLE",
                EventId = 0x11,
                GeoData = 0x5F
            }
        };

        private Location savedLocation;
        private Location enqueuedLocation;

        public WhenReceivingAPartialTpiLocationUpdateNotification()
        {
            A.CallTo(() => TestBootstrapper.FakeDatabase.Update(A<Location>._))
                .Invokes((Location location) => savedLocation = location);

            A.CallTo(() => TestBootstrapper.FakeQueue.EnqueueLocation(A<Location>._))
                .Invokes((Location location) => enqueuedLocation = location);

            A.CallTo(() =>
                    TestBootstrapper.FakeDatabase.Get(Utils.GetLocationId(Notification.Source,
                        Notification.SourceId)))
                .Returns(new Location { Source = Source, SourceId = SourceId });

            A.CallTo(() => TestBootstrapper.FakeDatabase.Get(Utils.GetLocationId(Notification.Source,
                    Notification.SourceId)))
                .Returns(existingLocationInDatabase);
        }

        protected virtual Notification Notification { get; } = new()
        {
            Source = Source,
            SourceId = SourceId,
            TpiMetaData = new TpiLocationMetaData
            {
                Bearing = -12.3,
                Fix = 1,
                Inputs = 0,
                Outputs = 5,
                Latitude = -4.125233,
                Longitude = 19.019234,
                Speed = 16.231905,
                Status = 4,
                Timestamp = (DateTime.Now - TimeSpan.FromMinutes(1)).ToString(CultureInfo.InvariantCulture),
                EventText = "Some occurrence has just occurred",
                Resource = "Driver1",
                Class = "Class of 69",
                Contents = 0xAF,
                Data = "Special Containment Procedures",
                EventId = 0x0F,
                GeoData = 0x10
            }
        };

        private async Task PublishNotification(Notification notification)
        {
            var mediator = ServiceProvider.GetRequiredService<IMediator>();
            
            await mediator.Publish(notification);
        }

        private Task PublishNotification() => PublishNotification(Notification);

        [Fact]
        public async Task ThenAnUpdateShouldHaveBeenMadeToTheDatabaseEntry()
        {
            await PublishNotification();

            savedLocation.Should().NotBeNull();
        }

        [Fact]
        public async Task ThenAnUpdateShouldHaveBeenPushedToTheQueue()
        {
            await PublishNotification();

            enqueuedLocation.Should().NotBeNull();
        }
        
        public class AndTheLatitudeIsNotPresent : WhenReceivingAPartialTpiLocationUpdateNotification
        {
            protected override Notification Notification => base.Notification with
            {
                TpiMetaData = base.Notification.TpiMetaData with { Latitude = null }
            };


            [Fact]
            public async Task ThenLatitudeShouldNotBeChangedInTheDatabaseCall()
            {
                await PublishNotification();

                savedLocation.TpiMetaData.Latitude.Should().Be(existingLocationInDatabase.TpiMetaData.Latitude);
            }

            [Fact]
            public async Task ThenLatitudeShouldNotBeChangedInTheQueueCall()
            {
                await PublishNotification();
                
                enqueuedLocation.TpiMetaData.Latitude.Should().Be(existingLocationInDatabase.TpiMetaData.Latitude);
            }
        }
        
        public class AndTheLongitudeIsNotPresent : WhenReceivingAPartialTpiLocationUpdateNotification
        {
            protected override Notification Notification => base.Notification with
            {
                TpiMetaData = base.Notification.TpiMetaData with { Longitude = null }
            };


            [Fact]
            public async Task ThenLongitudeShouldNotBeChangedInTheDatabaseCall()
            {
                await PublishNotification();

                savedLocation.TpiMetaData.Longitude.Should().Be(existingLocationInDatabase.TpiMetaData.Longitude);
            }

            [Fact]
            public async Task ThenLongitudeShouldNotBeChangedInTheQueueCall()
            {
                await PublishNotification();
                
                enqueuedLocation.TpiMetaData.Longitude.Should().Be(existingLocationInDatabase.TpiMetaData.Longitude);
            }
        }
        
        public class AndTheSpeedIsNotPresent : WhenReceivingAPartialTpiLocationUpdateNotification
        {
            protected override Notification Notification => base.Notification with
            {
                TpiMetaData = base.Notification.TpiMetaData with { Speed = null }
            };


            [Fact]
            public async Task ThenSpeedShouldNotBeChangedInTheDatabaseCall()
            {
                await PublishNotification();

                savedLocation.TpiMetaData.Speed.Should().Be(existingLocationInDatabase.TpiMetaData.Speed);
            }

            [Fact]
            public async Task ThenSpeedShouldNotBeChangedInTheQueueCall()
            {
                await PublishNotification();
                
                enqueuedLocation.TpiMetaData.Speed.Should().Be(existingLocationInDatabase.TpiMetaData.Speed);
            }
        }
        
        public class AndTheBearingIsNotPresent : WhenReceivingAPartialTpiLocationUpdateNotification
        {
            protected override Notification Notification => base.Notification with
            {
                TpiMetaData = base.Notification.TpiMetaData with { Bearing = null }
            };


            [Fact]
            public async Task ThenBearingShouldNotBeChangedInTheDatabaseCall()
            {
                await PublishNotification();

                savedLocation.TpiMetaData.Bearing.Should().Be(existingLocationInDatabase.TpiMetaData.Bearing);
            }

            [Fact]
            public async Task ThenBearingShouldNotBeChangedInTheQueueCall()
            {
                await PublishNotification();
                
                enqueuedLocation.TpiMetaData.Bearing.Should().Be(existingLocationInDatabase.TpiMetaData.Bearing);
            }
        }
        
        public class AndTheStatusIsNotPresent : WhenReceivingAPartialTpiLocationUpdateNotification
        {
            protected override Notification Notification => base.Notification with
            {
                TpiMetaData = base.Notification.TpiMetaData with { Status = null }
            };


            [Fact]
            public async Task ThenStatusShouldNotBeChangedInTheDatabaseCall()
            {
                await PublishNotification();

                savedLocation.TpiMetaData.Status.Should().Be(existingLocationInDatabase.TpiMetaData.Status);
            }

            [Fact]
            public async Task ThenStatusShouldNotBeChangedInTheQueueCall()
            {
                await PublishNotification();
                
                enqueuedLocation.TpiMetaData.Status.Should().Be(existingLocationInDatabase.TpiMetaData.Status);
            }
        }
        
        public class AndTheInputsAreNotPresent : WhenReceivingAPartialTpiLocationUpdateNotification
        {
            protected override Notification Notification => base.Notification with
            {
                TpiMetaData = base.Notification.TpiMetaData with { Inputs = null }
            };


            [Fact]
            public async Task ThenInputsShouldNotBeChangedInTheDatabaseCall()
            {
                await PublishNotification();

                savedLocation.TpiMetaData.Inputs.Should().Be(existingLocationInDatabase.TpiMetaData.Inputs);
            }

            [Fact]
            public async Task ThenInputsShouldNotBeChangedInTheQueueCall()
            {
                await PublishNotification();
                
                enqueuedLocation.TpiMetaData.Inputs.Should().Be(existingLocationInDatabase.TpiMetaData.Inputs);
            }
        }
        
        public class AndTheOutputsAreNotPresent : WhenReceivingAPartialTpiLocationUpdateNotification
        {
            protected override Notification Notification => base.Notification with
            {
                TpiMetaData = base.Notification.TpiMetaData with { Outputs = null }
            };


            [Fact]
            public async Task ThenOutputsShouldNotBeChangedInTheDatabaseCall()
            {
                await PublishNotification();

                savedLocation.TpiMetaData.Outputs.Should().Be(existingLocationInDatabase.TpiMetaData.Outputs);
            }

            [Fact]
            public async Task ThenOutputsShouldNotBeChangedInTheQueueCall()
            {
                await PublishNotification();
                
                enqueuedLocation.TpiMetaData.Outputs.Should().Be(existingLocationInDatabase.TpiMetaData.Outputs);
            }
        }
        
        public class AndFixIsNotPresent : WhenReceivingAPartialTpiLocationUpdateNotification
        {
            protected override Notification Notification => base.Notification with
            {
                TpiMetaData = base.Notification.TpiMetaData with { Fix = null }
            };


            [Fact]
            public async Task ThenFixShouldNotBeChangedInTheDatabaseCall()
            {
                await PublishNotification();

                savedLocation.TpiMetaData.Fix.Should().Be(existingLocationInDatabase.TpiMetaData.Fix);
            }

            [Fact]
            public async Task ThenFixShouldNotBeChangedInTheQueueCall()
            {
                await PublishNotification();
                
                enqueuedLocation.TpiMetaData.Fix.Should().Be(existingLocationInDatabase.TpiMetaData.Fix);
            }
        }
        
        public class AndGeoDataIsNotPresent : WhenReceivingAPartialTpiLocationUpdateNotification
        {
            protected override Notification Notification => base.Notification with
            {
                TpiMetaData = base.Notification.TpiMetaData with { GeoData = null }
            };


            [Fact]
            public async Task ThenGeoDataShouldNotBeChangedInTheDatabaseCall()
            {
                await PublishNotification();

                savedLocation.TpiMetaData.GeoData.Should().Be(existingLocationInDatabase.TpiMetaData.GeoData);
            }

            [Fact]
            public async Task ThenGeoDataShouldNotBeChangedInTheQueueCall()
            {
                await PublishNotification();
                
                enqueuedLocation.TpiMetaData.GeoData.Should().Be(existingLocationInDatabase.TpiMetaData.GeoData);
            }
        }
    }
}