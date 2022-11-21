using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Attributes;
using Reserve.API.Requests.Management.Categories;
using Reserve.Data.Types;

namespace Reserve.API.Controllers
{
    [Authorize, ApiController]
    [Route("api/management/categories")]
    public class CategoryManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Returns collection of categories, that have no children.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [RoleRequirement(RoleType.Administrator, RoleType.Employee)]
        public async Task<IActionResult> GetBaseCategories()
        {
            var request = new GetBaseCategoryCollection.RequestEnvelope();
            var response = await _mediator.Send(request);
            
            return Ok(response);
        }
        
        [HttpGet("nested")]
        [RoleRequirement(RoleType.Administrator, RoleType.Employee)]
        public async Task<IActionResult> GetCategories()
        {
            var request = new GetCategoryCollection.RequestEnvelope();
            var response = await _mediator.Send(request);
            
            return Ok(response);
        }

        [HttpGet("{category_id}")]
        [RoleRequirement(RoleType.Administrator, RoleType.Employee)]
        public async Task<IActionResult> GetCategoryById(long category_id)
        {
            var request = new GetCategoryById.RequestEnvelope(category_id);
            var response = await _mediator.Send(request);

            return Ok(response);
        }

        [HttpPost("create-category")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> CreateCategory(
            CreateCategory.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }
        
        [HttpPost("save-category")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> SaveCategory(
            SaveCategory.RequestEnvelope request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("delete-category")]
        [RoleRequirement(RoleType.Administrator)]
        public async Task<IActionResult> DeleteCategory(
            DeleteCategory.RequestEnvelope request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}