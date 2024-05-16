using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IProviderBusinessRepository : IAsyncRepository<ProviderBusiness>
    {
         Task<ProviderBusiness> GetByProviderID(long ProviderID);

    }
   
}
