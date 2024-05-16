using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Helper;
using Baetoti.Shared.Request.OrderRequest;
using Baetoti.Shared.Response.OrderRequest;
using Baetoti.Shared.Response.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class OrderRequestRepository : EFRepository<OrderRequest>, IOrderRequestRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public OrderRequestRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OrderRequest> GetByOrderID(long OrderID)
        {
            return await _dbContext.OrderRequests.Where(x => x.OrderID == OrderID && x.RequestStatus != 3 && x.RequestStatus != 5).FirstOrDefaultAsync();
        }

        public async Task<PaginationResponse> GetOrderRequest(GetOrderRequest request)
        {
            DriverConfig config = await _dbContext.DriverConfigs.Where(c => !c.MarkAsDeleted).FirstOrDefaultAsync();
            decimal commissionAmount = 0;
            if (config != null)
            {
                commissionAmount = config.DriverComission;
            }

            var list = from or in _dbContext.OrderRequests
                       join o in _dbContext.Orders on or.OrderID equals o.ID
                       join po in _dbContext.ProviderOrders on or.OrderID equals po.OrderID
                       join s in _dbContext.Stores on po.ProviderID equals s.ProviderID
                       join t in _dbContext.Transactions on or.OrderID equals t.OrderID
                       where or.RequestStatus == request.OrderRequestStatus && or.DriverID == request.DriverID
                       select new OrderRequestResponse
                       {
                           OrderID = or.OrderID,
                           DeliveryDate = o.ExpectedDeliveryTime,
                           StoreAddress = s.Location,
                           StoreLatitude = s.Latitude,
                           StoreLongitude = s.Longitude,
                           OrderAddress = o.DeliveryAddress,
                           OrderAddressLatitude = o.AddressLatitude,
                           OrderAddressLongitude = o.AddressLongitude,
                           DeliveryCharges = t.DeliveryCharges,
                           TotalDriverCharges = commissionAmount == 0 ? t.DeliveryCharges : t.DeliveryCharges - (commissionAmount / 100 * t.DeliveryCharges),
                           ServiceFee = t.ServiceFee,
                           OrderStatus = or.RequestStatus,
                           Distance = Helper.GetDistance(o.AddressLatitude, o.AddressLongitude, s.Latitude, s.Longitude)
                       };

            var totalRecords = list.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            PaginationResponse response = new PaginationResponse
            {
                CurrentPage = request.PageNumber,
                TotalPages = totalPages,
                PageSize = request.PageSize,
                TotalCount = totalRecords
            };

            if (!string.IsNullOrEmpty(request.SearchValue))
                response.Data = (await list.ToListAsync())
                                .Where(x =>
                                    x.StoreAddress.ToLower().Contains(request.SearchValue.ToLower()) ||
                                    x.OrderAddress.ToLower().Contains(request.SearchValue.ToLower())
                                )
                                .OrderByDescending(x => x.DeliveryDate)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToList();
            else
                response.Data = await list.OrderByDescending(x => x.DeliveryDate)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize).ToListAsync();

            return response;
        }

    }
}
