using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Reserve.Data.Types;

namespace Reserve.API.Filters
{
    public class RoleRequirementFilter : IAuthorizationFilter
    {
        private readonly RoleType[] _role_types;

        public RoleRequirementFilter(params RoleType[] role_types)
        {
            _role_types = role_types;
        }
        
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var role_literal = context.HttpContext
                .User.Claims.FirstOrDefault(it => it.Type == ClaimTypes.Role);

            if (role_literal != null)
            {
                var role = (RoleType) Enum.Parse(typeof(RoleType), role_literal.Value);
                if (_role_types.Contains(role))
                    return;
            }

            context.Result = new ForbidResult();
        }
    }
}