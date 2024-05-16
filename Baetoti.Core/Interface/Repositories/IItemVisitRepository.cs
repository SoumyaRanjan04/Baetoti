using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IItemVisitRepository : IAsyncRepository<ItemVisit>
    {
        Task<ItemVisit> LogItemVisit(ItemVisit itemVisit);

    }
}
