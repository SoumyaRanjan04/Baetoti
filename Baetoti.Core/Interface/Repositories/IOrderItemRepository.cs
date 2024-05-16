using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Order;
using Baetoti.Shared.Response.Order;
using Baetoti.Shared.Response.Shared;
using System;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IOrderItemRepository : IAsyncRepository<OrderItem>
    {
        Task<OrderResponse> GetAll(DateRangeFilter DateRange);
        Task<PaginationResponse> GetFilteredData(OrderFilterRequest orderFilterRequest);
        Task<OrderByIDResponse> GetByID(long ID);
        Task<OrderByUserIDResponse> GetUserOrderByID(long OrderID);
        Task<PaginationResponse> GetAllByUserID(long UserID, OrderStatusSearchRequest request);
        Task<PaginationResponse> TrackOrderByUserID(long UserID, TrackOrderRequest request);
    }
}
