using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.Cart;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
	public interface ICartRepository : IAsyncRepository<Cart>
	{
		Task<Cart> GetByItemID(long ItemID, long UserID);

		Task<List<Cart>> GetByUserID(long UserID);

		Task<GetCartResponse> GetCartDetail(long UserID);

		Task<bool> CheckForStore(long ItemID, long UserID);

	}
}
