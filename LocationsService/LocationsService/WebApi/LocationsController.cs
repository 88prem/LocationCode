namespace Com.Apdcomms.DataGateway.LocationsService.WebApi
{
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class LocationsController : Controller
    {
        private readonly IMediator mediator;

        public LocationsController(IMediator mediator)
        {
            this.mediator = mediator;
        } 
        
		[HttpGet]
        public async Task<ActionResult> GetAllLocations()
        {
            return this.Ok(await this.mediator.Send(new GetLocationsRequest()));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult> GetLocation([FromRoute] string id)
        {
            var result = await this.mediator.Send(new GetLocationRequest { LocationId = id });

            if (result is null)
            {
                return this.NotFound($"Location not found");
            }

            return this.Ok(result);
        }
    }
}