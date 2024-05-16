using System.Linq;
using System.Security.Claims;

namespace Baetoti.Shared.Extentions
{
    public static class IdentityExtensions
    {
        public static string GetUserId(this ClaimsPrincipal claimPrincipal)
        {
            if (!claimPrincipal.Identity.IsAuthenticated)
            {
                return null;
            }
            return claimPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultNameClaimType).Value;
        }

    }
}
