using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class FavouriteStoreRepository : EFRepository<FavouriteStore>, IFavouriteStoreRepository
	{

		private readonly BaetotiDbContext _dbContext;

		public FavouriteStoreRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<FavouriteStore> GetByStoreID(long StoreID, long UserID)
		{
			return await _dbContext.FavouriteStores.Where(x => x.StoreID == StoreID && x.UserID == UserID).FirstOrDefaultAsync();
		}

	}
}
