using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Helper;
using Baetoti.Shared.Request.FavouriteDriver;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Driver;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class FavouriteDriverRepository : EFRepository<FavouriteDriver>, IFavouriteDriverRepository
	{

		private readonly BaetotiDbContext _dbContext;

		public FavouriteDriverRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<FavouriteDriver> CheckIfExists(FavouriteDriverRequest request)
		{
			return await _dbContext.FavouriteDrivers.Where(x => x.DriverID == request.DriverID &&
			(request.UserID == 0 || x.UserID == request.UserID) &&
			(request.ProviderID == 0 || x.ProviderID == request.ProviderID)).FirstOrDefaultAsync();
		}

		public async Task<List<GetAllDriverResponse>> GetFavouriteDriver(GetFavouriteDriverRequest request)
		{
			Store store = await (from s in _dbContext.Stores
								 join p in _dbContext.Providers on s.ProviderID equals p.ID
								 where p.UserID == request.UserID
								 select s).FirstOrDefaultAsync();

			var res = await (from d in _dbContext.Drivers
							 join u in _dbContext.Users on d.UserID equals u.ID
							 join fd in _dbContext.FavouriteDrivers.Where(x => (request.UserID == 0 || x.UserID == request.UserID) &&
							 (request.ProviderID == 0 || x.ProviderID == request.ProviderID)) on d.ID equals fd.DriverID
							 where (d.DriverStatus == 1 || d.DriverStatus == 3) &&
							 d.IsPublic == true && d.IsOnline == true && d.IsAcceptJob == true
							 select new GetAllDriverResponse
							 {
								 ID = d.ID,
								 Name = $"{u.FirstName} {u.LastName}",
								 Location = u.Address,
								 Picture = u.Picture,
								 Latitude = u.Latitude,
								 Longitude = u.Longitude,
								 Distance = store == null ? 0 : Helper.GetDistance(store.Latitude, store.Longitude, u.Latitude, u.Longitude)
							 }).ToListAsync();
			return res;
		}

	}
}
