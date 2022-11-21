using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Requests.Management.Locations;

namespace Reserve.API.Controllers
{
    [AllowAnonymous]
    [ApiController, Route("/api/locations")]
    public class LocationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LocationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetLocationCollection()
        {
            var request = new GetLocationCollection.RequestEnvelope();
            var response = await _mediator.Send(request);

            return Ok(response);
        }
    }
}