using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Dashboard;
using Baetoti.Shared.Response.Dashboard;
using Baetoti.Shared.Response.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IDashboardRepository
    {
        Task<DashboardResponse> GetDashboardDataAsync(DashboardRequest request);

    }
}
