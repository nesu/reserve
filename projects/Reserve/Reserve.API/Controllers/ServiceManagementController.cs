using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Attributes;
using Reserve.API.Requests.Management;
using Reserve.API.Requests.Management.Services;
using Reserve.Data.Types;

namespace Reserve.API.Controllers
{
    [ApiController]
    [Route("api/management/services")]
    [Authorize]
    public class ServiceManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ServiceManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [RoleRequirement(RoleType.Administrator, RoleType.Employee)]
        public async Task<IActionResult> GetServiceCollection()
        {
            var request = new GetServiceCollection.RequestEnvelope();
            var response = await _mediator.Send(request);

            return Ok(response);
        }
        
        [HttpPost("events")]
        [RoleRequirement(RoleType.Administrator, RoleType.Employee)]
        public async Task<IActionResult> GetCalendarEvents(
            GetCalendarEventCollection.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpPost("create-service")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> CreateService(
            CreateService.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpPost("save-service")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> SaveService(
            SaveService.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpPost("delete-service")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> DeleteService(
            DeleteService.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpGet("assignments/{service_id}")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> GetServiceAssignmentCollection(
            long service_id)
        {
            var request = new GetServiceAssignmentCollection.RequestEnvelope(service_id);
            var response = await _mediator.Send(request);

            return Ok(response);
        }
        
        [HttpPost("create-assignment")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> CreateAssignment(
            CreateServiceAssignment.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpPost("delete-assignment")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> DeleteAssignment(
            DeleteServiceAssignment.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpPost("create-reservation")]
        public async Task<IActionResult> CreateReservation(
            CreateRemoteReservation.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}