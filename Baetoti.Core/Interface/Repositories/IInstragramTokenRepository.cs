using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IInstragramTokenRepository : IAsyncRepository<InstragramToken>
    {
        Task<InstragramToken> GetByStoreID(long StoreID);

    }
}
