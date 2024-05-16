using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.ReportedOrder;
using Baetoti.Shared.Response.ReportedOrder;
using Baetoti.Shared.Response.Shared;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
	public interface IReportedOrderRepository : IAsyncRepository<ReportedOrder>
	{
		Task<ReportedOrder> CheckIfExists(ReportedOrderRequest request);

		Task<PaginationResponse> GetAll(GetReportedOrderRequest request);

		Task<ReportedOrderByIDResponse> GetReportedOrderByID(long ID);

	}
}
