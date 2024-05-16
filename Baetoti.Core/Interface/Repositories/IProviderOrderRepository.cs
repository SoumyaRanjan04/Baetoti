using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
	public interface IProviderOrderRepository : IAsyncRepository<ProviderOrder>
	{
		Task<ProviderOrder> GetByOrderID(long OrderID);

		Task<Provider> GetProviderByOrderID(long OrderID);

		Task<Provider> GetProviderByItemID(long ItemID);

	}
}
