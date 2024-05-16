using Baetoti.Core.Entites;
using Baetoti.Shared.Response.Auth;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Services
{
    public interface IJwtService
    {
        Task<AuthResponse> GenerateTokenAsync(Employee user);

        ClaimsIdentity GenerateClaimsIdentity(Employee user, IList<string> roles);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        string GenerateRefreshToken();

    }
}
