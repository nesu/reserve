using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Requests.Common;

namespace Reserve.API.Controllers
{
    [ApiController, Route("api/experimental")]
    public class ExperimentalController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExperimentalController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("mail")]
        public async Task<IActionResult> SendMailRequest()
        {
            await _mediator.Publish(new SendMail.NotificationEnvelope());
            return Ok();
        }
    }
}