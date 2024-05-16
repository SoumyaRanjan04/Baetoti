using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Dashboard;
using Baetoti.Shared.Request.SupportRequest;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.SupportRequest;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface ISupportRequestRepository : IAsyncRepository<SupportRequest>
    {
        Task<SupportRequestByIDResponse> GetById(long ID, long UserID, string UserRole);

        Task<PaginationResponse> GetAll(SupportRequestFilter request);

        Task<DashboardResponse> GetDashboardData(DashboardRequest request);

    }
}
