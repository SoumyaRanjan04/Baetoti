using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Shared;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
	public interface IBuyerReviewRepository : IAsyncRepository<BuyerReview>
	{
		Task<PaginationResponse> GetReviews(RatingAndReviewRequest request, long? UserID);

	}
}
