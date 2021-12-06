namespace Com.Apdcomms.LocationsService.Tests
{
    using System;
    using System.Globalization;
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService.Exceptions;
    using Com.Apdcomms.DataGateway.LocationsService.Notifications;
    using Com.Apdcomms.DataGateway.LocationsService.Tpi;
    using FluentAssertions;
    using MediatR;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public abstract class WhenReceivingAnInvalidLocationUpdateNotification : TestBase
    {
        protected virtual Notification Notification { get; } = new()
        {
            Source = "TPI",
            SourceId = "Driver1",
            TpiMetaData = new TpiLocationMetaData
            {
                Bearing = 122.323124,
                Fix = 3,
                Inputs = 12,
                Latitude = -2.12837845,
                Longitude = 32.18561233,
                Outputs = 5,
                Speed = 0.00000152,
                Status = 1,
                Timestamp = (DateTime.Now - TimeSpan.FromMinutes(1)).ToString(CultureInfo.InvariantCulture),
                EventText = "Some event text",
                Resource = "Driver1",
                Class = "Class1",
                Contents = 0xB5,
                Data = "Data over here and over there",
                EventId = 0x01,
                GeoData = 0xF5,
            }
        };

        private Task SendNotification() => SendNotification(Notification);
        
        private async Task SendNotification(Notification notification)
        {
            var mediator = ServiceProvider.GetRequiredService<IMediator>();

            await mediator.Publish(notification);
        }
        
        public class BecauseSourceIdIsNullOrWhiteSpace : WhenReceivingAnInvalidLocationUpdateNotification
        {
            [Theory]
            [InlineData("      ")]
            [InlineData("")]
            [InlineData(null)]
            public async Task ThenAMissingSourceIdExceptionShouldBeThrown(string sourceId)
            {
                await FluentActions.Awaiting(async () => await SendNotification(Notification with { SourceId = sourceId })).Should()
                    .ThrowAsync<MissingSourceIdException>();
            }
        }

        public class BecauseSourceIsNullOrWhiteSpace : WhenReceivingAnInvalidLocationUpdateNotification
        {
            [Theory]
            [InlineData("      ")]
            [InlineData("")]
            [InlineData(null)]
            public async Task ThenAMissingSourceExceptionShouldBeThrown(string source)
            {
                await FluentActions.Awaiting(async () => await SendNotification(Notification with { Source = source })).Should()
                    .ThrowAsync<MissingSourceException>();
            }
        }

        public class BecauseTheMetaDataIsMissing : WhenReceivingAnInvalidLocationUpdateNotification
        {
            [Fact]
            public async Task ThenAMissingMetaDataExceptionIsThrown()
            {
                await FluentActions.Awaiting(async () => await SendNotification(Notification with { TpiMetaData = null })).Should()
                    .ThrowAsync<MissingMetaDataException>();
            }
        }
    }
}