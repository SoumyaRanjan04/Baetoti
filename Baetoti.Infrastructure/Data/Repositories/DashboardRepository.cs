using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Shared.Response.Dashboard;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Baetoti.Shared.Request.Dashboard;
using Microsoft.EntityFrameworkCore;
using System;
using Baetoti.Shared.Enum;
using Baetoti.Core.Entites;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public DashboardRepository(BaetotiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DashboardResponse> GetDashboardDataAsync(DashboardRequest request)
        {
            DashboardResponse dashboardResponse = new DashboardResponse();

            // Provider
            var providers = from p in _dbContext.Providers
                            join u in _dbContext.Users on p.UserID equals u.ID
                            select new
                            {
                                p.ProviderStatus,
                                u.CreatedAt
                            };
            dashboardResponse.Providers.TotalCount = providers.Count();
            dashboardResponse.Providers.Pending = await providers.Where(x => x.ProviderStatus == 2).CountAsync();
            dashboardResponse.Providers.Approved = await providers.Where(x => x.ProviderStatus == 3 || x.ProviderStatus == 1).CountAsync();
            dashboardResponse.Providers.Rejected = await providers.Where(x => x.ProviderStatus == 4).CountAsync();

            // Driver
            var drivers = from d in _dbContext.Drivers
                          join u in _dbContext.Users on d.UserID equals u.ID
                          select new
                          {
                              d.DriverStatus,
                              u.CreatedAt
                          };
            dashboardResponse.Drivers.TotalCount = await drivers.CountAsync();
            dashboardResponse.Drivers.Pending = await drivers.Where(x => x.DriverStatus == 2).CountAsync();
            dashboardResponse.Drivers.Approved = await drivers.Where(x => x.DriverStatus == 3 || x.DriverStatus == 1).CountAsync();
            dashboardResponse.Drivers.Rejected = await drivers.Where(x => x.DriverStatus == 4).CountAsync();

            // Items
            var items = _dbContext.Items;
            dashboardResponse.Items.TotalCount = await items.CountAsync();
            dashboardResponse.Items.Pending = await items.CountAsync(i => i.ItemStatus == 2);
            dashboardResponse.Items.Approved = await items.CountAsync(i => i.ItemStatus == 3);
            dashboardResponse.Items.Rejected = await items.CountAsync(i => i.ItemStatus == 4);

            // Revenue VS Commision
            var transactions = _dbContext.Transactions.Select(t => t);

            DateTime startDate = DateTime.Now.AddMonths(-1);
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(Convert.ToString(request.StartDate)) && !string.IsNullOrEmpty(Convert.ToString(request.EndDate)))
            {
                startDate = Convert.ToDateTime(request.StartDate);
                endDate = Convert.ToDateTime(request.EndDate);
            }

            // Group transactions by date and calculate revenue and commission for each date
            dashboardResponse.RevenueVsCommisions = transactions
                .Where(t => t.TransactionTime >= startDate && t.TransactionTime <= endDate)
                .GroupBy(t => t.TransactionTime.Date)
                .Select(group => new RevenueVsCommision
                {
                    date = group.Key.ToString("yyyy-MM-dd"),
                    Revenue = group.Sum(t => (int)(t.Amount + t.DeliveryCharges)),
                    Commision = group.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee)
                }).ToList();

            // Location
            dashboardResponse.Locations = await (from u in _dbContext.Users
                                                 join ul in _dbContext.UserLocations on u.ID equals ul.UserID
                                                 where ul.IsDefault == true &&
                                                 u.CreatedAt >= startDate && u.CreatedAt <= endDate
                                                 select new Location
                                                 {
                                                     Lat = ul.Latitude,
                                                     Lng = ul.Longitude
                                                 }).ToListAsync();

            // Total VS Online
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
            dashboardResponse.TotalVsOnlines = response2;

            // New Users
            dashboardResponse.NewUsers = await (from u in _dbContext.Users
                                                join d in _dbContext.Drivers on u.ID equals d.UserID into TempDriver
                                                from dt in TempDriver.DefaultIfEmpty()
                                                join p in _dbContext.Providers on u.ID equals p.UserID into TempProvider
                                                from pt in TempProvider.DefaultIfEmpty()
                                                join s in _dbContext.Stores on pt.ID equals s.ProviderID
                                                orderby u.CreatedAt descending
                                                select new NewUser
                                                {
                                                    Name = s != null ? s.Name : $"{u.FirstName} {u.LastName}",
                                                    Role = pt != null ? "Provider" : dt != null ? "Captain" : "Buyer",
                                                    Picture = s != null ? s.BusinessLogo : u.Picture
                                                }).Take(10).ToListAsync();

            // Sales
            startDate = DateTime.Now.AddMonths(-3);
            endDate = DateTime.Now;
            dashboardResponse.Sales = await (from t in _dbContext.Transactions
                                             where t.TransactionTime >= startDate && t.TransactionTime <= endDate
                                             group t by t.TransactionTime.Date into g
                                             select new Sale
                                             {
                                                 Date = g.Key.ToString("MMM dd"),
                                                 Sales = g.Count(),
                                                 Revenue = g.Sum(x => x.ProviderCommision + x.DriverCommision + x.ServiceFee)
                                             }).ToListAsync();

            // Popular Items
            dashboardResponse.PopularItems = await (from i in _dbContext.Items
                                                    join oi in _dbContext.OrderItems on i.ID equals oi.ItemID
                                                    join t in _dbContext.Transactions on oi.OrderID equals t.OrderID
                                                    group t by new { i.ID, i.Name } into itemGroup
                                                    select new PopularItem
                                                    {
                                                        Id = itemGroup.Key.ID,
                                                        Title = itemGroup.Key.Name,
                                                        Count = itemGroup.Sum(x => x.Amount), // Sum of amounts for revenue
                                                        Percentage = 0
                                                    })
                                        .OrderByDescending(item => item.Count) // Sort by revenue in descending order
                                        .Take(10) // Take top 10 items
                                        .ToListAsync();

            // Calculate the total revenue to calculate the percentage
            decimal totalRevenue = dashboardResponse.PopularItems.Sum(item => item.Count);

            // Calculate the percentage for each item
            foreach (var item in dashboardResponse.PopularItems)
            {
                item.Percentage = Math.Round((item.Count / totalRevenue) * 100, 2);
            }

            // Popular Provider
            var popularProvidersData = await (from p in _dbContext.Providers
                                              join po in _dbContext.ProviderOrders on p.ID equals po.ProviderID
                                              join t in _dbContext.Transactions on po.OrderID equals t.OrderID
                                              join u in _dbContext.Users on p.UserID equals u.ID
                                              join s in _dbContext.Stores on p.ID equals s.ProviderID
                                              group t by new { p.ID, u.FirstName, u.LastName, s.BusinessLogo, s.Name } into providerGroup
                                              select new
                                              {
                                                  Id = providerGroup.Key.ID,
                                                  Name = providerGroup.Key.Name,
                                                  Image = providerGroup.Key.BusinessLogo,
                                                  Count = providerGroup.Count()
                                              })
                                 .ToListAsync();

            // Calculate the total number of transactions to calculate the percentage
            int totalTransactions = popularProvidersData.Sum(provider => provider.Count);

            // Calculate the percentage for each provider
            var popularProvidersWithPercentage = popularProvidersData
                .Select(provider => new PopularProvider
                {
                    Id = provider.Id,
                    Name = provider.Name,
                    Image = provider.Image,
                    Count = provider.Count,
                    Percentage = provider.Count / totalTransactions * 100
                })
                .OrderByDescending(provider => provider.Count) // Sort by sales count in descending order
                .Take(10) // Take top 10 providers
                .ToList();

            dashboardResponse.PopularProviders = popularProvidersWithPercentage;

            // Calculate the total number of transactions to calculate the percentage
            totalTransactions = dashboardResponse.PopularProviders.Sum(provider => provider.Count);

            // Calculate the percentage for each provider
            foreach (var provider in dashboardResponse.PopularProviders)
            {
                provider.Percentage = Math.Round((provider.Count / totalRevenue) * 100, 2);
            }

            // Order Summary
            var orders = _dbContext.Orders.Select(o => o);
            dashboardResponse.OrderSummary.OrderCompleted = await orders.CountAsync(o => o.Status == 11);
            dashboardResponse.OrderSummary.Pending = await orders.CountAsync(o => o.Status == 0);
            dashboardResponse.OrderSummary.Approved = await orders.CountAsync(o => o.Status == 1);
            dashboardResponse.OrderSummary.OnTheWay = await orders.CountAsync(o => o.Status == 5);
            dashboardResponse.OrderSummary.Cancelled = await orders.CountAsync(o => o.Status == 8 || o.Status == 9 || o.Status == 10);

            // Latest Orders
            dashboardResponse.LatestOrders = await (from o in _dbContext.Orders
                                                    join u in _dbContext.Users on o.UserID equals u.ID
                                                    join pr in _dbContext.ProviderOrders on o.ID equals pr.OrderID
                                                    join p in _dbContext.Providers on pr.ProviderID equals p.ID
                                                    join pu in _dbContext.Users on p.UserID equals pu.ID
                                                    join s in _dbContext.Stores on p.ID equals s.ProviderID
                                                    join dor in _dbContext.DriverOrders on o.ID equals dor.OrderID into driverOrderTemp
                                                    from dot in driverOrderTemp.DefaultIfEmpty()
                                                    join d in _dbContext.Drivers on dot.DriverID equals d.ID into driverTemp
                                                    from dt in driverTemp.DefaultIfEmpty()
                                                    join du in _dbContext.Users on dt.UserID equals du.ID into driverUserTemp
                                                    from dut in driverUserTemp.DefaultIfEmpty()
                                                    select new LatestOrder
                                                    {
                                                        OrderId = o.ID,
                                                        ProviderId = p.ID,
                                                        UserID = u.ID,
                                                        ProviderUserID = pu.ID,
                                                        DriverUserID = dut != null ? dut.ID : 0,
                                                        Buyer = $"{u.FirstName} {u.LastName}",
                                                        Driver = $"{dut.FirstName} {dut.LastName}",
                                                        Provider = s.Name,
                                                        Date = o.CreatedAt.Value.Date.ToString(),
                                                        Status = ((OrderStatus)o.Status).ToString()
                                                    })
                                      .OrderByDescending(o => o.Date)
                                      .Take(10)
                                      .ToListAsync();

            return dashboardResponse;
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
