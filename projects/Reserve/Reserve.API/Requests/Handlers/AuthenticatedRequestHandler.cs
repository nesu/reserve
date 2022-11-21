using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Reserve.API.Extensions;
using Reserve.Data.Types;

namespace Reserve.API.Requests.Handlers
{
    public abstract class AuthenticatedRequestHandler<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        protected readonly RoleType Role;
        protected readonly string SessionIdentifier;

        protected AuthenticatedRequestHandler(IHttpContextAccessor accessor)
        {
            Role = RoleType.Guest;
            
            if (accessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var role = accessor.HttpContext
                    .User.Claims.FirstOrDefault(it => it.Type == ClaimTypes.Role);
                
                if (role == null)
                    throw new UnauthorizedAccessException($"Missing claim with type '{ClaimTypes.Role}'.");

                Role = (RoleType) Enum.Parse(typeof(RoleType), role.Value);
                SessionIdentifier = accessor.HttpContext
                    .User.Identity.GetIdentifyingValue();
            }
        }
        
        public abstract Task<TResponse> Handle(TRequest request, CancellationToken ct);
    }
}