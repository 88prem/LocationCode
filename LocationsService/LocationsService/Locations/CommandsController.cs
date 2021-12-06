using Microsoft.AspNetCore.Mvc;

namespace Com.Apdcomms.DataGateway.LocationsService
{
    using System.Threading.Tasks;
    using Com.Apdcomms.DataGateway.LocationsService.Refresh;
    using MediatR;

    [ApiController]
    [Route("api/[controller]/[Action]")]
    public class CommandsController : Controller
    {
        private readonly IMediator mediator;

        public CommandsController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        
        [HttpPost]
		[ActionName("Refresh")]
        public async Task<ActionResult> Post([FromBody] LocationsRefreshRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }
    }
}