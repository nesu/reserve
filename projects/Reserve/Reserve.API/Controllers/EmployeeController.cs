using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Requests.Common;

namespace Reserve.API.Controllers
{
    [AllowAnonymous]
    [ApiController, Route("/api/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{location_id}")]
        public async Task<IActionResult> GetCompactEmployeeCollection(
            long location_id)
        {
            var request = new GetCompactEmployeeCollection.RequestEnvelope
            {
                LocationId = location_id
            };
                
            var response = await _mediator.Send(request);
            
            return Ok(response);
        }
        
        [HttpGet("services/{employee_id}")]
        public async Task<IActionResult> GetServiceCollectionByEmployee(
            long employee_id)
        {
            var request = new GetServiceCollectionByEmployee.RequestEnvelope
            {
                EmployeeId = employee_id
            };

            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("availability")]
        public async Task<IActionResult> GetEmployeeAvailability(
            GetAvailableTimespanCollection.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}