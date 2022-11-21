using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Requests.Account;

namespace Reserve.API.Controllers
{
    [ApiController, Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(Authenticate.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}