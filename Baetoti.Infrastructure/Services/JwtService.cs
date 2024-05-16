using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.JwtConfig;
using Baetoti.Shared.Response.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly SymmetricSecurityKey _signingKey;
        private readonly IEmployeeRepository _userRepository;

        public JwtService(IOptions<JwtIssuerOptions> jwtOptions,
            IConfiguration configuration,
            IEmployeeRepository userRepository)
        {
            _jwtOptions = jwtOptions?.Value;
            ThrowIfInvalidOptions(_jwtOptions);
            var key = configuration?.GetSection("JwtConfiguration")["Key"];
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            _userRepository = userRepository;
        }

        /// <summary>
        /// Generates JWT token for user
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="roles">User roles</param>
        /// <returns></returns>
        public async Task<AuthResponse> GenerateTokenAsync(Employee user)
        {
            var issuedAt = _jwtOptions.IssuedAt.ToUnixTimeSeconds();
            var expiresAt = _jwtOptions.Expiration.ToUnixTimeSeconds();
            var notBeforeAt = _jwtOptions.NotBefore.ToUnixTimeSeconds();
            var roles = await _userRepository.GetRolesAsync(user);
            var validityClaims = await GenerateValidityClaims(issuedAt, expiresAt, notBeforeAt, user.ID);
            var claimsIdentity = AddValidityClaimsToIdentity(GenerateClaimsIdentity(user, roles), validityClaims);
            var jwtSecurityToken = new JwtSecurityToken
                (
                    issuer: _jwtOptions.Issuer,
                    audience: _jwtOptions.Audience,
                    claims: claimsIdentity.Claims,
                    notBefore: _jwtOptions.NotBefore,
                    expires: _jwtOptions.Expiration,
                    signingCredentials: _jwtOptions.SigningCredentials
                );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            user.RefreshToken = GenerateRefreshToken();
            await _userRepository.UpdateAsync(user);
            return new AuthResponse()
            {
                AccessToken = accessToken,
                RefreshToken = user.RefreshToken,
                ExpiresAt = expiresAt.ToString(),
                UserInformation = new EmployeeInformation
                {
                    UserID = user.ID,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = user.Address,
                    Comments = user.Comments,
                    DOB = user.DOB,
                    Gender = user.Gender,
                    Email = user.Email,
                    Location = user.Location,
                    Phone = user.Phone,
                    Username = user.Username
                }
            };
        }

        /// <summary>
        /// Generates user claims identity
        /// </summary>
        /// <param name="user">Application user</param>
        /// <param name="roles">User roles</param>
        /// <returns>ClaimsIdentity</returns>
        public ClaimsIdentity GenerateClaimsIdentity(Employee user, IList<string> roles)
        {
            var claimsId = new ClaimsIdentity(new GenericIdentity(user?.ID.ToString(), "Token")/*, new[]
            {
                new Claim(JwtCustomClaimNames.Id, user.Id.ToString()),
            }*/);

            foreach (var role in roles)
            {
                claimsId.AddClaim(new Claim(ClaimTypes.Role, role));
            }
            switch (roles.FirstOrDefault())
            {
                case "Admin":
                    claimsId.AddClaim(new Claim(JwtCustomClaimNames.Role, "Admin"));
                    break;
                case "Client":
                    claimsId.AddClaim(new Claim(JwtCustomClaimNames.Role, "Client"));
                    break;
                default:
                    break;
            }
            return claimsId;
        }

        /// <summary>
        /// Parse JWT token and return claim principal
        /// </summary>
        /// <param name="token">JWT access token</param>
        /// <returns>ClaimsPrincipal</returns>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true, //you might want to validate the audience and issuer depending on your use case
                ValidAudience = _jwtOptions.Audience,
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                RequireExpirationTime = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

        /// <summary>
        /// Generates Refresh Token
        /// </summary>
        /// <returns>string</returns>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <summary>
        ///Accepts (issue at, expired at, not before at) Unix time stamp and returns validity claims 
        /// </summary>
        /// <param name="issuedAt">Unix time stamp</param>
        /// <param name="expiresAt">Unix time stamp</param>
        /// <param name="notBeforeAt">Unix time stamp</param>
        /// <returns>Claim[]</returns>
        private async Task<Claim[]> GenerateValidityClaims(long issuedAt, long expiresAt, long notBeforeAt, long userId)
        {
            var validityClaims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, await (_jwtOptions?.JtiGenerator()).ConfigureAwait(true)),
                new Claim(JwtRegisteredClaimNames.Iat, issuedAt.ToString(), ClaimValueTypes.Integer64),
                new Claim(JwtRegisteredClaimNames.Exp, expiresAt.ToString()),
                new Claim(JwtRegisteredClaimNames.Nbf, notBeforeAt.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString())
            };
            return validityClaims;
        }

        /// <summary>
        /// Add validity claims to claim identity
        /// </summary>
        /// <param name="identity">Claim identity</param>
        /// <param name="validityClaims">Validity claims</param>
        /// <returns>ClaimsIdentity</returns>
        private ClaimsIdentity AddValidityClaimsToIdentity(ClaimsIdentity identity, Claim[] validityClaims)
        {
            if (identity == null)
            {
                throw new ArgumentNullException(nameof(identity));
            }
            if (validityClaims == null)
            {
                throw new ArgumentNullException(nameof(validityClaims));
            }

            identity.AddClaims(validityClaims);
            return identity;
        }

        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}
