using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Shared.Request.InstragramToken;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Extentions;

namespace Baetoti.API.Controllers
{
    public class InstagramController : ApiBaseController
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInstagramAPIService _instagramAPIService;
        private readonly IInstragramTokenRepository _instragramTokenRepository;

        public InstagramController(
            ISiteConfigService siteConfigService,
            IHttpContextAccessor httpContextAccessor,
            IInstagramAPIService instagramAPIService,
            IInstragramTokenRepository instragramTokenRepository
            ) : base(siteConfigService)
        {
            _httpContextAccessor = httpContextAccessor;
            _instagramAPIService = instagramAPIService;
            _instragramTokenRepository = instragramTokenRepository;
        }

        [HttpPost("SaveToken")]
        public async Task<IActionResult> SaveToken([FromBody] InstragramTokenRequest request)
        {
            try
            {
                InstragramToken token = new InstragramToken
                {
                    StoreID = request.StoreID,
                    Token = request.Token,
                    LastTimeTokenUpdated = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                };
                var response = await _instragramTokenRepository.AddAsync(token);
                return Ok(new SharedResponse(true, 200, "Token Saved Successfully", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] InstragramTokenRequest request)
        {
            try
            {
                var response = await _instagramAPIService.Refresh(request, _httpContextAccessor.HttpContext, _siteConfig);
                InstragramToken existingToken = await _instragramTokenRepository.GetByStoreID(request.StoreID);
                existingToken.Token = response.Token;
                existingToken.LastTimeTokenUpdated = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                await _instragramTokenRepository.UpdateAsync(existingToken);
                return Ok(new SharedResponse(true, 200, "Token Refreshed Successfully", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

    }
}
