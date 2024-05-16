using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.DriverConfig;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IDriverConfigRepository : IAsyncRepository<DriverConfig>
    {
        Task<List<DriverConfigResponse>> GetAllConfig();

    }
}
