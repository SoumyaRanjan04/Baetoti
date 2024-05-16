using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface ICityRepository : IAsyncRepository<City>
    {
        Task<List<City>> GetAll(string RegionID);

    }
}
