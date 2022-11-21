using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Reserve.API.Extensions
{
    public static class IdentityExtensions
    {
        public static string GetIdentifyingValue(this IIdentity identity)
        {
            Claim subject = (identity as ClaimsIdentity)?.FindFirst(ClaimTypes.Email);
            if (subject == null)
                throw new InvalidOperationException($"Missing \"{ClaimTypes.Email}\" claim.");

            return subject.Value;
        }
    }
}