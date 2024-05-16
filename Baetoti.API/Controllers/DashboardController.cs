using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Shared.Request.Dashboard;

namespace Baetoti.API.Controllers
{
    public class DashboardController : ApiBaseController
    {

        public readonly IDashboardRepository _dashboardRepository;

        public DashboardController(
            IDashboardRepository dashboardRepository
            )
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpPost("GetData")]
        public async Task<IActionResult> GetData([FromBody] DashboardRequest request)
        {
            try
            {
                var dashboardData = await _dashboardRepository.GetDashboardDataAsync(request);
                return Ok(new SharedResponse(true, 200, "", dashboardData));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
