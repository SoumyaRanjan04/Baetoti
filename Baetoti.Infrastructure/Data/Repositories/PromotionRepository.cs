using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Request.Promotion;
using Baetoti.Shared.Response.Promotion;
using System.Threading.Tasks;
using System.Linq;
using Baetoti.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using Baetoti.Shared.Response.Dashboard;
using Baetoti.Shared.Response.User;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using static Baetoti.Shared.Response.Promotion.ViewPromotionResponse;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.SupportRequest;
using System;
using System.Collections.Generic;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class PromotionRepository : EFRepository<Promotion>, IPromotionRepository
    {

        private readonly BaetotiDbContext _dbContext;
        private readonly IConfiguration _config;

        public PromotionRepository(BaetotiDbContext dbContext, IConfiguration config) : base(dbContext)
        {
            _dbContext = dbContext;
            _config = config;
        }

        public async Task<GetPromotionResponse> GetAll(GetPromotionRequest getPromotionRequest)
        {
            GetPromotionResponse getPromotionResponse = new GetPromotionResponse();

            var res = from p in _dbContext.Promotions
                      join up in _dbContext.UserPromotions on p.ID equals up.PromotionID
                      join u in _dbContext.Users on up.UserID equals u.ID
                      select new Promo
                      {
                          ID = p.ID,
                          User = $"{u.FirstName} {u.LastName}",
                          PromoStart = p.CreatedAt.ToString(),
                          PromoExpiration = p.PromoCodeValidity.ToString(),
                          By = $"{u.FirstName} {u.LastName}",
                          TotalSignups = u != null ? 1 : 0,
                          PromoStatus = p.PromotionStatus
                      };
            getPromotionResponse.ExpiredPromo = await res.Where(x => x.PromoStatus == (int)PromotionStatus.Inactive).ToListAsync();
            getPromotionResponse.ExistingPromo = await res.Where(x => x.PromoStatus == (int)PromotionStatus.Active).ToListAsync();

            getPromotionResponse.Ongoing = getPromotionResponse.ExistingPromo.Count;
            getPromotionResponse.Expired = getPromotionResponse.ExpiredPromo.Count;
            getPromotionResponse.Total = getPromotionResponse.Ongoing + getPromotionResponse.Expired;

            // Revenue VS Commision
            var transactions = _dbContext.Transactions.Select(t => t);

            DateTime startDate = DateTime.Now.AddMonths(-1);
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(Convert.ToString(getPromotionRequest.StartDate)) && !string.IsNullOrEmpty(Convert.ToString(getPromotionRequest.EndDate)))
            {
                startDate = Convert.ToDateTime(getPromotionRequest.StartDate);
                endDate = Convert.ToDateTime(getPromotionRequest.EndDate);
            }

            getPromotionResponse.RevenueVsCommisions = transactions
                .Where(t => t.TransactionTime >= startDate && t.TransactionTime <= endDate)
                .GroupBy(t => t.TransactionTime.Date)
                .Select(group => new RevenueVsCommision
                {
                    date = group.Key.ToString("yyyy-MM-dd"),
                    Revenue = group.Sum(t => (int)(t.Amount + t.DeliveryCharges)),
                    Commision = group.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee)
                }).ToList();

            // Sales
            getPromotionResponse.Sales = await (from t in _dbContext.Transactions
                                                where t.TransactionTime >= startDate && t.TransactionTime <= endDate
                                                group t by t.TransactionTime.Date into g
                                                select new Sale
                                                {
                                                    Date = g.Key.ToString("MMM dd"),
                                                    Sales = g.Count(),
                                                    Revenue = g.Sum(x => x.ProviderCommision + x.DriverCommision + x.ServiceFee)
                                                }).ToListAsync();

            // SignUp
            var usersQuery = _dbContext.Users
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate);

            var driversQuery = _dbContext.Drivers
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate);

            var providersQuery = _dbContext.Providers
                .Where(t => t.CreatedAt >= startDate && t.CreatedAt <= endDate);

            // Get total and online user counts
            var totalUsersPerDate = usersQuery
                .AsEnumerable() // Switch to client evaluation after fetching data
                .GroupBy(u => u.CreatedAt.Value.Date)
                .Select(group => new UserCount
                {
                    Date = group.Key.ToString("yyyy-MM-dd"),
                    TotalUser = group.Count(),
                })
                .ToList();

            var onlineUsersPerDate = usersQuery
                .Where(u => true)
                .AsEnumerable() // Switch to client evaluation after fetching data
                .GroupBy(u => u.CreatedAt.Value.Date)
                .Select(group => new UserCount
                {
                    Date = group.Key.ToString("yyyy-MM-dd"),
                    OnlineUser = group.Count(),
                })
                .ToList();

            // Get total and online driver counts
            var totalDriversPerDate = driversQuery
                .AsEnumerable() // Switch to client evaluation after fetching data
                .GroupBy(d => d.CreatedAt.Value.Date)
                .Select(group => new UserCount
                {
                    Date = group.Key.ToString("yyyy-MM-dd"),
                    TotalUser = group.Count(),
                })
                .ToList();

            var onlineDriversPerDate = driversQuery
                .Where(d => d.IsOnline)
                .AsEnumerable() // Switch to client evaluation after fetching data
                .GroupBy(d => d.CreatedAt.Value.Date)
                .Select(group => new UserCount
                {
                    Date = group.Key.ToString("yyyy-MM-dd"),
                    OnlineUser = group.Count(),
                })
                .ToList();

            // Get total and online provider counts
            var totalProvidersPerDate = providersQuery
                .AsEnumerable() // Switch to client evaluation after fetching data
                .GroupBy(p => p.CreatedAt.Value.Date)
                .Select(group => new UserCount
                {
                    Date = group.Key.ToString("yyyy-MM-dd"),
                    TotalUser = group.Count(),
                })
                .ToList();

            var onlineProvidersPerDate = providersQuery
                .Where(p => p.IsOnline)
                .AsEnumerable() // Switch to client evaluation after fetching data
                .GroupBy(p => p.CreatedAt.Value.Date)
                .Select(group => new UserCount
                {
                    Date = group.Key.ToString("yyyy-MM-dd"),
                    OnlineUser = group.Count(),
                })
                .ToList();

            // Populate the TotalVsOnline response
            TotalVsOnline response2 = new TotalVsOnline
            {
                Users = totalUsersPerDate,
                TotalDrivers = totalDriversPerDate,
                TotalProviders = totalProvidersPerDate,
                TotalUser = totalUsersPerDate.Sum(u => u.TotalUser),
                TotalDriver = totalDriversPerDate.Sum(d => d.TotalUser),
                TotalProvider = totalProvidersPerDate.Sum(p => p.TotalUser),
            };

            // Merge the online user, driver, and provider counts into the response
            MergeOnlineCounts(response2.Users, onlineUsersPerDate);
            MergeOnlineCounts(response2.TotalDrivers, onlineDriversPerDate);
            MergeOnlineCounts(response2.TotalProviders, onlineProvidersPerDate);
            getPromotionResponse.TotalVsOnlines = response2;

            return getPromotionResponse;
        }

        public async Task<ViewPromotionResponse> View(long PromotionID)
        {
            ViewPromotionResponse viewPromotionResponse = new ViewPromotionResponse();

            // Promo Detail

            var promoDetail = _dbContext.Promotions.Where(p => p.ID == PromotionID).Select(x => new PromoDetail
            {
                PromotionType = x.PromotionType == 1 ? "All Users" : "First Time Users",
                PromoCodeName = x.PromoCodeName,
                PromoCodeNameArabic = x.PromoCodeNameArabic,
                PromoCodeDescription = x.PromoCodeDescription,
                PromoCodeDescriptionArabic = x.PromoCodeDescriptionArabic,
                DiscountType = x.DiscountType == 1 ? "Percentage" : "Value",
                DiscountValue = x.DiscountValue,
                MinimumBasketValue = x.MinimumBasketValue,
                PromoBearer = x.PromoBearer,
                Commision = x.Commision,
                CommisionType = x.CommisionType == 1 ? "Percentage" : "Value",
                NumberOfVoucher = x.NumberOfVoucher,
                NumberOfRedeems = x.NumberOfRedeems,
                PromoCodeValidity = x.PromoCodeValidity,
            });

            // Users
            var userList = from u in _dbContext.Users
                           join p in _dbContext.Providers on u.ID equals p.UserID
                           into userProvider
                           from p in userProvider.DefaultIfEmpty()
                           join d in _dbContext.Drivers on u.ID equals d.UserID
                           into userProviderDriver
                           from d in userProviderDriver.DefaultIfEmpty()
                           join up in _dbContext.UserPromotions on u.ID equals up.UserID
                           select new UserList
                           {
                               UserID = u.ID,
                               Name = $"{u.FirstName} {u.LastName}",
                               Revenue = 0,
                               MobileNumber = u.Phone,
                               UserStatus = u.UserStatus == 1 ? "Active" : "Inactive",
                               ProviderStatus = p == null ? "-" : p.ProviderStatus == 2 ? "Pending" : p.ProviderStatus == 3 ? "Approved" : "Rejected",
                               DriverStatus = d == null ? "-" : d.DriverStatus == 2 ? "Pending" : d.DriverStatus == 3 ? "Approved" : "Rejected"
                           };
            viewPromotionResponse.userList = await userList.ToListAsync();

            // Buyer History
            using (IDbConnection db = new SqlConnection(_config.GetConnectionString("Default")))
            {
                var param = new DynamicParameters();
                param.Add("@PromotionID", PromotionID);
                using (var m = db.QueryMultiple("[baetoti].[GetPromotionBuyerHistory]", param, commandType: CommandType.StoredProcedure))
                {
                    var buyerHistory = m.Read<BuyerHistory>().ToList();
                    viewPromotionResponse.buyerHistory = buyerHistory;
                }
            }

            // SignUps
            UserCount user = new UserCount()
            {
                Date = "Jan/4/2016",
                TotalUser = 3,
                OnlineUser = 1
            };
            viewPromotionResponse.TotalVsOnlines.Users.Add(user);
            user = new UserCount()
            {
                Date = "Jan/4/2022",
                TotalUser = 20,
                OnlineUser = 16
            };
            viewPromotionResponse.TotalVsOnlines.Users.Add(user);

            UserCount provider = new UserCount()
            {
                Date = "Jan/4/2016",
                TotalUser = 3,
                OnlineUser = 1
            };
            viewPromotionResponse.TotalVsOnlines.TotalProviders.Add(provider);
            provider = new UserCount()
            {
                Date = "Jan/4/2022",
                TotalUser = 20,
                OnlineUser = 16
            };
            viewPromotionResponse.TotalVsOnlines.TotalProviders.Add(provider);

            UserCount driver = new UserCount()
            {
                Date = "Jan/4/2016",
                TotalUser = 3,
                OnlineUser = 1
            };
            viewPromotionResponse.TotalVsOnlines.TotalDrivers.Add(driver);
            driver = new UserCount()
            {
                Date = "Jan/4/2022",
                TotalUser = 20,
                OnlineUser = 16
            };
            viewPromotionResponse.TotalVsOnlines.TotalDrivers.Add(driver);

            viewPromotionResponse.TotalVsOnlines.TotalUser = 25;
            viewPromotionResponse.TotalVsOnlines.TotalProvider = 15;
            viewPromotionResponse.TotalVsOnlines.TotalDriver = 10;

            // Sales
            Sale Sales = new Sale()  // Pick Top 10
            {
                Date = "24:00",
                Sales = 114,
                Revenue = 15
            };
            viewPromotionResponse.Sales.Add(Sales);
            Sales = new Sale()
            {
                Date = "22:00",
                Sales = 110,
                Revenue = 10
            };
            viewPromotionResponse.Sales.Add(Sales);

            // Revenue VS Commision
            RevenueVsCommision revenueVsCommision = new RevenueVsCommision()
            {
                date = "24:00",
                Commision = 99.43M,
                Revenue = 5
            };
            viewPromotionResponse.RevenueVsCommisions.Add(revenueVsCommision);
            revenueVsCommision = new RevenueVsCommision()
            {
                date = "23:30",
                Commision = 91.43M,
                Revenue = 23
            };
            viewPromotionResponse.RevenueVsCommisions.Add(revenueVsCommision);
            revenueVsCommision = new RevenueVsCommision()
            {
                date = "22:00",
                Commision = 45.43M,
                Revenue = 6
            };
            viewPromotionResponse.RevenueVsCommisions.Add(revenueVsCommision);

            return viewPromotionResponse;
        }

        private static void MergeOnlineCounts(List<UserCount> totalList, List<UserCount> onlineList)
        {
            foreach (var online in onlineList)
            {
                var total = totalList.FirstOrDefault(item => item.Date == online.Date);
                if (total != null)
                {
                    total.OnlineUser = online.OnlineUser;
                }
            }
        }

    }
}
