using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.FavouriteDriver;
using Baetoti.Shared.Response.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
	public interface IFavouriteDriverRepository : IAsyncRepository<FavouriteDriver>
	{
		Task<FavouriteDriver> CheckIfExists(FavouriteDriverRequest request);

		Task<List<GetAllDriverResponse>> GetFavouriteDriver(GetFavouriteDriverRequest request);

	}
}
