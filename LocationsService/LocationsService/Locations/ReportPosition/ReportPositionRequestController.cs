namespace Com.Apdcomms.DataGateway.LocationsService.ReportPosition
{
    using System.Threading.Tasks;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class ReportPositionRequestController : ControllerBase
    {
        private readonly IMediator mediator;

        public ReportPositionRequestController(
            IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ReportPositionRequest request)
        {
            await mediator.Send(request);
            return Ok();
        }
    }
}