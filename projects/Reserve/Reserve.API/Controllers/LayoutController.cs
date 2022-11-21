using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Attributes;
using Reserve.API.Requests.Account;
using Reserve.API.Requests.Common;
using Reserve.API.Requests.Layout;
using Reserve.API.Requests.Management.Layout;
using Reserve.Data.Types;

namespace Reserve.API.Controllers
{
    [AllowAnonymous]
    [ApiController, Route("api/layout")]
    public class LayoutController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LayoutController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("visuals")]
        public async Task<IActionResult> GetVisuals()
        {
            var request = new GetVisualElements.RequestEnvelope();
            var response = await _mediator.Send(request);

            return Ok(response);
        }

        [HttpGet("page/{page_identifier}")]
        public async Task<IActionResult> GetPage(string page_identifier)
        {
            var request = new GetPageByIdentifier.RequestEnvelope
            {
                Identifier = page_identifier
            };

            var response = await _mediator.Send(request);
            
            return Ok(response);
        }

        [HttpGet("pages")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> GetPages()
        {
            var request = new GetPageCollection.RequestEnvelope();
            var response = await _mediator.Send(request);

            return Ok(response);
        }
        
        // @TODO: ROLE GUARDS
        [HttpPost("edit-page")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> EditPage(
            EditPage.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        // @TODO: ROLE GUARDS
        [HttpPost("create-page")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> CreatePage(
            CreatePage.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        // @TODO: ROLE GUARDS
        [HttpPost("delete-page")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> DeletePage(
            DeletePage.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        // @TODO: ROLE GUARDS
        [HttpGet("navigator-element/{identifier}")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> GetNavigatorElementCollection(long identifier)
        {
            var request = new GetNavigatorElementById.RequestEnvelope
            {
                Id = identifier
            };
            
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        // @TODO: ROLE GUARDS
        [HttpGet("navigator-elements")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> GetNavigatorElementCollection()
        {
            var request = new GetNavigatorElementCollection.RequestEnvelope();
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        // @TODO: ROLE GUARDS
        [HttpPost("navigator-element")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> GetNavigatorElementCollection(
            CreateNavigatorElement.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        // @TODO: ROLE GUARDS
        [HttpPost("edit-navigator-element")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> EditNavigatorElement(
            EditNavigatorElement.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        // @TODO: ROLE GUARDS
        [HttpPost("delete-navigator-element")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> DeleteNavigatorElement(
            DeleteNavigatorElement.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
    }
}