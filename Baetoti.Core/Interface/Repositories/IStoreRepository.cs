using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Store;
using Baetoti.Shared.Response.Store;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IStoreRepository : IAsyncRepository<Store>
    {
        Task<List<StoreResponse>> GetAllByUserId(long id);

        Task<StoreResponse> GetByStoreId(long id, long? UserId);

        Task<List<StoreResponse>> GetNearBy(StoreFilterRequest storeFilterRequest, long? UserID);

        Task<List<StoreResponse>> GetFavouriteStore(long UserID);

        Task<List<StoreTagResponse>> GetStoreTags(long storeID);

        Task<StoreReviewsAndRatingsResponse> GetReviewsAndRatings(StoreReviewAndRatingFilterRequest request);

        Task<List<StoreResponse>> GetOnlineProvidersStore(decimal LocationRange, long? UserID);

        Task<bool> CheckDuplicateURL(StoreRequest request);

        Task<Store> GetByProviderId(long providerId);

    }
}
