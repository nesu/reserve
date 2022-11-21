using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Attributes;
using Reserve.API.Requests.Management.Accounts;
using Reserve.Data.Types;

namespace Reserve.API.Controllers
{
    [Authorize, ApiController]
    [Route("api/management/accounts")]
    public class AccountManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet("{account_id}")]
        [RoleRequirement(RoleType.Employee, RoleType.Administrator)]
        public async Task<IActionResult> GetAccount(
            long account_id)
        {
            var request = new GetAccount.RequestEnvelope { Id = account_id }; 
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost]
        [RoleRequirement(RoleType.Employee, RoleType.Administrator)]
        public async Task<IActionResult> GetAccountCollection(
            GetAccountCollection.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("create-account")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> CreateAccount(
            CreateAccount.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpPost("save-account")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> SaveAccount(
            SaveAccount.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}