using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Store;
using Baetoti.Shared.Response.Store;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Baetoti.Core.Interface.Repositories
{
    public interface IStoreAddressRepository : IAsyncRepository<StoreAddress>
    {
         Task<List<StoreAddress>> GetAllByStoreId(long id);
    }
}
