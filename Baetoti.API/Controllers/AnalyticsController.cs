using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Shared.Request.Analytic;
using Baetoti.Shared.Request.Shared;

namespace Baetoti.API.Controllers
{
    public class AnalyticsController : ApiBaseController
    {

        private readonly IMapper _mapper;
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(
            IMapper mapper,
            IAnalyticsService analyticsService
            )
        {
            _mapper = mapper;
            _analyticsService = analyticsService;
        }

        [HttpPost("GetMapData")]
        public async Task<IActionResult> GetMapData(FilterRequest filter)
        {
            try
            {
                var response = await _analyticsService.GetMapData(filter);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetMarkovData")]
        public async Task<IActionResult> GetMarkovData(RequestID filter)
        {
            try
            {
                var response = await _analyticsService.GetMarkovData(filter);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetStatisticData")]
        public async Task<IActionResult> GetStatisticData(FilterRequest filter)
        {
            try
            {
                var response = await _analyticsService.GetStatisticData(filter);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetRevenuPerDayData")]
        public async Task<IActionResult> GetRevenuPerDayData(RevenuePerDayRequest filter)
        {
            try
            {
                var response = await _analyticsService.GetRevenuPerDayData(filter);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetFinanceData")]
        public async Task<IActionResult> GetFinanceData(FilterRequest filter)
        {
            try
            {
                var response = await _analyticsService.GetFinanceData(filter);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetCohortData")]
        public async Task<IActionResult> GetCohortData(CohortFilter filter)
        {
            try
            {
                var response = await _analyticsService.GetCohortData(filter);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
