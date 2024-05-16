using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Promotion;
using Baetoti.Shared.Response.Promotion;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
	public interface IPromotionRepository : IAsyncRepository<Promotion>
	{
		Task<GetPromotionResponse> GetAll(GetPromotionRequest getPromotionRequest);

		Task<ViewPromotionResponse> View(long PromotionID);

	}

}
