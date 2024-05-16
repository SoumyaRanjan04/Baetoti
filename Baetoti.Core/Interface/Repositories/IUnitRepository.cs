using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.Unit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IUnitRepository : IAsyncRepository<Unit>
    {
        Task<List<UnitResponse>> GetAllUnitsAsync();
        Task<UnitResponse> GetUnitByID(int ID);
    }
}
