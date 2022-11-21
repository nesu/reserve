using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Attributes;
using Reserve.API.Requests.Common;
using Reserve.Data.Types;

namespace Reserve.API.Controllers
{
    [RoleRequirement(RoleType.Administrator, RoleType.Client, RoleType.Employee)]
    [ApiController, Route("api/reservations")]
    public class ReservationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReservationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        
    }
}