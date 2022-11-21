using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Attributes;
using Reserve.API.Requests.Management.Locations;
using Reserve.Data.Types;

namespace Reserve.API.Controllers
{
    [Authorize, ApiController]
    [Route("api/management/locations")]
    public class LocationManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LocationManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
        [RoleRequirement(RoleType.Administrator, RoleType.Employee)]
        public async Task<IActionResult> GetLocationCollection()
        {
            var request = new GetLocationCollection.RequestEnvelope();
            var response = await _mediator.Send(request);

            return Ok(response);
        }
        
        [HttpPost("create-location")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> CreateLocation(
            CreateLocation.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpPost("save-location")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> SaveLocation(
            SaveLocation.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpPost("delete-location")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> CreateLocation(
            DeleteLocation.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}