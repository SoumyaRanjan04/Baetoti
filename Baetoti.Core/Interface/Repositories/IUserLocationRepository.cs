using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.UserLocation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
	public interface IUserLocationRepository : IAsyncRepository<UserLocation>
	{
		Task<List<GetLocationResponse>> GetAllLocations(long UserID);

		Task<UserLocation> GetDefaultLocation(long UserID);

		Task<UserLocation> CheckLiveLocation(long UserID);

		Task<GetLocationResponse> GetLiveLocation(long UserID);

	}
}
