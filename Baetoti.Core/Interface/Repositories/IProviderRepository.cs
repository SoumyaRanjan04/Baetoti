using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Provider;
using Baetoti.Shared.Response.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IProviderRepository : IAsyncRepository<Provider>
    {
        Task<Provider> GetByUserID(long UserID);

        Task<ProviderResponse> GetDetailByUserID(long UserID);

        Task<ProviderResponse> GetDetailByProviderID(long ProviderID);

        Task<ProviderAndBusinessResponse> GetProviderBusinessDetailByUserID(long UserID);

        Task<List<GetAllProviderResponse>> GetAllAsync();

        Task<PaginationResponse> GetAllOnlineAsync(int PageNumber, int PageSize);

        Task<ProviderDashboardResponse> GetDashboardData(long UserID);

        Task<GraphDataResponse> GetSalesGraphData(int SaleGraphFilter, long UserID);

        Task<EarnGraphDataResponse> GetEarnGraphData(int EarnGraphFilter, long UserID);

        Task<Provider> GetByGovtID(string GovtID);

        Task<List<ProviderUserlatlngResponse>> GetUserlatlng(double lat, double lng);
    }
}
