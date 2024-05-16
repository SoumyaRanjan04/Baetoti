using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.OrderRequest;
using Baetoti.Shared.Response.Shared;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
	public interface IOrderRequestRepository : IAsyncRepository<OrderRequest>
	{
		Task<OrderRequest> GetByOrderID(long OrderID);

		Task<PaginationResponse> GetOrderRequest(GetOrderRequest request);

	}
}
