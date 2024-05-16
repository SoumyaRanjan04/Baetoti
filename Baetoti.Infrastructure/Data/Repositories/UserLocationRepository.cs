using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.UserLocation;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class UserLocationRepository : EFRepository<UserLocation>, IUserLocationRepository
	{

		private readonly BaetotiDbContext _dbContext;

		public UserLocationRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<UserLocation> GetDefaultLocation(long UserID)
		{
			return await _dbContext.UserLocations.Where(x => x.UserID == UserID &&
			x.IsDefault == true && x.MarkAsDeleted == false && x.IsLive == false).FirstOrDefaultAsync();
		}

		public async Task<List<GetLocationResponse>> GetAllLocations(long UserID)
		{
			return await _dbContext.UserLocations
				.Where(u => u.UserID == UserID && u.MarkAsDeleted == false && u.IsLive == false)
				.Select(x => new GetLocationResponse
				{
					LocationID = x.ID,
					UserID = x.UserID,
					Latitude = x.Latitude,
					Longitude = x.Longitude,
					Address = x.Address,
					Title = x.Title,
					IsSelected = x.IsDefault
				}).ToListAsync();
		}

		public async Task<UserLocation> CheckLiveLocation(long UserID)
		{
			return await _dbContext.UserLocations.Where(x => x.UserID == UserID &&
													x.IsLive == true).FirstOrDefaultAsync();
		}

		public async Task<GetLocationResponse> GetLiveLocation(long UserID)
		{
			return await _dbContext.UserLocations
				.Where(u => u.UserID == UserID && u.IsLive == true)
				.Select(x => new GetLocationResponse
				{
					LocationID = x.ID,
					UserID = x.UserID,
					Latitude = x.Latitude,
					Longitude = x.Longitude,
					Address = x.Address,
					Title = x.Title,
					IsSelected = false
				}).FirstOrDefaultAsync();
		}
	}
}
