using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IDriverRepository : IAsyncRepository<Driver>
    {
        Task<GetDriverByIDResponse> GetByUserID(long UserID);

        Task<Driver> GetDriverByUserID(long UserID);

        Task<List<GetAllDriverResponse>> GetAllAsync();

        Task<List<GetAllDriverResponse>> GetNearBy(long UserID, double LocationRange);

        Task<EarnGraphDataResponse> GetEarnGraphData(int EarnGraphFilter, long UserID);

        Task<Driver> GetByGovtID(string GovtID);

    }
}
