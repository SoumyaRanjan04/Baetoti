using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Item;
using Baetoti.Shared.Response.Item;
using Baetoti.Shared.Response.Shared;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IItemRepository : IAsyncRepository<Item>
    {
        Task<PaginationResponse> GetFilteredItemsDataAsync(ItemFilterRequest filterRequest, long? UserID);

        Task<PaginationResponse> GetAll(decimal Longitude, decimal Latitude,int ItemFilter, long? UserID, string UserRole, int PageNumber, int PageSize);

        Task<ItemDashboardResponse> GetAllForDashboard(long? UserID, decimal Longitude, decimal Latitude);

        Task<ItemResponseByID> GetByID(long ItemID, long UserID);

        Task<ItemComparisonResponse> ViewByID(long ItemID, long ID);

        Task<ItemsOnBoardingResponse> GetAllItemsOnBoardingRequest(ItemOnBoardingRequest itemOnBoardingRequest);

        Task<List<Item>> GetListofAllItems();


    }
}
