using Baetoti.Core.Entites;
using Baetoti.Shared.Response.Auth;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Services
{
	public interface IUserJwtService
	{
		Task<UserAuthResponse> GenerateTokenAsync(User user);

		ClaimsIdentity GenerateClaimsIdentity(User user, IList<string> roles);

		ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

		string GenerateRefreshToken();
	}
}
