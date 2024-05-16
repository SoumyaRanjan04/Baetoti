using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.Fence;
using Baetoti.Shared.Response.OperationalConfig;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IFenceRepository : IAsyncRepository<Fence>
    {
        Task<List<OperationalConfigResponse>> GetAllFences();

        Task<OperationalConfigResponseByID> GetFenceByID(long ID);

        Task<List<FenceResponse>> GetFenceList();

        Task<FenceResponse> GetFenceByCityID(string CityID);

    }
}
