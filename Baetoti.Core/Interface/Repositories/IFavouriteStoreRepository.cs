using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
	public interface IFavouriteStoreRepository : IAsyncRepository<FavouriteStore>
	{
		Task<FavouriteStore> GetByStoreID(long StoreID, long UserID);

	}
}
