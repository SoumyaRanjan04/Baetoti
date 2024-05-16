using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Baetoti.API.Controllers.Base
{
	[EnableCors("Trusted")]
	[Produces("application/json")]
	[Route("api/[controller]")]
	[Authorize]
	[ApiController]
	public class ApiBaseController : ControllerBase
	{
		protected string Identity
		{
			get
			{
				return User.Claims.Where(c => c.Type == ClaimsIdentity.DefaultNameClaimType)
						  .Select(c => c.Value).SingleOrDefault();
			}
		}

		protected string UserId
		{
			
			get
			{
				var claim = User.Claims.Where(c => c.Type == ClaimsIdentity.DefaultNameClaimType).ToList();
				if (claim.Count > 0)
				{
					return User.Claims.Where(c => c.Type == ClaimsIdentity.DefaultNameClaimType)
						.FirstOrDefault().Value;
                }
                else
                {
					return "";
                }
			}
		}

		protected string FirstName
		{
			get
			{
				return User.Claims.Where(c => c.Type == ClaimTypes.GivenName)
					.FirstOrDefault()?.Value;
			}
		}

        protected string UserRole
        {
            get
            {
                return User.Claims.Where(c => c.Type == ClaimTypes.Role)
                    .FirstOrDefault()?.Value;
            }
        }

        public readonly SiteConfig _siteConfig;

		public ApiBaseController(ISiteConfigService siteConfigService = null)
		{
			if (siteConfigService != null)
				_siteConfig = siteConfigService.GetSiteConfigs();
		}

	}
}
