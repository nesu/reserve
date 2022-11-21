using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Extensions;
using Reserve.API.Requests.Account;
using Reserve.API.Requests.Common;

namespace Reserve.API.Controllers
{
    [Authorize]
    [ApiController, Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAccount()
        {
            var request = new GetAuthenticated.RequestEnvelope();
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("personal-information")]
        public async Task<IActionResult> SavePersonalInformation(
            SavePersonalInformation.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(
            ChangePassword.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("create-reservation")]
        public async Task<IActionResult> CreateReservation(
            CreateReservation.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpPost("reschedule-reservation")]
        public async Task<IActionResult> RescheduleReservation(
            RescheduleReservation.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpPost("cancel-reservation")]
        public async Task<IActionResult> CancelReservation(
            CancelReservation.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpGet("reservations")]
        public async Task<IActionResult> GetReservations()
        {
            var request = new GetReservationCollection.RequestEnvelope();
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpGet("reservation/{reservation_id}")]
        public async Task<IActionResult> GetReservation(
            long reservation_id)
        {
            var request = new GetReservation.RequestEnvelope
            {
                Id = reservation_id
            };
            
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}