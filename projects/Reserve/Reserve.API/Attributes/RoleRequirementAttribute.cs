using System;
using Microsoft.AspNetCore.Mvc;
using Reserve.API.Filters;
using Reserve.Data.Types;

namespace Reserve.API.Attributes
{
    public class RoleRequirementAttribute : TypeFilterAttribute
    {
        public RoleRequirementAttribute(params RoleType[] role_types) : base(typeof(RoleRequirementFilter))
        {
            Arguments = new object[] { role_types };
        }
    }
}