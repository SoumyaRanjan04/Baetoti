using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.Order;
using System.Threading.Tasks;
using System.Linq;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Baetoti.Shared.Request.Order;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.Store;
using Baetoti.Shared.Helper;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class OrderComparer : IEqualityComparer<OrderStates>
    {
        #region IEqualityComparer<Post> Members

        public bool Equals(OrderStates x, OrderStates y)
        {
            return x.OrderID.Equals(y.OrderID);
        }

        public int GetHashCode(OrderStates obj)
        {
            return obj.OrderID.GetHashCode();
        }

        #endregion
    }
    public class OrderItemRepository : EFRepository<OrderItem>, IOrderItemRepository
    {

        private readonly BaetotiDbContext _dbContext;
        private readonly IConfiguration _config;

        public OrderItemRepository(
            BaetotiDbContext dbContext,
            IConfiguration config) : base(dbContext)
        {
            _dbContext = dbContext;
            _config = config;
        }

        public async Task<OrderResponse> GetAll(DateRangeFilter DateRange)
        {
            var orderResponse = await (from c in _dbContext.Orders
                                       select new OrderResponse
                                       {
                                           Completed = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.Completed && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate)),
                                           Pending = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.Pending && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate)),
                                           Approved = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.Approved && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate)),
                                           Rejected = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.Rejected && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate)),
                                           InProgress = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.InProgress && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate)),
                                           Ready = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.Ready && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate)),
                                           PickedUp = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.PickedUp && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate)),
                                           Delivered = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.Delivered && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate)),
                                           CancelledByCustomer = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.CancelledByCustomer && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate)),
                                           CancelledByProvider = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.CancelledByProvider && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate)),
                                           CancelledByDriver = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.CancelledByDriver && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate)),
                                           Complaints = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.Complaint && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate)),
                                           CancelledBySystem = _dbContext.Orders.Count(o => o.Status == (int)OrderStatus.CancelledBySystem && (DateRange.StartDate == null || o.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || o.CreatedAt <= DateRange.EndDate))
                                       }).FirstOrDefaultAsync();

            orderResponse.AverageRating = _dbContext.ItemReviews.Average(x => x.Rating);

            var providerOrderStates = new ProviderOrderStates();
            providerOrderStates.Approved = _dbContext.Orders.Count(p => p.Status == (int)OrderStatus.Approved && (DateRange.StartDate == null || p.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || p.CreatedAt <= DateRange.EndDate));
            providerOrderStates.Rejected = _dbContext.Orders.Count(p => p.Status == (int)OrderStatus.Rejected && (DateRange.StartDate == null || p.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || p.CreatedAt <= DateRange.EndDate));
            providerOrderStates.Canceled = _dbContext.Orders.Count(p => p.Status == (int)OrderStatus.CancelledByProvider && (DateRange.StartDate == null || p.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || p.CreatedAt <= DateRange.EndDate));
            orderResponse.ProviderOrder = providerOrderStates;

            var driverOrderStates = new DriverOrderStates();
            driverOrderStates.Pending = _dbContext.Orders.Count(d => d.Status == (int)OrderStatus.Ready && (DateRange.StartDate == null || d.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || d.CreatedAt <= DateRange.EndDate));
            driverOrderStates.Delivered = _dbContext.Orders.Count(d => d.Status == (int)OrderStatus.Delivered && (DateRange.StartDate == null || d.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || d.CreatedAt <= DateRange.EndDate));
            driverOrderStates.PickedUp = _dbContext.Orders.Count(d => d.Status == (int)OrderStatus.PickedUp && (DateRange.StartDate == null || d.CreatedAt >= DateRange.StartDate) && (DateRange.EndDate == null || d.CreatedAt <= DateRange.EndDate));
            orderResponse.DriverOrder = driverOrderStates;

            orderResponse.orderGraph = _dbContext.Orders
                                                .Where(order => order.CreatedAt >= DateRange.StartDate && order.CreatedAt <= DateRange.EndDate)
                                                .GroupBy(order => order.CreatedAt.Value.Date)
                                                .Select(group => new OrderGraph
                                                {
                                                    Name = group.Key.ToString("MMM dd"),
                                                    Orders = group.Count()
                                                })
                                                .ToList();
            return orderResponse;
        }

        public async Task<PaginationResponse> GetFilteredData(OrderFilterRequest request)
        {

            PaginationResponse response = new PaginationResponse();
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("Default")))
            {
                var param = new DynamicParameters();
                param.Add("@StartDate", request.DateRange.StartDate);
                param.Add("@EndDate", request.DateRange.EndDate);
                param.Add("@OrderStatus", request.OrderStatus);
                param.Add("@CategoryID", request.CategoryID);
                param.Add("@SubCategoryID", request.SubCategoryID);
                param.Add("@CountryID", request.CountryID);
                param.Add("@RegionID", request.RegionID);
                param.Add("@CityID", request.CityID);
                param.Add("@Gender", request.Gender);
                param.Add("@PageNumber", request.PageNumber);
                param.Add("@PageSize", request.PageSize);

                using (var m = db.QueryMultiple("[dbo].[baetoti.GetFilterData]", param, commandType: CommandType.StoredProcedure))
                {
                    var data = m.Read<OrderStates>().ToList();
                    var totalRecords = await m.ReadFirstOrDefaultAsync<int>();
                    var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

                    response.CurrentPage = request.PageNumber;
                    response.TotalPages = totalPages;
                    response.PageSize = request.PageSize;
                    response.TotalCount = totalRecords;
                    response.Data = data;
                }
            }
            return response;

        }

        public async Task<OrderByIDResponse> GetByID(long ID)
        {
            var response = new OrderByIDResponse();
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("Default")))
            {
                var param = new DynamicParameters();
                param.Add("@OrderID", ID);
                using (var m = db.QueryMultiple("[baetoti].[GetOrderByID]", param, commandType: CommandType.StoredProcedure))
                {
                    var orderDetail = m.ReadFirstOrDefault<OrderDetail>();
                    response.orderDetail = orderDetail;

                    var customerDetail = m.ReadFirstOrDefault<CustomerDetail>();
                    response.customerDetail = customerDetail;

                    var providerDetail = m.ReadFirstOrDefault<ProviderDetail>();
                    response.providerDetail = providerDetail;

                    var driverDetail = m.ReadFirstOrDefault<DriverDetail>();
                    response.driverDetail = driverDetail;

                    var paymentInfo = m.ReadFirstOrDefault<PaymentInfo>();
                    response.paymentInfo = paymentInfo;

                    var orderStatus = m.ReadFirstOrDefault<OrderStatusResponse>();
                    response.orderStatus = orderStatus;

                    var itemsList = m.Read<ItemList>().ToList();
                    response.itemsList = itemsList;

                    var reviews = m.Read<Reviews>().ToList();
                    response.reviews = reviews;

                    var costSummary = m.ReadFirstOrDefault<CostSummary>();
                    response.costSummary = costSummary;
                }
            }

            response.MapData = new List<MapData>
            {
                new MapData{ Latitude = response.customerDetail.Latitude, Longitude = response.customerDetail.Longitude},
                new MapData{ Latitude = response.providerDetail.Latitude, Longitude = response.providerDetail.Longitude},

            };

            if (response.driverDetail != null)
            {
                response.MapData.Add(new MapData { Latitude = response.driverDetail.Latitude, Longitude = response.driverDetail.Longitude });
            }
            else
            {
                new MapData { Latitude = response.driverDetail != null ? response.driverDetail.Latitude : 0, Longitude = response.driverDetail != null ? response.driverDetail.Longitude : 0 };
            }

            response.twoLineGraph.Date = new string[]
            {
                $"{response.orderDetail.CreateDate} {response.orderDetail.CreatedTime}",
                $"{response.orderDetail.OrderReadyDate} {response.orderDetail.OrderReadyTime}",
                $"{response.orderDetail.OrderPickUpDate} {response.orderDetail.OrderPickUpTime}",
                $"{response.orderDetail.ScheduleDate} {response.orderDetail.ScheduleTime}",
                $"{response.orderDetail.ActualDate} {response.orderDetail.ActualTime}"
            };
            response.twoLineGraph.Status = new string[]
            {
                "Placed",
                "Ready",
                "Pickup",
                "Expected Delivery",
                "Actual Delivery",
            };

            var invoice = await _dbContext.Invoices.Where(inv => inv.OrderId == ID).FirstAsync();
            if (invoice != null)
            {
                response.invoiceID = invoice.QRCode;
            }
            return response;
        }

        public async Task<PaginationResponse> GetAllByUserID(long UserID, OrderStatusSearchRequest request)
        {
            DriverConfig config = await _dbContext.DriverConfigs.Where(c => !c.MarkAsDeleted).FirstOrDefaultAsync();
            decimal commissionAmount = 0;
            if (config != null)
            {
                commissionAmount = config.DriverComission;
            }

            var list = from o in _dbContext.Orders
                       join u in _dbContext.Users on o.UserID equals u.ID
                       join pr in _dbContext.ProviderOrders on o.ID equals pr.OrderID
                       into providerOrderTemp
                       from pot in providerOrderTemp.DefaultIfEmpty()
                       join p in _dbContext.Providers on pot.ProviderID equals p.ID into providerTemp
                       from pt in providerTemp.DefaultIfEmpty()
                       join pu in _dbContext.Users on pt.UserID equals pu.ID
                       into providerUserTemp
                       from put in providerUserTemp.DefaultIfEmpty()
                       join dor in _dbContext.DriverOrders.Where(x => x.Status != 4 && x.Status != 9) on o.ID equals dor.OrderID
                       into driverOrderTemp
                       from dot in driverOrderTemp.DefaultIfEmpty()
                       join d in _dbContext.Drivers on dot.DriverID equals d.ID into driverTemp
                       from dt in driverTemp.DefaultIfEmpty()
                       join du in _dbContext.Users on dt.UserID equals du.ID
                       into driverUserTemp
                       from dut in driverUserTemp.DefaultIfEmpty()
                       join t in _dbContext.Transactions on o.ID equals t.OrderID
                       join s in _dbContext.Stores on pot.ProviderID equals s.ProviderID
                       where request.OrderStatus == o.Status &&
                       (request.UserType == (int)UserType.Buyer ? u.ID == UserID : request.UserType == (int)UserType.Provider ? put.ID == UserID : dut.ID == UserID)
                       && u.MarkAsDeleted == false && pt.MarkAsDeleted == false
                       select new UserOrderStates
                       {
                           OrderID = o.ID,
                           UserID = u.ID,
                           DriverID = dut == null ? 0 : dut.ID,
                           ProviderID = put == null ? 0 : put.ID,
                           Buyer = $"{u.FirstName} {u.LastName}",
                           Provider = put == null ? "" : $"{put.FirstName} {put.LastName}",
                           Driver = dut == null ? "" : $"{dut.FirstName} {dut.LastName}",
                           OrderAmount = _dbContext.OrderItems.Where(x => x.ItemID == o.ID).Sum(x => x.Quantity),
                           PaymentType = ((TransactionType)t.TransactionType).ToString(),
                           Date = o.CreatedAt,
                           NotesForDriver = o.NotesForDriver,
                           ProviderComments = o.ProviderComments,
                           DriverComments = o.DriverComments,
                           ExpectedDeliveryTime = o.ExpectedDeliveryTime,
                           DeliverOrPickup = o.IsPickBySelf ? "PickUp" : "Delivery",
                           IsPickBySelf = o.IsPickBySelf,
                           OrderAddress = o.DeliveryAddress,
                           OrderAddressLatitude = o.AddressLatitude,
                           OrderAddressLongitude = o.AddressLongitude,
                           OrderStatus = ((OrderStatus)o.Status).ToString(),
                           OrderStatusKey = o.Status,
                           DeliveryCharges = t.DeliveryCharges,
                           TotalDriverCharges = commissionAmount == 0 ? t.DeliveryCharges : t.DeliveryCharges - (commissionAmount / 100 * t.DeliveryCharges),
                           ServiceFees = t.ServiceFee,
                           DeliveryDate = o.ExpectedDeliveryTime,
                           OrderPrice = t.Amount,
                           Distance = Helper.GetDistance(o.AddressLatitude, o.AddressLongitude, s.Latitude, s.Longitude),
                           TotalItemsCount = (from oi in _dbContext.OrderItems
                                              where oi.OrderID == o.ID
                                              select oi.ID).Count(),
                           orderItems = (from oi in _dbContext.OrderItems
                                         join i in _dbContext.Items on oi.ItemID equals i.ID
                                         join c in _dbContext.Categories on i.CategoryID equals c.ID
                                         where oi.OrderID == o.ID
                                         select new OrderItemsResponse
                                         {
                                             ItemID = oi.ItemID,
                                             ItemName = i.Name,
                                             ItemComments = oi.Comments,
                                             ItemImages = (from im in _dbContext.ItemImages
                                                           where im.ItemID == i.ID
                                                           select new ImageResponse
                                                           {
                                                               Image = im.Image
                                                           }).ToList(),
                                             Price = i.Price,
                                             BaetotiPrice = i.BaetotiPrice,
                                             Quantity = oi.Quantity,
                                             CategoryID = c.ID,
                                             Category = c.CategoryName,
                                             CategoryArabic = c.CategoryArabicName
                                         }).ToList(),
                           buyerDetail = new BuyerDetail
                           {
                               ID = o.UserID,
                               Name = $"{u.FirstName} {u.LastName}",
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               Picture = u.Picture
                           },
                           store = (from s in _dbContext.Stores
                                    join p in _dbContext.Providers on s.ProviderID equals p.ID
                                    join po in _dbContext.ProviderOrders on p.ID equals po.ProviderID
                                    where po.OrderID == o.ID
                                    select new StoreResponse
                                    {
                                        ID = s.ID,
                                        ProviderID = s.ProviderID,
                                        Name = s.Name,
                                        Description = s.Description,
                                        Location = s.Location,
                                        Longitude = s.Longitude,
                                        Latitude = s.Latitude,
                                        IsAddressHidden = s.IsAddressHidden,
                                        VideoURL = s.VideoURL,
                                        Images = (from si in _dbContext.StoreImages
                                                  where si.StoreID == s.ID
                                                  select new ImageResponse
                                                  {
                                                      Image = si.Image
                                                  }).ToList(),
                                        BusinessLogo = s.BusinessLogo,
                                        InstagramGallery = s.InstagramGallery,
                                        Status = s.Status,
                                    }).FirstOrDefault()
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
                                    x.Provider.ToLower().Contains(request.SearchValue.ToLower()) ||
                                    x.Driver.ToLower().Contains(request.SearchValue.ToLower()) ||
                                    x.Buyer.ToLower().Contains(request.SearchValue.ToLower()) ||
                                    x.store.Name.ToLower().Contains(request.SearchValue.ToLower())
                                )
                                .OrderByDescending(x => request.SortOrder == 2 ? x.Date : x.DeliveryDate)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .Select(ss => ss.OrderID).Distinct()
                                .ToList();
            else
                response.Data = await list.OrderByDescending(x => request.SortOrder == 2 ? x.Date : x.DeliveryDate)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize).ToListAsync();

            return response;
        }

        public async Task<OrderByUserIDResponse> GetUserOrderByID(long OrderID)
        {
            var storeResponse = await (from s in _dbContext.Stores
                                       join p in _dbContext.Providers on s.ProviderID equals p.ID
                                       join po in _dbContext.ProviderOrders on p.ID equals po.ProviderID
                                       where po.OrderID == OrderID
                                       select new StoreResponse
                                       {
                                           ID = s.ID,
                                           ProviderID = s.ProviderID,
                                           Name = s.Name,
                                           Description = s.Description,
                                           Location = s.Location,
                                           Longitude = s.Longitude,
                                           Latitude = s.Latitude,
                                           IsAddressHidden = s.IsAddressHidden,
                                           VideoURL = s.VideoURL,
                                           Images = (from si in _dbContext.StoreImages
                                                     where si.StoreID == s.ID
                                                     select new ImageResponse
                                                     {
                                                         Image = si.Image
                                                     }).ToList(),
                                           BusinessLogo = s.BusinessLogo,
                                           InstagramGallery = s.InstagramGallery,
                                           Status = s.Status,
                                       }).FirstOrDefaultAsync();

            DriverConfig config = await _dbContext.DriverConfigs.Where(c => !c.MarkAsDeleted).FirstOrDefaultAsync();
            decimal commissionAmount = 0;
            if (config != null)
            {
                commissionAmount = config.DriverComission;
            }

            return await (from o in _dbContext.Orders.Where(x => x.ID == OrderID)
                          join u in _dbContext.Users on o.UserID equals u.ID
                          join dor in _dbContext.DriverOrders.Where(x => x.Status != 4 && x.Status != 9) on o.ID equals dor.OrderID
                          into driverOrderTemp
                          from dot in driverOrderTemp.DefaultIfEmpty()
                          join por in _dbContext.ProviderOrders on o.ID equals por.OrderID
                          join p in _dbContext.Providers on por.ProviderID equals p.ID
                          join pu in _dbContext.Users on p.UserID equals pu.ID
                          join fd in _dbContext.FavouriteDrivers on new { dot.DriverID, por.ProviderID } equals new { fd.DriverID, fd.ProviderID } into tempFavouriteDriver
                          from tfd in tempFavouriteDriver.DefaultIfEmpty()
                          join d in _dbContext.Drivers on dot.DriverID equals d.ID
                          into driverTemp
                          from drt in driverTemp.DefaultIfEmpty()
                          join du in _dbContext.Users on drt.UserID equals du.ID
                          into driverUserTemp
                          from dut in driverUserTemp.DefaultIfEmpty()
                          join t in _dbContext.Transactions on o.ID equals t.OrderID
                          join br in _dbContext.BuyerReviews.Where(b => b.DriverID != 0L) on o.ID equals br.OrderID into buyerdriverReviewTemp
                          from brdt in buyerdriverReviewTemp.DefaultIfEmpty()
                          join br in _dbContext.BuyerReviews.Where(b => b.ProviderID != 0L) on o.ID equals br.OrderID into buyerproviderReviewTemp
                          from brpt in buyerproviderReviewTemp.DefaultIfEmpty()
                          join dr in _dbContext.DriverReviews.Where(b => b.UserID != 0L) on o.ID equals dr.OrderID into driverbuyerrReviewTemp
                          from drbt in driverbuyerrReviewTemp.DefaultIfEmpty()
                          join dr in _dbContext.DriverReviews.Where(b => b.ProviderID != 0L) on o.ID equals dr.OrderID into driverproviderReviewTemp
                          from drpt in driverproviderReviewTemp.DefaultIfEmpty()
                          join sr in _dbContext.StoreReviews.Where(b => b.UserID != 0L) on o.ID equals sr.OrderID into buyerstoreReviewTemp
                          from srbt in buyerstoreReviewTemp.DefaultIfEmpty()
                          join sr in _dbContext.StoreReviews.Where(b => b.DriverID != 0L) on o.ID equals sr.OrderID into driverstoreReviewTemp
                          from srdt in driverstoreReviewTemp.DefaultIfEmpty()
                          join orr in _dbContext.OrderRequests.Where(r => r.RequestStatus != 3 && r.RequestStatus != 5) on o.ID equals orr.OrderID into orderRequestTemp
                          from ort in orderRequestTemp.DefaultIfEmpty()
                          join ul in _dbContext.UserLocations.Where(x => x.IsLive == true) on drt.UserID equals ul.UserID
                          into driverLocationTemp
                          from dlt in driverLocationTemp.DefaultIfEmpty()
                          select new OrderByUserIDResponse
                          {
                              OrderID = o.ID,
                              DeliverOrPickup = o.IsPickBySelf ? "Pickup" : "Delivery",
                              NotesForDriver = o.NotesForDriver,
                              ProviderComments = o.ProviderComments,
                              DriverComments = o.DriverComments,
                              DeliveryAddress = o.DeliveryAddress,
                              AddressLatitude = o.AddressLatitude,
                              AddressLongitude = o.AddressLongitude,
                              CreatedAt = o.CreatedAt,
                              ExpectedDeliveryTime = o.ExpectedDeliveryTime,
                              ActualDeliveryTime = o.ActualDeliveryTime,
                              OrderPickupTime = o.OrderPickUpTime,
                              OrderReadyTime = o.OrderReadyTime,
                              IsPickBySelf = o.IsPickBySelf,
                              OrderStatus = ((OrderStatus)o.Status).ToString(),
                              OrderRequestStatus = ort == null ? 0 : ort.RequestStatus,
                              TotalCharges = t.Amount,
                              DeliveryCharges = t.DeliveryCharges,
                              ServiceFees = t.ServiceFee,
                              TotalDriverCharges = commissionAmount == 0 ? t.DeliveryCharges : t.DeliveryCharges - (commissionAmount / 100 * t.DeliveryCharges),
                              Distance = Helper.GetDistance(o.AddressLatitude, o.AddressLongitude, storeResponse.Latitude, storeResponse.Longitude),
                              IsBuyerRatedByDriver = brdt != null,
                              IsBuyerRatedByProvider = brpt != null,
                              IsDriverRatedByBuyer = drbt != null,
                              IsDriverRatedByProvider = drpt != null,
                              IsProviderRatedByBuyer = srbt != null,
                              IsProviderRatedByDriver = srdt != null,
                              IsFavouriteDriver = tfd != null,
                              buyer = new Shared.Response.User.BuyerResponse
                              {
                                  UserID = o.UserID,
                                  Name = $"{u.FirstName} {u.LastName}",
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  Description = u.Description,
                                  Picture = u.Picture,
                                  //City = u.City,
                                  //State = u.State,
                                  Address = u.Address,
                                  //PostalCode = u.Zip,
                                  //Country = u.Country,
                                  Latitude = u.Latitude,
                                  Longitude = u.Longitude,
                                  //Status = u.State,
                                  Phone = u.Phone,
                                  Email = u.Email
                              },
                              provider = new Shared.Response.User.ProviderResponse
                              {
                                  ProviderID = p.ID,
                                  UserID = p.UserID,
                                  Name = $"{pu.FirstName} {pu.LastName}",
                                  FirstName = pu.FirstName,
                                  LastName = pu.LastName,
                                  MaroofID = p.MaroofID,
                                  GovernmentID = p.GovernmentID,
                                  ExpirationDate = Convert.ToString(p.ExpirationDate),
                                  GovernmentIDPicture = p.GovernmentIDPicture,
                                  Picture = pu.Picture,
                                  Status = ((UserStatus)pu.UserStatus).ToString(),
                                  Phone = pu.Phone,
                                  Email = pu.Email,
                                  Instagram = "",
                                  Location = pu.DefaultLocationTitle,
                                  IsPublic = p.IsPublic,
                                  IsOnline = p.IsOnline,
                              },
                              driver = new Shared.Response.User.DriverResponse
                              {
                                  Name = $"{dut.FirstName} {dut.LastName}",
                                  FirstName = dut.FirstName,
                                  LastName = dut.LastName,
                                  Description = dut.Description,
                                  Picture = dut.Picture,
                                  //Status = dut.UserStatus,
                                  Phone = dut.Phone,
                                  Email = dut.Email,
                                  Gender = dut.Gender,
                                  DriverID = Convert.ToString(drt.ID),
                                  UserID = dut == null ? 0 : dut.ID,
                                  DrivingLiscence = drt.ExpirayDateofLicense,
                                  VehicleAuthorization = drt.ExpiryDateofVehicleAuthorization,
                                  MedicalCheckup = drt.ExpiryDateofMedicalcheckup,
                                  VehicleInsurance = drt.ExpiryDateofVehicleInsurance,
                                  DrivingLiscensePic = drt.DrivingLicensePic,
                                  VehicleRegistrationPic = drt.VehicleRegistrationPic,
                                  VehicleAuthorizationPic = drt.VehicleAuthorizationPic,
                                  MedicalReportPic = drt.MedicalCheckupPic,
                                  VehicleInsurancePic = drt.VehicleInsurancePic,
                                  LiveLatitude = dlt == null ? 0 : dlt.Latitude,
                                  LiveLongitude = dlt == null ? 0 : dlt.Longitude,
                              },
                              store = storeResponse,
                              orderItems = (from oi in _dbContext.OrderItems
                                            join i in _dbContext.Items on oi.ItemID equals i.ID
                                            join c in _dbContext.Categories on i.CategoryID equals c.ID
                                            where oi.OrderID == o.ID
                                            select new OrderItemsResponse
                                            {
                                                ItemID = oi.ItemID,
                                                ItemName = i.Name,
                                                ItemComments = oi.Comments,
                                                ItemImages = (from im in _dbContext.ItemImages
                                                              where im.ItemID == i.ID
                                                              select new ImageResponse
                                                              {
                                                                  Image = im.Image
                                                              }).ToList(),
                                                Price = i.Price,
                                                BaetotiPrice = i.BaetotiPrice,
                                                Quantity = oi.Quantity,
                                                CategoryID = c.ID,
                                                Category = c.CategoryName,
                                                CategoryArabic = c.CategoryArabicName
                                            }).ToList()
                          }).FirstOrDefaultAsync();
        }

        public async Task<PaginationResponse> TrackOrderByUserID(long UserID, TrackOrderRequest request)
        {
            DriverConfig config = await _dbContext.DriverConfigs.Where(C => !C.MarkAsDeleted).FirstOrDefaultAsync();
            decimal commissionAmount = 0;
            if (config != null)
            {
                commissionAmount = config.DriverComission;
            }
            var list = from o in _dbContext.Orders
                       join u in _dbContext.Users on o.UserID equals u.ID
                       join por in _dbContext.ProviderOrders on o.ID equals por.OrderID
                       join p in _dbContext.Providers on por.ProviderID equals p.ID
                       join pu in _dbContext.Users on p.UserID equals pu.ID
                       join dor in _dbContext.DriverOrders.Where(x => x.Status != 4 && x.Status != 9) on o.ID equals dor.OrderID
                       into driverOrderTemp
                       from dot in driverOrderTemp.DefaultIfEmpty()
                       join d in _dbContext.Drivers on dot.DriverID equals d.ID into tempDriver
                       from d in tempDriver.DefaultIfEmpty()
                       join du in _dbContext.Users on d.UserID equals du.ID
                       into driverUserTemp
                       from dut in driverUserTemp.DefaultIfEmpty()
                       join t in _dbContext.Transactions on o.ID equals t.OrderID
                       join s in _dbContext.Stores on por.ProviderID equals s.ProviderID
                       where (request.UserType == (int)UserType.Buyer ? u.ID == UserID : request.UserType == (int)UserType.Provider ? pu.ID == UserID : dut.ID == UserID)
                       select new UserOrderStates
                       {
                           OrderID = o.ID,
                           UserID = u.ID,
                           DriverID = dut == null ? 0 : dut.ID,
                           ProviderID = p.ID,
                           Buyer = $"{u.FirstName} {u.LastName}",
                           Provider = $"{pu.FirstName} {pu.LastName}",
                           Driver = dut == null ? "" : $"{dut.FirstName} {dut.LastName}",
                           OrderAmount = _dbContext.OrderItems.Where(x => x.ItemID == o.ID).Sum(x => x.Quantity),
                           PaymentType = ((TransactionType)t.TransactionType).ToString(),
                           Date = o.CreatedAt,
                           ExpectedDeliveryTime = o.ExpectedDeliveryTime,
                           DeliverOrPickup = o.IsPickBySelf ? "PickUp" : "Delivery",
                           IsPickBySelf = o.IsPickBySelf,
                           NotesForDriver = o.NotesForDriver,
                           ProviderComments = o.ProviderComments,
                           DriverComments = o.DriverComments,
                           OrderAddress = o.DeliveryAddress,
                           OrderAddressLatitude = o.AddressLatitude,
                           OrderAddressLongitude = o.AddressLongitude,
                           OrderStatus = ((OrderStatus)o.Status).ToString(),
                           OrderStatusKey = o.Status,
                           store = (from s in _dbContext.Stores
                                    join p in _dbContext.Providers on s.ProviderID equals p.ID
                                    join po in _dbContext.ProviderOrders on p.ID equals po.ProviderID
                                    where po.OrderID == o.ID
                                    select new StoreResponse
                                    {
                                        ID = s.ID,
                                        ProviderID = s.ProviderID,
                                        Name = s.Name,
                                        Description = s.Description,
                                        Location = s.Location,
                                        Longitude = s.Longitude,
                                        Latitude = s.Latitude,
                                        IsAddressHidden = s.IsAddressHidden,
                                        VideoURL = s.VideoURL,
                                        Images = (from si in _dbContext.StoreImages
                                                  where si.StoreID == s.ID
                                                  select new ImageResponse
                                                  {
                                                      Image = si.Image
                                                  }).ToList(),
                                        BusinessLogo = s.BusinessLogo,
                                        InstagramGallery = s.InstagramGallery,
                                        Status = s.Status,
                                    }).FirstOrDefault(),
                           DeliveryDate = o.ExpectedDeliveryTime,
                           OrderPrice = t.Amount,
                           DeliveryCharges = t.DeliveryCharges,
                           TotalDriverCharges = commissionAmount == 0 ? t.DeliveryCharges : t.DeliveryCharges - (commissionAmount / 100 * t.DeliveryCharges),
                           TotalItemsCount = (from oi in _dbContext.OrderItems
                                              where oi.OrderID == o.ID
                                              select oi.ID).Count(),
                           orderItems = (from oi in _dbContext.OrderItems
                                         join i in _dbContext.Items on oi.ItemID equals i.ID
                                         join c in _dbContext.Categories on i.CategoryID equals c.ID
                                         where oi.OrderID == o.ID
                                         select new OrderItemsResponse
                                         {
                                             ItemID = oi.ItemID,
                                             ItemName = i.Name,
                                             ItemComments = oi.Comments,
                                             ItemImages = (from im in _dbContext.ItemImages
                                                           where im.ItemID == i.ID
                                                           select new ImageResponse
                                                           {
                                                               Image = im.Image
                                                           }).ToList(),
                                             Price = i.Price,
                                             BaetotiPrice = i.BaetotiPrice,
                                             Quantity = oi.Quantity,
                                             CategoryID = c.ID,
                                             Category = c.CategoryName,
                                             CategoryArabic = c.CategoryArabicName
                                         }).ToList(),
                           buyerDetail = new BuyerDetail
                           {
                               ID = o.UserID,
                               Name = $"{u.FirstName} {u.LastName}",
                               FirstName = u.FirstName,
                               LastName = u.LastName,
                               Picture = u.Picture
                           },
                       };

            int[] statusArray;
            if (request.TrackOrderStatus == (int)TrackOrderStatus.Active)
            {
                statusArray = new int[] { 1, 3, 4, 5 };
                list = list.Where(x => statusArray.Contains(x.OrderStatusKey));
            }
            else if (request.TrackOrderStatus == (int)TrackOrderStatus.Completed)
            {
                statusArray = new int[] { 11 };
                list = list.Where(x => statusArray.Contains(x.OrderStatusKey));
            }
            else if (request.TrackOrderStatus == (int)TrackOrderStatus.Cancelled)
            {
                statusArray = new int[] { 8, 9, 10 };
                list = list.Where(x => statusArray.Contains(x.OrderStatusKey));
            }
            else if (request.TrackOrderStatus == (int)TrackOrderStatus.WaitingForRating)
            {
                statusArray = new int[] { 11, 6 };
                list = list.Where(x => statusArray.Contains(x.OrderStatusKey));
                if (request.UserType == (int)UserType.Buyer)
                    list = list.Where(x => !_dbContext.StoreReviews.Any(y => y.UserID == x.UserID && y.OrderID == x.OrderID))
                                .Where(x => !_dbContext.DriverReviews.Any(y => y.UserID == x.UserID && y.OrderID == x.OrderID));
                else if (request.UserType == (int)UserType.Provider)
                    list = list.Where(x => !_dbContext.DriverReviews.Any(y => y.ProviderID == x.ProviderID && y.OrderID == x.OrderID))
                                .Where(x => !_dbContext.BuyerReviews.Any(y => y.ProviderID == x.ProviderID && y.OrderID == x.OrderID));
                else if (request.UserType == (int)UserType.Driver)
                    list = list.Where(x => !_dbContext.BuyerReviews.Any(y => y.DriverID == x.DriverID && y.OrderID == x.OrderID));
            }

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
                                    x.Provider.ToLower().Contains(request.SearchValue.ToLower()) ||
                                    x.Driver.ToLower().Contains(request.SearchValue.ToLower()) ||
                                    x.Buyer.ToLower().Contains(request.SearchValue.ToLower()) ||
                                    x.store.Name.ToLower().Contains(request.SearchValue.ToLower())
                                )
                                .OrderByDescending(x => request.SortOrder == 2 ? x.Date : x.DeliveryDate)
                                .Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize)
                                .ToList();
            else
                response.Data = await list.OrderByDescending(x => request.SortOrder == 2 ? x.Date : x.DeliveryDate)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize).ToListAsync();

            return response;
        }

    }
}
