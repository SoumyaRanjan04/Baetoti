using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Request.User;
using Baetoti.Shared.Response.Item;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.User;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GraphData = Baetoti.Shared.Response.User.GraphData;
using User = Baetoti.Core.Entites.User;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class UserRepository : EFRepository<User>, IUserRepository
    {

        private readonly BaetotiDbContext _dbContext;
        private readonly IConfiguration _config;

        public UserRepository(BaetotiDbContext dbContext, IConfiguration config) : base(dbContext)
        {
            _dbContext = dbContext;
            _config = config;
        }

        public async Task<User> GetByMobileNumberAsync(string mobileNumber)
        {
            return await _dbContext.Users.Where(x => x.Phone == mobileNumber && x.MarkAsDeleted == false).FirstOrDefaultAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbContext.Users.Where(x => x.Email.Trim() == email.Trim()).FirstOrDefaultAsync();
        }

        public async Task<OnBoardingResponse> GetOnBoardingDataAsync(OnBoardingRequest onBoardingRequest)
        {
            if (string.IsNullOrEmpty(Convert.ToString(onBoardingRequest.StartDate)))
            {
                onBoardingRequest.StartDate = DateTime.Now.AddMonths(-1);
                onBoardingRequest.EndDate = DateTime.Now;
            }

            // Provider
            var provider = new ProviderOnBoardingRequest();
            var providerState = new UserStates();
            if (onBoardingRequest.Key.ToLower() == "all" || onBoardingRequest.Key.ToLower() == "provider")
            {
                var providers = from p in _dbContext.Providers
                                join u in _dbContext.Users on p.UserID equals u.ID
                                join e in _dbContext.Employee on p.UpdatedByAdminId equals (int?)e.ID into tempEmployee
                                from e in tempEmployee.DefaultIfEmpty()
                                where (onBoardingRequest.StartDate == null || (p.CreatedAt.Value.Date >= onBoardingRequest.StartDate.Value.Date &&
                                p.CreatedAt.Value.Date <= onBoardingRequest.EndDate.Value.Date))
                                select new
                                {
                                    p.UserID,
                                    Name = $"{u.FirstName} {u.LastName}",
                                    u.Email,
                                    u.Phone,
                                    u.Address,
                                    RequestedDate = p.CreatedAt,
                                    RequestCloseDate = p.LastUpdatedAt,
                                    p.ProviderStatus,
                                    CreateBy = e != null ? $"{e.FirstName} {e.LastName}" : "",
                                    EmployeeId = e != null ? e.ID : 0,
                                    Status = ((UserStatus)u.UserStatus).ToString(),
                                    onBoardingStatusID = p.OnBoardingStatus,
                                    OnBoardingStatus = ((OnBoardingStatus)p.OnBoardingStatus).ToString(),
                                    CountryID = p.CountryId,
                                    RegionID = p.RegionId,
                                    CityID = p.CityId,
                                };
                providerState.Pending = providers.Where(x => x.onBoardingStatusID == 0).Count();
                providerState.Approved = providers.Where(x => x.onBoardingStatusID == 1).Count();
                providerState.Rejected = providers.Where(x => x.onBoardingStatusID == 2).Count();

                provider.userStates = providerState;
                provider.pendingUserList = await providers.Where(x => x.onBoardingStatusID == 0).Select(p => new OnBoardingUserList
                {
                    UserID = p.UserID,
                    Name = p.Name,
                    Email = p.Email,
                    MobileNumber = p.Phone,
                    Address = p.Address,
                    RequestDate = p.RequestedDate,
                    RequestCloseDate = p.RequestCloseDate,
                    IsProvider = true,
                    IsDriver = false,
                    IsRequestClosed = false,
                    CreatedBy = p.CreateBy,
                    EmployeeId = p.EmployeeId,
                    Status = p.Status,
                    OnBoardingStatus = p.OnBoardingStatus,
                    CountryID = p.CountryID,
                    RegionID = p.RegionID,
                    CityID = p.CityID,
                    CountryName = _dbContext.Countries.Where(ww => ww.ID == p.CountryID).Select(ss => ss.CountryName).FirstOrDefault().ToString(),
                    RegionName = _dbContext.Regions.Where(ww => ww.Id == p.RegionID).Select(ss => ss.NameAr).FirstOrDefault().ToString(),
                    CityName = _dbContext.Cities.Where(ww => ww.Id == p.CityID).Select(ss => ss.NameAr).FirstOrDefault().ToString(),

                }).OrderByDescending(oo => oo.RequestDate).ToListAsync();

                provider.closeUserList = await providers.Where(x => x.onBoardingStatusID != 0)
                    .Select(p => new OnBoardingUserList
                    {
                        UserID = p.UserID,
                        Name = p.Name,
                        Email = p.Email,
                        MobileNumber = p.Phone,
                        Address = p.Address,
                        RequestDate = p.RequestedDate,
                        RequestCloseDate = p.RequestCloseDate,
                        IsProvider = true,
                        IsDriver = false,
                        IsRequestClosed = true,
                        CreatedBy = p.CreateBy,
                        EmployeeId = p.EmployeeId,
                        Status = p.Status,
                        OnBoardingStatus = p.OnBoardingStatus
                    }).OrderByDescending(oo => oo.RequestCloseDate).ToListAsync();

                provider.graphData = providers.GroupBy(x => x.RequestedDate.Value.Date).Select(p => new GraphData
                {
                    Date = p.Key.ToString("MM/dd/yyyy"),
                    Providers = p.Count(),
                    Drivers = 0
                }).ToList();
            }

            // Driver
            var driver = new DriverOnBoardingRequest();
            var driverState = new UserStates();
            if (onBoardingRequest.Key.ToLower() == "all" || onBoardingRequest.Key.ToLower() == "driver")
            {
                var drivers = from d in _dbContext.Drivers
                              join u in _dbContext.Users on d.UserID equals u.ID
                              join e in _dbContext.Employee on d.UpdatedByAdminId equals (int?)e.ID into tempEmployee
                              from e in tempEmployee.DefaultIfEmpty()
                              where (onBoardingRequest.StartDate == null || (d.CreatedAt.Value.Date >= onBoardingRequest.StartDate.Value.Date &&
                                d.CreatedAt.Value.Date <= onBoardingRequest.EndDate.Value.Date))
                              select new
                              {
                                  d.UserID,
                                  Name = $"{u.FirstName} {u.LastName}",
                                  u.Email,
                                  u.Phone,
                                  u.Address,
                                  RequestedDate = d.CreatedAt,
                                  RequestCloseDate = d.LastUpdatedAt,
                                  d.DriverStatus,
                                  CreateBy = e != null ? $"{e.FirstName} {e.LastName}" : "",
                                  EmployeeId = e != null ? e.ID : 0,
                                  Status = ((UserStatus)u.UserStatus).ToString(),
                                  OnBoardingStatus = ((OnBoardingStatus)d.OnBoardingStatus).ToString(),
                                  OnBoardingStatusID = d.OnBoardingStatus,
                                  CountryID = d.CountryId,
                                  RegionID = d.RegionId,
                                  CityID = d.CityId,
                              };
                driverState.Pending = drivers.Where(x => x.OnBoardingStatusID == 0).Count();
                driverState.Approved = drivers.Where(x => x.OnBoardingStatusID == 1).Count();
                driverState.Rejected = drivers.Where(x => x.OnBoardingStatusID == 2).Count();

                driver.userStates = driverState;
                driver.pendingUserList = await drivers.Where(x => x.OnBoardingStatusID == 0).Select(d => new OnBoardingUserList
                {
                    UserID = d.UserID,
                    Name = d.Name,
                    Email = d.Email,
                    MobileNumber = d.Phone,
                    Address = d.Address,
                    RequestDate = d.RequestedDate,
                    RequestCloseDate = d.RequestCloseDate,
                    IsProvider = false,
                    IsDriver = true,
                    IsRequestClosed = false,
                    CreatedBy = d.CreateBy,
                    EmployeeId = d.EmployeeId,
                    Status = d.Status,
                    OnBoardingStatus = d.OnBoardingStatus,
                    CountryID = d.CountryID,
                    RegionID = d.RegionID,
                    CityID = d.CityID,
                    CountryName = _dbContext.Countries.Where(ww => ww.ID == d.CountryID).Select(ss => ss.CountryName).FirstOrDefault().ToString(),
                    RegionName = _dbContext.Regions.Where(ww => ww.Id == d.RegionID).Select(ss => ss.NameAr).FirstOrDefault().ToString(),
                    CityName = _dbContext.Cities.Where(ww => ww.Id == d.CityID).Select(ss => ss.NameAr).FirstOrDefault().ToString(),

                }).ToListAsync();

                driver.closeUserList = await drivers.Where(x => x.OnBoardingStatusID != 0).Select(d => new OnBoardingUserList
                {
                    UserID = d.UserID,
                    Name = d.Name,
                    Email = d.Email,
                    MobileNumber = d.Phone,
                    Address = d.Address,
                    RequestDate = d.RequestedDate,
                    RequestCloseDate = d.RequestCloseDate,
                    IsProvider = false,
                    IsDriver = true,
                    IsRequestClosed = true,
                    CreatedBy = d.CreateBy,
                    EmployeeId = d.EmployeeId,
                    Status = d.Status,
                    OnBoardingStatus = d.OnBoardingStatus,
                }).ToListAsync();

                driver.graphData = drivers.GroupBy(x => x.RequestedDate.Value.Date).Select(p => new GraphData
                {
                    Date = p.Key.ToString("MM/dd/yyyy"),
                    Providers = 0,
                    Drivers = p.Count()
                }).ToList();
            }

            // Combined
            var combined = new ProviderAndDriverOnBoardingRequest();
            var combinedState = new UserStates();
            combinedState.Pending = providerState.Pending + driverState.Pending;
            combinedState.Approved = providerState.Approved + driverState.Approved;
            combinedState.Rejected = providerState.Rejected + driverState.Rejected;

            combined.userStates = combinedState;

            if (provider.pendingUserList.Count > 0)
                combined.pendingUserList.AddRange(provider.pendingUserList);
            if (driver.pendingUserList.Count > 0)
                combined.pendingUserList.AddRange(driver.pendingUserList);

            if (provider.closeUserList.Count > 0)
                combined.closeUserList.AddRange(provider.closeUserList);
            if (driver.closeUserList.Count > 0)
                combined.closeUserList.AddRange(driver.closeUserList);

            // graphData lists

            var combinedGraphData = (
                from providerData in provider.graphData
                join driverData in driver.graphData
                on providerData.Date equals driverData.Date
                select new GraphData
                {
                    Date = providerData.Date,
                    Providers = providerData.Providers,
                    Drivers = driverData.Drivers
                }
            ).ToList();

            // Add any remaining provider or driver data that doesn't have a match in the other list
            var remainingProviderData = provider.graphData
                .Where(providerData => !combinedGraphData.Any(combinedData => combinedData.Date == providerData.Date))
                .Select(providerData => new GraphData
                {
                    Date = providerData.Date,
                    Providers = providerData.Providers,
                    Drivers = 0
                });

            var remainingDriverData = driver.graphData
                .Where(driverData => !combinedGraphData.Any(combinedData => combinedData.Date == driverData.Date))
                .Select(driverData => new GraphData
                {
                    Date = driverData.Date,
                    Providers = 0,
                    Drivers = driverData.Drivers
                });

            combinedGraphData.AddRange(remainingProviderData);
            combinedGraphData.AddRange(remainingDriverData);

            combined.graphData = combinedGraphData;

            // Final Result
            var onBoardingResponse = new OnBoardingResponse();
            onBoardingResponse.providersAndDrivers = combined;

            return onBoardingResponse;
        }

        public async Task<UserResponse> GetAllUsersDataAsync()
        {
            // Users
            var userList = from u in _dbContext.Users
                           join p in _dbContext.Providers on u.ID equals p.UserID
                           into userProvider
                           from p in userProvider.DefaultIfEmpty()
                           join d in _dbContext.Drivers on u.ID equals d.UserID
                           into userProviderDriver
                           from d in userProviderDriver.DefaultIfEmpty()
                           select new UserList
                           {
                               UserID = u.ID,
                               IsOnline = u.IsOnline,
                               Name = $"{u.FirstName} {u.LastName}",
                               Revenue = 0,
                               MobileNumber = u.Phone,
                               Address = u.Address,
                               SignUpDate = u.CreatedAt,
                               UserStatus = u.UserStatus == 1 ? "Active" : "Inactive",
                               ProviderStatus = p == null ? "-" : p.ProviderStatus == 2 ? "Pending" : p.ProviderStatus == 3 ? "Approved" : "Rejected",
                               DriverStatus = d == null ? "-" : d.DriverStatus == 2 ? "Pending" : d.DriverStatus == 3 ? "Approved" : "Rejected"
                           };
            var userSammary = new UserSummary();
            userSammary.TotalUser = await userList.CountAsync();
            userSammary.ActiveUser = await userList.Where(x => x.UserStatus == "Active").CountAsync();
            userSammary.NewUser = await userList.Where(x => x.SignUpDate >= DateTime.Now.AddMonths(-1)).CountAsync();
            userSammary.LiveUser = await userList.Where(x => x.IsOnline).CountAsync();
            userSammary.ReportedUser = 0;

            // Providers
            var providerSummary = new ProviderSummary();
            var providers = from u in _dbContext.Users
                            join p in _dbContext.Providers on u.ID equals p.UserID
                            join s in _dbContext.Stores on p.ID equals s.ProviderID
                            join rs in _dbContext.ReportedStores on s.ID equals rs.StoreID into tempStore
                            from ts in tempStore.DefaultIfEmpty()
                            select new
                            {
                                u.UserStatus,
                                p.CreatedAt,
                                p.IsOnline,
                                IsReported = ts != null
                            };
            providerSummary.TotalProvider = await providers.CountAsync();
            providerSummary.NewProvider = await providers.CountAsync(x => x.CreatedAt >= DateTime.Now.AddMonths(-1));
            providerSummary.ActiveProvider = await providers.CountAsync(x => x.UserStatus == 1);
            providerSummary.LiveProvider = await providers.CountAsync(x => x.IsOnline);
            providerSummary.ReportedProvider = providers.Count(x => x.IsReported);

            // Drivers
            var driverSummary = new DriverSummary();
            var drivers = from u in _dbContext.Users
                          join d in _dbContext.Drivers on u.ID equals d.UserID
                          join ro in _dbContext.ReportedOrders on d.ID equals ro.DriverID into tempDrivers
                          from td in tempDrivers.DefaultIfEmpty()
                          select new
                          {
                              u.UserStatus,
                              d.CreatedAt,
                              d.IsOnline,
                              IsReported = td != null
                          };
            driverSummary.TotalDriver = await drivers.CountAsync();
            driverSummary.NewDriver = await drivers.CountAsync(x => x.CreatedAt >= DateTime.Now.AddMonths(-1));
            driverSummary.ActiveDriver = await drivers.CountAsync(x => x.UserStatus == 1);
            driverSummary.LiveDriver = await drivers.CountAsync(x => x.IsOnline);
            driverSummary.ReportedDriver = await drivers.CountAsync(x => x.IsReported);

            // Over All
            var userResponse = new UserResponse();
            userResponse.userSummary = userSammary;
            userResponse.providerSummary = providerSummary;
            userResponse.driverSummary = driverSummary;

            return userResponse;
        }

        public async Task<PaginationResponse> GetFilteredUsersDataAsync(UserFilterRequest request)
        {
            var list = from u in _dbContext.Users
                       join p in _dbContext.Providers on u.ID equals p.UserID into userProvider
                       from p in userProvider.DefaultIfEmpty()
                       join d in _dbContext.Drivers on u.ID equals d.UserID into userProviderDriver
                       from d in userProviderDriver.DefaultIfEmpty()
                       where (request.UserType == 0 || (request.UserType == 1 ? (d == null && p == null) : request.UserType == 2 ? (p != null) : (d != null)))
                       && (request.UserStatus == null || u.UserStatus == request.UserStatus)
                       && (string.IsNullOrEmpty(request.Gender) || u.Gender == request.Gender)
                       && (request.Status == null || u.IsOnline == request.Status)
                       && (request.DateRange.StartDate == null || (u.CreatedAt >= request.DateRange.StartDate && u.CreatedAt <= request.DateRange.EndDate))
                       && u.IsProfileCompleted
                       select new UserList
                       {
                           UserID = u.ID,
                           Name = $"{u.FirstName} {u.LastName}",
                           Revenue = (from po in _dbContext.ProviderOrders.Where(x => x.ProviderID == p.ID)
                                      join t in _dbContext.Transactions on po.OrderID equals t.OrderID
                                      select t).Sum(t => t.Amount) +
                                      (from dor in _dbContext.DriverOrders.Where(x => x.DriverID == d.ID)
                                       join t in _dbContext.Transactions on dor.OrderID equals t.OrderID
                                       select t).Sum(t => t.DeliveryCharges),
                           OrderAcceptanceRate = (
                                    from od in _dbContext.Orders
                                    join po in _dbContext.ProviderOrders.Where(x => x.ProviderID == p.ID) on od.ID equals po.OrderID
                                    group new { od, po } by od.Status into statusGroup
                                    select statusGroup.Count(po => po.po.ID > 0) > 0 ?
                                           (double)statusGroup.Sum(od => od.od.Status == 2 ? 1 : 0) /
                                           statusGroup.Count(po => po.po.ID > 0) * 100 : 0.0
                                ).FirstOrDefault(),
                           MobileNumber = u.Phone,
                           Address = u.Address,
                           SignUpDate = u.CreatedAt,
                           UserStatus = ((UserStatus)u.UserStatus).ToString(),
                           ProviderStatus = p == null ? "-" : ((ProviderStatus)p.ProviderStatus).ToString(),
                           DriverStatus = d == null ? "-" : ((DriverStatus)d.DriverStatus).ToString(),
                           IsOnline = u.IsOnline
                       };

            var totalRecords = list.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            PaginationResponse response = new PaginationResponse
            {
                CurrentPage = request.PageNumber,
                TotalPages = totalPages,
                PageSize = request.PageSize,
                TotalCount = totalRecords,
                Data = await list.OrderByDescending(x => x.SignUpDate)
                                 .Skip((request.PageNumber - 1) * request.PageSize)
                                 .Take(request.PageSize)
                                 .ToListAsync()
            };

            return response;
        }

        public async Task<UserProfile> GetUserProfile(long UserID)
        {
            var userProfile = new UserProfile();
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("Default")))
            {
                var param = new DynamicParameters();
                param.Add("@UserID", UserID);
                using (var m = db.QueryMultiple("[baetoti].[GetUserProfile]", param, commandType: CommandType.StoredProcedure))
                {

                    var buyer = await m.ReadFirstOrDefaultAsync<BuyerResponse>();
                    userProfile.buyer = buyer;

                    var buyerHistory = m.Read<BuyerHistory>().ToList();
                    userProfile.buyer.buyerHistory = buyerHistory;

                    var provider = await m.ReadFirstOrDefaultAsync<ProviderResponse>();
                    var storeSchedule = m.Read<WeekDays>().ToList();
                    var items = m.Read<ItemListResponse>().ToList();
                    var order = m.Read<ProviderOrders>().ToList();
                    var order2 = m.Read<ProviderOrders2>().ToList();
                    if (provider != null)
                    {
                        userProfile.provider = provider;
                        userProfile.provider.weekDays = storeSchedule;
                        userProfile.provider.Items = items;
                        userProfile.provider.Orders = order;
                        userProfile.provider.Orders2 = order2;
                    }

                    var driver = await m.ReadFirstOrDefaultAsync<DriverResponse>();
                    if (driver != null)
                    {
                        userProfile.driver = driver;

                        var deliveryDetail = m.Read<DeliveryDetail>().ToList();
                        userProfile.driver.deliveryDetails = deliveryDetail;
                    }

                    var analyticalData = await m.ReadFirstOrDefaultAsync<AnalyticalData>();
                    var providerAnalyticalData = await m.ReadFirstOrDefaultAsync<AnalyticalData>();
                    var driverAnalyticalData = await m.ReadFirstOrDefaultAsync<AnalyticalData>();
                    if (analyticalData != null)
                    {
                        userProfile.analytics.analyticalData = analyticalData;
                        userProfile.analytics.providerAnalyticalData = providerAnalyticalData;
                        userProfile.analytics.driverAnalyticalData = driverAnalyticalData;

                        var cancelledOrder = m.Read<UserCancelledOrder>().ToList();
                        var userCancelledOrder = new UserCancelledOrder2();
                        cancelledOrder.ForEach(x =>
                        {
                            userCancelledOrder.CancelledOrder.Add(x.CancelledOrder);
                            userCancelledOrder.TotalOrder.Add(x.TotalOrder);
                            userCancelledOrder.OrderDate.Add(x.OrderDate);
                        });
                        userProfile.analytics.userCancelledOrder = userCancelledOrder;

                        var providerOrderPrice = m.Read<OrderPrice2>().ToList();
                        var orderPrice = new OrderPrice();
                        providerOrderPrice.ForEach(x =>
                        {
                            orderPrice.TotalOrder.Add(x.TotalOrder);
                            orderPrice.TotalPrice.Add(x.TotalPrice);
                            orderPrice.OrderDate.Add(x.OrderDate);
                        });
                        userProfile.analytics.provider.orderPrice = orderPrice;

                        var providerCancelledOrder = m.Read<CancelledOrder>().ToList();
                        var providerCancelledOrder2 = new CancelledOrder2();
                        providerCancelledOrder.ForEach(x =>
                        {
                            providerCancelledOrder2.Cancelled.Add(x.Cancelled);
                            providerCancelledOrder2.TotalOrder.Add(x.TotalOrder);
                            providerCancelledOrder2.OrderDate.Add(x.OrderDate);
                        });
                        userProfile.analytics.provider.cancelledOrder = providerCancelledOrder2;

                        var driverDeliveryTimeAccuracy = m.Read<DeliveryTimeAccuracy2>().ToList();
                        var deliveryTimeAccuracy = new DeliveryTimeAccuracy();
                        driverDeliveryTimeAccuracy.ForEach(x =>
                        {
                            deliveryTimeAccuracy.Schedule.Add(x.Schedule);
                            deliveryTimeAccuracy.Actual.Add(x.Actual);
                            deliveryTimeAccuracy.Date.Add(x.Date);
                        });
                        userProfile.analytics.driver.deliveryTimeAccuracy = deliveryTimeAccuracy;

                        var driverCancelledOrder2 = m.Read<DriverCancelledOrder2>().ToList();
                        var driverCancelledOrder = new DriverCancelledOrder();
                        driverCancelledOrder2.ForEach(x =>
                        {
                            driverCancelledOrder.TotalOrder.Add(x.TotalOrder);
                            driverCancelledOrder.Cancelled.Add(x.Cancelled);
                            driverCancelledOrder.OrderDate.Add(x.OrderDate);
                        });
                        userProfile.analytics.driver.driverCancelledOrder = driverCancelledOrder;

                        var totalAcceptedOrder2 = m.Read<TotalAcceptedOrder2>().ToList();
                        var totalAcceptedOrder = new TotalAcceptedOrder();
                        totalAcceptedOrder2.ForEach(x =>
                        {
                            totalAcceptedOrder.TotalOrder.Add(x.TotalOrder);
                            totalAcceptedOrder.AcceptedOrder.Add(x.AcceptedOrder);
                            totalAcceptedOrder.OrderDate.Add(x.OrderDate);
                        });
                        userProfile.analytics.order.totalAcceptedOrder = totalAcceptedOrder;

                        var averageOrderPrice2 = m.Read<AverageOrderPrice2>().ToList();
                        var averageOrderPrice = new AverageOrderPrice();
                        averageOrderPrice2.ForEach(x =>
                        {
                            averageOrderPrice.TotalOrder.Add(x.TotalOrder);
                            averageOrderPrice.OrderDate.Add(x.OrderDate);
                        });
                        userProfile.analytics.order.averageOrderPrice = averageOrderPrice;

                        var orderTimeAccuracy2 = m.Read<OrderTimeAccuracy2>().ToList();
                        var orderTimeAccuracy = new OrderTimeAccuracy();
                        orderTimeAccuracy2.ForEach(x =>
                        {
                            orderTimeAccuracy.Schedule.Add(x.Schedule);
                            orderTimeAccuracy.Actual.Add(x.Actual);
                            orderTimeAccuracy.Date.Add(x.Date);
                        });
                        userProfile.analytics.order.orderTimeAccuracy = orderTimeAccuracy;
                    }

                    var transactionsHistory = m.Read<TransactionsHistory>().ToList();
                    userProfile.wallet.transactionsHistory = transactionsHistory;

                    if (provider == null && driver == null)
                        userProfile.isViewUserProfile = false;
                    else
                        userProfile.isViewUserProfile = true;

                    if (provider == null)
                    {
                        userProfile.isProvider = false;
                        userProfile.isStoreCreated = false;
                    }
                    else
                    {
                        userProfile.isProvider = true;
                        if (string.IsNullOrEmpty(provider.StoreName))
                            userProfile.isStoreCreated = false;
                        else
                            userProfile.isStoreCreated = true;
                    }

                    if (driver == null)
                        userProfile.isDriver = false;
                    else
                        userProfile.isDriver = true;
                }
            }
            return userProfile;
        }

        public async Task<BookmarkAndVisitResponse> GetBookmarkAndAccountVisit(long UserID, int UserType)
        {
            BookmarkAndVisitResponse response = new BookmarkAndVisitResponse();
            if (UserType == (int)Shared.Enum.UserType.Provider)
            {
                response.BookmarkCount = await (from fs in _dbContext.FavouriteStores
                                                join s in _dbContext.Stores on fs.StoreID equals s.ID
                                                join p in _dbContext.Providers on s.ProviderID equals p.ID
                                                where p.UserID == UserID
                                                select fs.ID).CountAsync();
                response.AccountVisitCount = await (from av in _dbContext.AccountVisits
                                                    join p in _dbContext.Providers on av.ProviderID equals p.ID
                                                    where p.UserID == UserID && av.UserType == 2
                                                    select av.ID).CountAsync();
            }
            if (UserType == (int)Shared.Enum.UserType.Driver)
            {
                response.BookmarkCount = await (from fd in _dbContext.FavouriteDrivers
                                                join d in _dbContext.Drivers on fd.DriverID equals d.ID
                                                where d.UserID == UserID
                                                select fd.ID).CountAsync();
                response.AccountVisitCount = await (from av in _dbContext.AccountVisits
                                                    join d in _dbContext.Drivers on av.DriverID equals d.ID
                                                    where d.UserID == UserID && av.UserType == 3
                                                    select av.ID).CountAsync();
            }
            return response;
        }

        public async Task<List<UserSearchResponse>> SearchUser(UserSearchRequest request)
        {
            return await (from u in _dbContext.Users
                          where u.FirstName.Contains(request.Name) || u.LastName.Contains(request.Name)
                          select new UserSearchResponse
                          {
                              ID = u.ID,
                              Name = $"{u.FirstName} {u.LastName}"
                          }).ToListAsync();
        }

        public async Task<List<UserSearchResponse>> SearchFilterUser(UserFliterSearchRequest request)
        {
            if (request.UserType == 0 || request.UserType == 1)
            {
                var defaultUsersQuery = _dbContext.Users
                    .Where(u =>
                        (string.IsNullOrEmpty(request.Name) || u.FirstName.Contains(request.Name) || u.LastName.Contains(request.Name))
                        && (string.IsNullOrEmpty(request.Gender) || u.Gender.Contains(request.Gender))
                        && (request.CountryID == null || u.CountryID == request.CountryID)
                        && (string.IsNullOrEmpty(request.RegionID) || u.RegionID == request.RegionID)
                        && (string.IsNullOrEmpty(request.CityID) || u.CityID == request.CityID)
                    );

                // Project to UserSearchResponse here
                var defaultUsers = defaultUsersQuery
                    .Select(u => new UserSearchResponse
                    {
                        ID = u.ID,
                        Name = $"{u.FirstName} {u.LastName}",
                        UserType = 1
                    });

                if (request.UserType == 0)
                {
                    return await defaultUsers.ToListAsync();
                }

                // Join and project for providerUsers
                var providerUsers = defaultUsersQuery
                    .Join(_dbContext.Providers, u => u.ID, p => p.UserID, (u, p) => new UserSearchResponse
                    {
                        ID = u.ID,
                        Name = $"{u.FirstName} {u.LastName}",
                        UserType = 2
                    });

                // Join and project for driverUsers
                var driverUsers = defaultUsersQuery
                    .Join(_dbContext.Drivers, u => u.ID, d => d.UserID, (u, d) => new UserSearchResponse
                    {
                        ID = u.ID,
                        Name = $"{u.FirstName} {u.LastName}",
                        UserType = 3
                    });

                // Perform set operations before the final select
                var result = defaultUsers.ToList()
                    .Except(providerUsers.ToList())
                    .Except(driverUsers.ToList())
                    .ToList();
                return result.DistinctBy(x => x.ID).ToList();
            }

            var query = from u in _dbContext.Users
                        where
                         (string.IsNullOrEmpty(request.Name) || u.FirstName.Contains(request.Name) || u.LastName.Contains(request.Name))
                          && (string.IsNullOrEmpty(request.Gender) || u.Gender.Contains(request.Gender))
                          &&
                         (
                         (
                         request.UserType == 2
                         && _dbContext.Providers.Any(b => b.UserID == u.ID
                         && ((request.CountryID == null) || b.CountryId == request.CountryID)
                         && (string.IsNullOrEmpty(request.RegionID) || b.RegionId == request.RegionID)
                         && (string.IsNullOrEmpty(request.CityID) || b.CityId == request.CityID))
                         )
                        ||
                        (
                        request.UserType == 3
                        && _dbContext.Drivers.Any(d => d.UserID == u.ID
                        && ((request.CountryID == null) || d.CountryId == request.CountryID)
                        && (string.IsNullOrEmpty(request.RegionID) || d.RegionId == request.RegionID)
                        && (string.IsNullOrEmpty(request.CityID) || d.CityId == request.CityID))
                        )
                        )
                        select new UserSearchResponse
                        {
                            ID = u.ID,
                            Name = u.FirstName + " " + u.LastName,
                            UserType = request.UserType
                        };
            return query.ToList();
        }

    }
}
