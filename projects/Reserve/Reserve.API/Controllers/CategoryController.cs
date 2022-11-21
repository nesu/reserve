using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Requests.Common;

namespace Reserve.API.Controllers
{
    [AllowAnonymous]
    [ApiController, Route("/api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet("root")]
        public async Task<IActionResult> GetParentCategoryCollection()
        {
            var request = new GetParentCategoryCollection.RequestEnvelope();
            var response = await _mediator.Send(request);

            return Ok(response);
        }
    }
}