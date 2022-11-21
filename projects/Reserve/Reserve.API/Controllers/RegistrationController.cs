using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Reserve.API.Requests.Account;

namespace Reserve.API.Controllers
{
    [ApiController, Route("api/registration")]
    public class RegistrationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RegistrationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register.RequestEnvelope request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}