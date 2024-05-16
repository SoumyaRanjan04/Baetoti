using Baetoti.Core.Interface.Services;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Shared.Request.Analytic;
using Baetoti.Shared.Response.Analytic;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Request.Shared;
using System;
using System.Globalization;
using Baetoti.Core.Entites;
using Baetoti.Shared.Response.Dashboard;
using System.Text.RegularExpressions;

namespace Baetoti.Infrastructure.Services
{
    public class AnalyticsService : IAnalyticsService
    {

        private readonly BaetotiDbContext _dbContext;

        public AnalyticsService(BaetotiDbContext baetotiDbContext)
        {
            _dbContext = baetotiDbContext;
        }

        public async Task<List<MapResponse>> GetMapData(FilterRequest filter)
        {
            var baseQuery = from u in _dbContext.Users
                            join ul in _dbContext.UserLocations on u.ID equals ul.UserID
                            where ul.IsDefault == true &&
                            u.CreatedAt >= filter.StartDate &&
                            u.CreatedAt <= filter.EndDate &&
                            (string.IsNullOrEmpty(filter.Gender) || u.Gender == filter.Gender)
                            select new MapResponse
                            {
                                UserID = u.ID,
                                Name = $"{u.FirstName} {u.LastName}",
                                Latitude = ul.Latitude,
                                Longitude = ul.Longitude
                            };

            if (filter.UserType == (int)UserType.Buyer)
            {
                baseQuery = baseQuery.Where(user =>
                    !_dbContext.Drivers.Any(driver => driver.UserID == user.UserID) &&
                    !_dbContext.Providers.Any(provider => provider.UserID == user.UserID)
                );
            }
            else if (filter.UserType == (int)UserType.Provider)
            {
                baseQuery = baseQuery.Where(user =>
                    _dbContext.Providers.Any(provider => provider.UserID == user.UserID)
                );
            }
            else if (filter.UserType == (int)UserType.Driver)
            {
                baseQuery = baseQuery.Where(user =>
                    _dbContext.Drivers.Any(driver => driver.UserID == user.UserID)
                );
            }

            if (filter.CategoryID > 0)
            {
                baseQuery = baseQuery.Where(user =>
                    _dbContext.Providers.Any(provider =>
                        _dbContext.Items.Any(item =>
                            item.ProviderID == provider.ID &&
                            item.CategoryID == filter.CategoryID &&
                            (filter.SubCategoryID == 0 || item.SubCategoryID == filter.SubCategoryID)
                        )
                    )
                );
            }

            return await baseQuery.ToListAsync();
        }

        public async Task<MarkovResponse> GetMarkovData(RequestID filter)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ID == filter.ID);

            if (user == null)
            {
                throw new Exception("Unable to find user with provided id");
            }

            var orderCount = await _dbContext.Orders.CountAsync(o => o.UserID == filter.ID);

            var analysisData = await (from o in _dbContext.Orders
                                      where o.UserID == filter.ID
                                      join po in _dbContext.ProviderOrders on o.ID equals po.OrderID
                                      join s in _dbContext.Stores on po.ProviderID equals s.ProviderID
                                      group s by s.Name into g
                                      select new AnalysisData
                                      {
                                          Store = g.Key,
                                          Rate = Math.Round((decimal)g.Count() / orderCount * 100, 2)
                                      }).ToListAsync();

            var markovResponse = new MarkovResponse
            {
                Name = $"{user.FirstName} {user.LastName}",
                Data = analysisData
            };

            return markovResponse;
        }

        public async Task<List<RevenuePerDayResponse>> GetRevenuPerDayData(RevenuePerDayRequest filter)
        {
            List<RevenuePerDayResponse> response = new List<RevenuePerDayResponse>();
            if (filter.RevenuPerDayFilter == (int)RevenuPerDayFilter.Daily)
            {
                var startDate = DateTime.Today.AddDays(-7); // or any specific start date you want
                var endDate = DateTime.Today; // or any specific end date you want (one week)

                var fullDayHours = Enumerable.Range(0, 24)
                    .Select(hour => startDate.AddHours(hour));

                var dayNames = Enum.GetNames(typeof(DayOfWeek));

                // Header row
                var headerRow = new RevenuePerDayResponse
                {
                    Label = "Hour",
                    Revenue = new Dictionary<string, object>()
                };

                // Add day names to the header row
                foreach (var dayOfWeek in Enumerable.Range(0, 7))
                {
                    headerRow.Revenue[dayNames[dayOfWeek]] = dayNames[dayOfWeek];
                }
                headerRow.Revenue["GrandTotal"] = "Grand Total";

                response.Add(headerRow);

                // Data rows
                foreach (var hour in fullDayHours)
                {
                    var dataRow = new RevenuePerDayResponse
                    {
                        Label = $"{hour:hh tt}",
                        Revenue = new Dictionary<string, object>()
                    };

                    // Iterate through each day of the week to find the revenue for the current hour and day
                    foreach (var dayOfWeek in Enumerable.Range(0, 7))
                    {
                        var currentDay = startDate.AddDays(dayOfWeek);
                        var transactionsForHour = _dbContext.Transactions
                            .Where(t => t.TransactionTime >= currentDay && t.TransactionTime < currentDay.AddDays(1) && t.TransactionTime.Hour == hour.Hour);

                        var totalForHour = await transactionsForHour.SumAsync(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
                        dataRow.Revenue[dayNames[dayOfWeek]] = totalForHour;
                    }

                    decimal rowTotal = 0;

                    foreach (var dayOfWeek in Enumerable.Range(0, 7))
                    {
                        var currentDay = startDate.AddDays(dayOfWeek);
                        var transactionsForHour = _dbContext.Transactions
                            .Where(t => t.TransactionTime >= currentDay && t.TransactionTime < currentDay.AddDays(1) && t.TransactionTime.Hour == hour.Hour);

                        var totalForHour = transactionsForHour.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
                        dataRow.Revenue[dayNames[dayOfWeek]] = totalForHour;
                        rowTotal += totalForHour;
                    }

                    // Add the row total to the last column of the data row
                    dataRow.Revenue["GrandTotal"] = rowTotal;

                    response.Add(dataRow);
                }

                // Grand Total row
                var grandTotalRow = new RevenuePerDayResponse
                {
                    Label = "Grand Total",
                    Revenue = new Dictionary<string, object>()
                };

                // Iterate through each day of the week to find the revenue for the entire week for each day
                foreach (var dayOfWeek in Enumerable.Range(0, 7))
                {
                    var currentDay = startDate.AddDays(dayOfWeek);
                    var transactionsForDay = _dbContext.Transactions
                        .Where(t => t.TransactionTime >= currentDay && t.TransactionTime < currentDay.AddDays(1));

                    var totalForDay = transactionsForDay.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
                    grandTotalRow.Revenue[dayNames[dayOfWeek]] = totalForDay;
                }

                response.Add(grandTotalRow);
            }
            else if (filter.RevenuPerDayFilter == (int)RevenuPerDayFilter.Monthly)
            {
                var startDate = DateTime.Today.AddMonths(-1); // or any specific start date you want
                var endDate = DateTime.Today; // or any specific end date you want (one month)

                var dayNames = Enumerable.Range(1, DateTime.DaysInMonth(startDate.Year, startDate.Month))
                    .Select(day => day.ToString("00"));

                var monthNames = Enumerable.Range(1, 12)
                    .Select(month => new DateTime(2000, month, 1).ToString("MMM", CultureInfo.InvariantCulture));

                // Header row
                var headerRow = new RevenuePerDayResponse
                {
                    Label = "Day",
                    Revenue = new Dictionary<string, object>()
                };

                // Add month names to the header row
                foreach (var monthName in monthNames)
                {
                    headerRow.Revenue[monthName] = monthName;
                }
                headerRow.Revenue["GrandTotal"] = "Grand Total";

                response.Add(headerRow);

                // Data rows
                foreach (var dayNumber in dayNames)
                {
                    var dataRow = new RevenuePerDayResponse
                    {
                        Label = dayNumber,
                        Revenue = new Dictionary<string, object>()
                    };

                    // Iterate through each month to find the revenue for the current day and month
                    foreach (var monthName in monthNames)
                    {
                        var currentMonth = new DateTime(DateTime.Now.Year, monthNames.ToList().FindIndex(m => m == monthName) + 1, 1);
                        var transactionsForDay = _dbContext.Transactions
                            .Where(t => t.TransactionTime >= currentMonth && t.TransactionTime < currentMonth.AddMonths(1) && t.TransactionTime.Day == int.Parse(dayNumber));

                        var totalForDay = transactionsForDay.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
                        dataRow.Revenue[monthName] = totalForDay;
                    }

                    decimal rowTotal = 0;

                    // Calculate the row total
                    foreach (var monthName in monthNames)
                    {
                        var currentMonth = new DateTime(DateTime.Now.Year, monthNames.ToList().FindIndex(m => m == monthName) + 1, 1);
                        var transactionsForDay = _dbContext.Transactions
                            .Where(t => t.TransactionTime >= currentMonth && t.TransactionTime < currentMonth.AddMonths(1) && t.TransactionTime.Day == int.Parse(dayNumber));

                        var totalForDay = transactionsForDay.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
                        dataRow.Revenue[monthName] = totalForDay;
                        rowTotal += totalForDay;
                    }

                    // Add the row total to the last column of the data row
                    dataRow.Revenue["GrandTotal"] = rowTotal;

                    response.Add(dataRow);
                }

                // Grand Total row
                var grandTotalRow = new RevenuePerDayResponse
                {
                    Label = "Grand Total",
                    Revenue = new Dictionary<string, object>()
                };

                // Calculate the grand total for each month
                foreach (var monthName in monthNames)
                {
                    var currentMonth = new DateTime(DateTime.Now.Year, monthNames.ToList().FindIndex(m => m == monthName) + 1, 1);
                    var transactionsForMonth = _dbContext.Transactions
                        .Where(t => t.TransactionTime >= currentMonth && t.TransactionTime < currentMonth.AddMonths(1));

                    var totalForMonth = transactionsForMonth.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
                    grandTotalRow.Revenue[monthName] = totalForMonth;
                }

                response.Add(grandTotalRow);
            }
            else if (filter.RevenuPerDayFilter == (int)RevenuPerDayFilter.Yearly)
            {
                var startYear = DateTime.Now.Year - 4; // Last 5 years
                var endYear = DateTime.Now.Year;

                var monthNames = Enumerable.Range(1, 12)
                    .Select(month => new DateTime(2000, month, 1).ToString("MMM", CultureInfo.InvariantCulture));

                // Header row
                var headerRow = new RevenuePerDayResponse
                {
                    Label = "Year",
                    Revenue = new Dictionary<string, object>()
                };

                // Add month names to the header row
                foreach (var monthName in monthNames)
                {
                    headerRow.Revenue[monthName] = monthName;
                }
                headerRow.Revenue["GrandTotal"] = "Grand Total";

                response.Add(headerRow);

                // Data rows
                foreach (var year in Enumerable.Range(startYear, endYear - startYear + 1))
                {
                    var dataRow = new RevenuePerDayResponse
                    {
                        Label = year.ToString(),
                        Revenue = new Dictionary<string, object>()
                    };

                    // Iterate through each month to find the revenue for the current year and month
                    foreach (var monthName in monthNames)
                    {
                        var currentMonth = new DateTime(year, monthNames.ToList().FindIndex(m => m == monthName) + 1, 1);

                        var transactionsForMonth = _dbContext.Transactions
                            .AsEnumerable() // Switch to client evaluation
                            .Where(t => t.TransactionTime >= currentMonth && t.TransactionTime < currentMonth.AddMonths(1));

                        var totalForMonth = transactionsForMonth.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
                        dataRow.Revenue[monthName] = totalForMonth;
                    }

                    decimal rowTotal = 0;

                    // Calculate the row total
                    foreach (var monthName in monthNames)
                    {
                        var currentMonth = new DateTime(year, monthNames.ToList().FindIndex(m => m == monthName) + 1, 1);

                        var transactionsForMonth = _dbContext.Transactions
                            .AsEnumerable() // Switch to client evaluation
                            .Where(t => t.TransactionTime >= currentMonth && t.TransactionTime < currentMonth.AddMonths(1));

                        var totalForMonth = transactionsForMonth.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
                        dataRow.Revenue[monthName] = totalForMonth;
                        rowTotal += totalForMonth;
                    }

                    // Add the row total to the last column of the data row
                    dataRow.Revenue["GrandTotal"] = rowTotal;

                    response.Add(dataRow);
                }

                // Grand Total row
                var grandTotalRow = new RevenuePerDayResponse
                {
                    Label = "Grand Total",
                    Revenue = new Dictionary<string, object>()
                };

                // Calculate the grand total for each month
                foreach (var monthName in monthNames)
                {
                    var transactionsForMonth = _dbContext.Transactions
                        .AsEnumerable() // Switch to client evaluation
                        .Where(t => t.TransactionTime >= new DateTime(startYear, 1, 1) && t.TransactionTime < new DateTime(endYear + 1, 1, 1) && t.TransactionTime.Month == (monthNames.ToList().FindIndex(m => m == monthName) + 1));

                    var totalForMonth = transactionsForMonth.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
                    grandTotalRow.Revenue[monthName] = totalForMonth;
                }

                // Calculate the grand total for the entire year
                decimal grandTotalYear = 0;
                foreach (var year in Enumerable.Range(startYear, endYear - startYear + 1))
                {
                    var transactionsForYear = _dbContext.Transactions
                        .Where(t => t.TransactionTime >= new DateTime(year, 1, 1) && t.TransactionTime < new DateTime(year + 1, 1, 1));

                    var totalForYear = transactionsForYear.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
                    grandTotalYear += totalForYear;
                }

                // Add the grand total for the entire year in the last column of the grand total row
                grandTotalRow.Revenue["GrandTotal"] = grandTotalYear;

                // Add only the grand total for each month to the last row
                foreach (var monthName in monthNames)
                {
                    var transactionsForMonth = _dbContext.Transactions
                        .AsEnumerable() // Switch to client evaluation
                        .Where(t => t.TransactionTime >= new DateTime(startYear, 1, 1) && t.TransactionTime < new DateTime(endYear + 1, 1, 1) && t.TransactionTime.Month == (monthNames.ToList().FindIndex(m => m == monthName) + 1));

                    var totalForMonth = transactionsForMonth.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
                    grandTotalRow.Revenue[monthName] = totalForMonth;
                }

                response.Add(grandTotalRow);
            }
            return response;
        }

        public async Task<StatisticResponse> GetStatisticData(FilterRequest filter)
        {
            StatisticResponse response = new StatisticResponse();

            DateTime startDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;

            var dailyActiveUser = _dbContext.UserLoginHistory.Where(h => h.Date >= startDate && h.Date <= endDate);

            // Generate the list of DailyUser objects containing the count of active users for each hour within the selected date range
            response.DailyActiveUser = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .SelectMany(offset => Enumerable.Range(0, 24).Select(hour => startDate.AddDays(offset).AddHours(hour)))
                .GroupJoin(
                    dailyActiveUser,
                    date => date.Date,
                    loginHistory => loginHistory.Date.Date,
                    (date, loginHistoryGroup) => new DailyUser
                    {
                        Date = date.ToString("HH:mm"),
                        User = loginHistoryGroup.Select(loginHistory => loginHistory.UserID).Distinct().Count(),
                        Value = loginHistoryGroup.Count()
                    })
                .ToList();

            // Generate the list of DailyUser objects containing the count of active users and active minutes for each hour within the selected date range
            response.UserActiveMinutes = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .SelectMany(offset => Enumerable.Range(0, 24).Select(hour => startDate.AddDays(offset).AddHours(hour)))
                .GroupJoin(
                    dailyActiveUser,
                    date => date,
                    loginHistory => loginHistory.LoginTime,
                    (date, loginHistoryGroup) => new DailyUser
                    {
                        Date = date.ToString("HH:mm"),
                        User = loginHistoryGroup.Count(),
                        Value = loginHistoryGroup.Sum(loginHistory =>
                            (loginHistory.LogoutTime >= date && loginHistory.LoginTime <= date.AddHours(1))
                                ? (int)(loginHistory.LogoutTime - loginHistory.LoginTime).TotalMinutes
                                : 0
                        )
                    })
                .ToList();

            var users = _dbContext.Users.Where(u => u.CreatedAt >= startDate && u.CreatedAt <= endDate).Select(u => u);

            // Create an auxiliary list with all hours within the selected date range
            var allHoursInRange = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .SelectMany(offset => Enumerable.Range(0, 24).Select(hour => startDate.AddDays(offset).AddHours(hour)))
                .ToList();

            // Generate the list of DailyUser objects containing the count of new sign-ups for each hour within the selected date range
            response.NewSignUp = allHoursInRange
                .GroupJoin(
                    users.Where(u => u.CreatedAt.HasValue && u.CreatedAt.Value >= startDate && u.CreatedAt.Value <= endDate),
                    date => date.Hour,
                    user => user.CreatedAt.Value.Hour,
                    (date, userGroup) => new { Date = date, User = userGroup.FirstOrDefault() })
                .GroupBy(x => x.Date)
                .Select(g => new DailyUser
                {
                    Date = g.Key.ToString("HH:mm"),
                    User = g.Count(x => x.User != null),
                    Value = g.Count(x => x.User != null)
                })
            .ToList();

            var orders = _dbContext.Transactions.Select(o => o);

            decimal curSum = orders.Where(o => o.TransactionTime.Date == DateTime.Now.Date).Sum(o => o.Amount);
            decimal minAmount = orders.Min(o => o.Amount);
            decimal maxAmount = orders.Max(o => o.Amount);
            decimal avgAmount = orders.Average(o => o.Amount);

            // Calculate the sum of amounts for each date within the selected date range
            var dateSumQuery = orders
                .Where(o => o.TransactionTime >= filter.StartDate && o.TransactionTime <= filter.EndDate)
                .GroupBy(o => o.TransactionTime.Date)
                .Select(g => new { Date = g.Key, Sum = g.Sum(o => o.Amount) });

            // Generate the list of OrderData objects containing the sum of amounts and average for each date within the selected date range
            List<OrderData> orderDataList = dateSumQuery.Select(item => new OrderData
            {
                Date = item.Date.ToString("yyyy-MM-dd"),
                Price = item.Sum,
                Average = dateSumQuery
                .Where(i => i.Date == item.Date)
                .Select(i => i.Sum)
                .SingleOrDefault()
            }).ToList();

            OrderStats orderStats = new OrderStats
            {
                Cur = curSum,
                Avg = avgAmount,
                Min = minAmount,
                Max = maxAmount,
                Data = orderDataList
            };
            response.TotalOrder = orderStats;

            var itemRevenues = from i in _dbContext.Items
                               join oi in _dbContext.OrderItems on i.ID equals oi.ItemID
                               join t in _dbContext.Transactions on oi.OrderID equals t.OrderID
                               join c in _dbContext.Categories on i.CategoryID equals c.ID
                               join sc in _dbContext.SubCategories on c.ID equals sc.CategoryId
                               where t.Status == (int)TransactionStatus.Paid
                               select new
                               {
                                   i.Name,
                                   c.CategoryName,
                                   sc.SubCategoryName,
                                   Revenue = t.DriverCommision + t.ProviderCommision + t.ServiceFee,
                                   Date = t.TransactionTime
                               };

            // Specify the current date
            DateTime currentDate = DateTime.Today;

            // Calculate the current date revenue (Cur)
            decimal curRevenue = itemRevenues
                .Where(ir => ir.Date.Date == currentDate)
                .Sum(ir => ir.Revenue);

            // Calculate the overall average, minimum, and maximum revenues
            decimal avgRevenue = itemRevenues.Average(ir => ir.Revenue);
            decimal minRevenue = itemRevenues.Min(ir => ir.Revenue);
            decimal maxRevenue = itemRevenues.Max(ir => ir.Revenue);

            // Calculate the average revenue for each item
            var itemAverageRevenues = itemRevenues
                .Where(t => t.Date >= filter.StartDate && t.Date <= filter.EndDate)
                .GroupBy(ir => ir.Name)
                .Select(g => new { Name = g.Key, AverageRevenue = g.Average(ir => ir.Revenue) })
                .OrderByDescending(item => item.AverageRevenue)
                .Take(10)
                .ToList();

            // Calculate the average revenue for each category
            var categoryAverageRevenues = itemRevenues
                .Where(t => t.Date >= filter.StartDate && t.Date <= filter.EndDate)
                .GroupBy(ir => ir.CategoryName)
                .Select(g => new { Name = g.Key, AverageRevenue = g.Average(ir => ir.Revenue) })
                .OrderByDescending(category => category.AverageRevenue)
                .Take(10)
                .ToList();

            // Calculate the average revenue for each subcategory
            var subCategoryAverageRevenues = itemRevenues
                .Where(t => t.Date >= filter.StartDate && t.Date <= filter.EndDate)
                .GroupBy(ir => ir.SubCategoryName)
                .Select(g => new { Name = g.Key, AverageRevenue = g.Average(ir => ir.Revenue) })
                .OrderByDescending(subCategory => subCategory.AverageRevenue)
                .Take(10)
                .ToList();

            response.TopItem = new GeneralTopStats
            {
                Cur = curRevenue,
                Avg = avgRevenue,
                Min = minRevenue,
                Max = maxRevenue,
                Labels = itemAverageRevenues.Select(item => item.Name).ToList(),
                Values = itemAverageRevenues.Select(item => item.AverageRevenue).ToList()
            };

            response.TopCategory = new GeneralTopStats
            {
                Cur = curRevenue,
                Avg = avgRevenue,
                Min = minRevenue,
                Max = maxRevenue,
                Labels = categoryAverageRevenues.Select(category => category.Name).ToList(),
                Values = categoryAverageRevenues.Select(category => category.AverageRevenue).ToList()
            };

            response.TopSubCategory = new GeneralTopStats
            {
                Cur = curRevenue,
                Avg = avgRevenue,
                Min = minRevenue,
                Max = maxRevenue,
                Labels = subCategoryAverageRevenues.Select(subCategory => subCategory.Name).ToList(),
                Values = subCategoryAverageRevenues.Select(subCategory => subCategory.AverageRevenue).ToList()
            };

            var userPurchaseAmounts = from u in _dbContext.Users
                                      join o in _dbContext.Orders on u.ID equals o.UserID
                                      join t in _dbContext.Transactions on o.ID equals t.OrderID
                                      where t.Status == (int)TransactionStatus.Paid
                                      group t by new { u.ID, Name = u.FirstName } into userGroup
                                      select new
                                      {
                                          UserID = userGroup.Key.ID,
                                          UserName = userGroup.Key.Name,
                                          TotalPurchaseAmount = userGroup.Sum(t => t.Amount),
                                          TransactionDate = userGroup.Max(t => t.TransactionTime).Date
                                      };

            // Calculate the current date revenue (Cur) for top users
            curRevenue = userPurchaseAmounts
                .Where(upa => upa.TransactionDate == currentDate)
                .Sum(upa => upa.TotalPurchaseAmount);

            // Calculate the overall average, minimum, and maximum purchase amounts
            decimal avgPurchaseAmount = userPurchaseAmounts.Average(upa => upa.TotalPurchaseAmount);
            decimal minPurchaseAmount = userPurchaseAmounts.Min(upa => upa.TotalPurchaseAmount);
            decimal maxPurchaseAmount = userPurchaseAmounts.Max(upa => upa.TotalPurchaseAmount);

            // Select the top 10 users based on their total purchase amount
            var topUsers = userPurchaseAmounts
                .Where(t => t.TransactionDate >= filter.StartDate && t.TransactionDate <= filter.EndDate)
                .OrderByDescending(upa => upa.TotalPurchaseAmount)
                .Take(10)
                .ToList();

            // Create the GeneralTopStats response class
            response.TopBuyer = new GeneralTopStats
            {
                Cur = curRevenue,
                Avg = avgPurchaseAmount,
                Min = minPurchaseAmount,
                Max = maxPurchaseAmount,
                Labels = topUsers.Select(user => user.UserName).ToList(),
                Values = topUsers.Select(user => user.TotalPurchaseAmount).ToList()
            };

            var userTransactionCounts = from u in _dbContext.Users
                                        join o in _dbContext.Orders on u.ID equals o.UserID
                                        join t in _dbContext.Transactions on o.ID equals t.OrderID
                                        group t by new { u.ID, Name = u.FirstName } into userGroup
                                        select new
                                        {
                                            UserID = userGroup.Key.ID,
                                            UserName = userGroup.Key.Name,
                                            TransactionCount = userGroup.Count(),
                                            TransactionDate = userGroup.Max(t => t.TransactionTime).Date
                                        };

            // Calculate the current date transaction count (Cur) for top users
            int curTransactionCount = userTransactionCounts
                .Where(utc => utc.TransactionDate.Date == currentDate)
                .Sum(utc => utc.TransactionCount);

            // Calculate the overall average, minimum, and maximum transaction counts
            decimal avgTransactionCount = (decimal)userTransactionCounts.Average(utc => utc.TransactionCount);
            decimal minTransactionCount = userTransactionCounts.Min(utc => utc.TransactionCount);
            decimal maxTransactionCount = userTransactionCounts.Max(utc => utc.TransactionCount);

            // Select the top 10 users based on their transaction count
            var topUserscount = userTransactionCounts
                .Where(t => t.TransactionDate >= filter.StartDate && t.TransactionDate <= filter.EndDate)
                .OrderByDescending(utc => utc.TransactionCount)
                .Take(10)
                .ToList();

            // Create the GeneralTopStats response class
            response.TopBuyerCount = new GeneralTopStats
            {
                Cur = curTransactionCount,
                Avg = avgTransactionCount,
                Min = minTransactionCount,
                Max = maxTransactionCount,
                Labels = topUserscount.Select(user => user.UserName).ToList(),
                Values = topUserscount.Select(user => Convert.ToDecimal(user.TransactionCount)).ToList()
            };

            var providerMetrics = from p in _dbContext.Providers
                                  join po in _dbContext.ProviderOrders on p.ID equals po.ProviderID
                                  join t in _dbContext.Transactions on po.OrderID equals t.OrderID
                                  join u in _dbContext.Users on p.UserID equals u.ID
                                  where t.Status == (int)TransactionStatus.Paid
                                  group t by new { p.ID, u.FirstName } into providerGroup
                                  select new
                                  {
                                      ProviderID = providerGroup.Key.ID,
                                      Name = providerGroup.Key.FirstName,
                                      Sale = providerGroup.Sum(t => t.Amount),
                                      Count = providerGroup.Count(),
                                      TransactionDate = providerGroup.Max(t => t.TransactionTime).Date
                                  };

            decimal curSale = providerMetrics
            .Where(pm => pm.TransactionDate.Date == currentDate)
            .Sum(pm => pm.Sale);

            // Calculate the current date transaction count (Cur) for top providers by counts
            int curCount = providerMetrics
                .Where(pm => pm.TransactionDate.Date == currentDate)
                .Sum(pm => pm.Count);

            // Calculate the overall average, minimum, and maximum sales and counts
            decimal avgSale = providerMetrics.Average(pm => pm.Sale);
            decimal minSale = providerMetrics.Min(pm => pm.Sale);
            decimal maxSale = providerMetrics.Max(pm => pm.Sale);

            double avgCount = providerMetrics.Average(pm => pm.Count);
            decimal minCount = providerMetrics.Min(pm => pm.Count);
            decimal maxCount = providerMetrics.Max(pm => pm.Count);

            // Select the top providers based on sales (sum of transaction amounts)
            var topProvidersBySale = providerMetrics
                .Where(t => t.TransactionDate >= filter.StartDate && t.TransactionDate <= filter.EndDate)
                .OrderByDescending(pm => pm.Sale)
                .Take(10)
                .ToList();

            // Select the top providers based on counts (number of transactions)
            var topProvidersByCount = providerMetrics
                .Where(t => t.TransactionDate >= filter.StartDate && t.TransactionDate <= filter.EndDate)
                .OrderByDescending(pm => pm.Count)
                .Take(10)
                .ToList();

            // Create the GeneralTopStats response classes for sales and counts
            response.TopProvider = new GeneralTopStats
            {
                Cur = curSale,
                Avg = avgSale,
                Min = minSale,
                Max = maxSale,
                Labels = topProvidersBySale.Select(pm => pm.Name.ToString()).ToList(),
                Values = topProvidersBySale.Select(pm => pm.Sale).ToList()
            };

            response.TopProviderCount = new GeneralTopStats
            {
                Cur = curCount,
                Avg = (decimal)avgCount,
                Min = minCount,
                Max = maxCount,
                Labels = topProvidersByCount.Select(pm => pm.Name.ToString()).ToList(),
                Values = topProvidersByCount.Select(pm => Convert.ToDecimal(pm.Count)).ToList()
            };

            var driverMetrics = from d in _dbContext.Drivers
                                join dor in _dbContext.DriverOrders on d.ID equals dor.DriverID
                                join t in _dbContext.Transactions on dor.OrderID equals t.OrderID
                                join u in _dbContext.Users on d.UserID equals u.ID
                                group t by new { d.ID, u.FirstName } into driverGroup
                                select new
                                {
                                    DriverID = driverGroup.Key.ID,
                                    Name = driverGroup.Key.FirstName,
                                    DeliveryCharges = driverGroup.Sum(t => t.DeliveryCharges),
                                    Count = driverGroup.Count(),
                                    TransactionDate = driverGroup.Max(t => t.TransactionTime).Date
                                };

            decimal curDeliveryCharges = driverMetrics
            .Where(dm => dm.TransactionDate.Date == currentDate)
            .Sum(dm => dm.DeliveryCharges);

            // Calculate the current date transaction count (Cur) for top providers by counts
            curCount = driverMetrics
                .Where(dm => dm.TransactionDate.Date == currentDate)
                .Sum(dm => dm.Count);

            // Calculate the overall average, minimum, and maximum delivery charges and counts
            decimal avgDeliveryCharges = driverMetrics.Average(dm => dm.DeliveryCharges);
            decimal minDeliveryCharges = driverMetrics.Min(dm => dm.DeliveryCharges);
            decimal maxDeliveryCharges = driverMetrics.Max(dm => dm.DeliveryCharges);

            avgCount = driverMetrics.Average(dm => dm.Count);
            minCount = driverMetrics.Min(dm => dm.Count);
            maxCount = driverMetrics.Max(dm => dm.Count);

            // Select the top drivers based on delivery charges (sum of delivery charges)
            var topDriversByDeliveryCharges = driverMetrics
                .Where(t => t.TransactionDate >= filter.StartDate && t.TransactionDate <= filter.EndDate)
                .OrderByDescending(dm => dm.DeliveryCharges)
                .Take(10)
                .ToList();

            // Select the top drivers based on counts (number of transactions)
            var topDriversByCount = driverMetrics
                .Where(t => t.TransactionDate >= filter.StartDate && t.TransactionDate <= filter.EndDate)
                .OrderByDescending(dm => dm.Count)
                .Take(10)
                .ToList();

            // Create the GeneralTopStats response classes for delivery charges and counts
            response.TopDriver = new GeneralTopStats
            {
                Cur = curDeliveryCharges,
                Avg = avgDeliveryCharges,
                Min = minDeliveryCharges,
                Max = maxDeliveryCharges,
                Labels = topDriversByDeliveryCharges.Select(dm => dm.Name.ToString()).ToList(),
                Values = topDriversByDeliveryCharges.Select(dm => dm.DeliveryCharges).ToList()
            };

            response.TopDriverCount = new GeneralTopStats
            {
                Cur = curCount,
                Avg = (decimal)avgCount,
                Min = minCount,
                Max = maxCount,
                Labels = topDriversByCount.Select(dm => dm.Name.ToString()).ToList(),
                Values = topDriversByCount.Select(dm => Convert.ToDecimal(dm.Count)).ToList()
            };

            response.LatestOrders = await (from o in _dbContext.Orders
                                           join u in _dbContext.Users on o.UserID equals u.ID
                                           join pr in _dbContext.ProviderOrders on o.ID equals pr.OrderID
                                           join p in _dbContext.Providers on pr.ProviderID equals p.ID
                                           join pu in _dbContext.Users on p.UserID equals pu.ID
                                           join dor in _dbContext.DriverOrders on o.ID equals dor.OrderID into driverOrderTemp
                                           from dot in driverOrderTemp.DefaultIfEmpty()
                                           join d in _dbContext.Drivers on dot.DriverID equals d.ID into driverTemp
                                           from dt in driverTemp.DefaultIfEmpty()
                                           join du in _dbContext.Users on dt.UserID equals du.ID into driverUserTemp
                                           from dut in driverUserTemp.DefaultIfEmpty()
                                           select new LatestOrder
                                           {
                                               OrderId = o.ID,
                                               UserID = u.ID,
                                               ProviderUserID = pu.ID,
                                               DriverUserID = dut != null ? dut.ID : 0,
                                               Buyer = $"{u.FirstName} {u.LastName}",
                                               Driver = $"{dut.FirstName} {dut.LastName}",
                                               Provider = $"{pu.FirstName} {pu.LastName}",
                                               Date = o.CreatedAt.Value.Date.ToString(),
                                               Status = ((OrderStatus)o.Status).ToString()
                                           })
                                      .OrderByDescending(o => o.Date)
                                      .Take(10)
                                      .ToListAsync();

            var transactions = _dbContext.Transactions.Where(t => t.Status == (int)TransactionStatus.Paid).Select(t => t);

            curRevenue = transactions
            .Where(t => t.TransactionTime.Date == currentDate)
            .Sum(t => t.Amount + t.DeliveryCharges);

            decimal curCommission = transactions
                .Where(t => t.TransactionTime.Date == currentDate)
                .Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);

            // Calculate the overall average, minimum, and maximum revenue and commission
            avgRevenue = transactions.Average(t => t.Amount + t.DeliveryCharges);
            minRevenue = transactions.Min(t => t.Amount + t.DeliveryCharges);
            maxRevenue = transactions.Max(t => t.Amount + t.DeliveryCharges);

            decimal avgCommission = transactions.Average(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
            decimal minCommission = transactions.Min(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);
            decimal maxCommission = transactions.Max(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee);

            // Group transactions by date and calculate revenue and commission for each date
            var perDayData = transactions
                .Where(t => t.TransactionTime >= filter.StartDate && t.TransactionTime <= filter.EndDate)
                .GroupBy(t => t.TransactionTime.Date)
                .Select(group => new RevenueVsCommisionData
                {
                    date = group.Key.ToString("yyyy-MM-dd"),
                    Revenue = group.Sum(t => (int)(t.Amount + t.DeliveryCharges)),
                    Commision = group.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee)
                }).ToList();

            // Create the RevenueVsCommision response class
            response.RevenueVsCommisions = new Shared.Response.Analytic.RevenueVsCommision
            {
                Cur = curCommission,
                Avg = avgCommission,
                Min = minCommission,
                Max = maxCommission,
                Data = perDayData
            };

            var usersQuery = _dbContext.Users
                .Where(t => t.CreatedAt >= filter.StartDate && t.CreatedAt <= filter.EndDate);

            var driversQuery = _dbContext.Drivers
                .Where(t => t.CreatedAt >= filter.StartDate && t.CreatedAt <= filter.EndDate);

            var providersQuery = _dbContext.Providers
                .Where(t => t.CreatedAt >= filter.StartDate && t.CreatedAt <= filter.EndDate);

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

            response.TotalVsOnlines = response2;

            return response;
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

        public async Task<FinanceResponse> GetFinanceData(FilterRequest filter)
        {
            FinanceResponse financeResponse = new FinanceResponse();

            var list = from t in _dbContext.Transactions
                       select t;

            list = list.Where(t => t.TransactionTime >= filter.StartDate && t.TransactionTime <= filter.EndDate);

            if (!string.IsNullOrEmpty(filter.Gender))
            {
                list = list.Join(_dbContext.Users, t => t.UserID, u => u.ID, (t, u) => new { Transaction = t, User = u })
                           .Where(tu => tu.User.Gender == filter.Gender)
                           .Select(tu => tu.Transaction);
            }

            if (filter.UserType == (int)UserType.Buyer)
            {
                list = list.GroupJoin(_dbContext.Drivers, t => t.UserID, d => d.UserID, (t, drivers) => new { Transaction = t, Drivers = drivers.DefaultIfEmpty() })
                                            .GroupJoin(_dbContext.Providers, t => t.Transaction.UserID, p => p.UserID, (t, providers) => new { Transaction = t.Transaction, Drivers = t.Drivers, Providers = providers.DefaultIfEmpty() })
                                            .Where(t => t.Drivers.All(d => d == null) && t.Providers.All(p => p == null))
                                            .Select(t => t.Transaction);
            }
            else if (filter.UserType == (int)UserType.Provider)
            {
                list = list.Join(_dbContext.Providers, t => t.UserID, p => p.UserID, (t, p) => t);
            }
            else if (filter.UserType == (int)UserType.Driver)
            {
                list = list.Join(_dbContext.Drivers, t => t.UserID, d => d.UserID, (t, d) => t);
            }

            if (filter.CategoryID > 0)
            {
                list = list.Join(_dbContext.Providers, t => t.UserID, p => p.UserID, (t, p) => new { Transaction = t, Provider = p })
                           .Join(_dbContext.Items, tp => tp.Provider.ID, i => i.ProviderID, (tp, i) => new { Transaction = tp.Transaction, Item = i })
                           .Where(t => t.Item.CategoryID == filter.CategoryID &&
                                       (filter.SubCategoryID == 0 || t.Item.SubCategoryID == filter.SubCategoryID))
                           .Select(t => t.Transaction);
            }

            financeResponse.TotalTransaction = await list.CountAsync();
            financeResponse.TotalRevenu = list.Sum(t => t.Amount + t.DeliveryCharges + t.ProviderCommision + t.DriverCommision + t.ServiceFee);
            financeResponse.TotalCommision = list.Sum(t => t.ProviderCommision + t.DriverCommision);

            var groupedTransactions = list.GroupBy(t => t.TransactionTime.Date)
                                          .Select(group => new
                                          {
                                              Date = group.Key,
                                              SumAmount = group.Sum(t => t.Amount),
                                              AvgAmount = group.Average(t => t.Amount),
                                              MinAmount = group.Min(t => t.Amount),
                                              MaxAmount = group.Max(t => t.Amount),
                                              SumCommision = group.Sum(t => t.ProviderCommision + t.DriverCommision),
                                              AvgCommision = group.Average(t => t.ProviderCommision + t.DriverCommision),
                                              MinCommision = group.Min(t => t.ProviderCommision + t.DriverCommision),
                                              MaxCommision = group.Max(t => t.ProviderCommision + t.DriverCommision),
                                              SumDriverCommision = group.Sum(t => t.DriverCommision),
                                              AvgDriverCommision = group.Average(t => t.DriverCommision),
                                              SumProviderCommision = group.Sum(t => t.ProviderCommision),
                                              AvgProviderCommision = group.Average(t => t.ProviderCommision),
                                              SumSale = group.Sum(t => t.Amount + t.DeliveryCharges),
                                              AvgSale = group.Average(t => t.Amount + t.DeliveryCharges),
                                              MinSale = group.Min(t => t.Amount + t.DeliveryCharges),
                                              MaxSale = group.Max(t => t.Amount + t.DeliveryCharges),
                                              SumOrderSale = group.Sum(t => t.Amount),
                                              AvgOrderSale = group.Average(t => t.Amount),
                                              SumDeliverySale = group.Sum(t => t.DeliveryCharges),
                                              AvgDeliverySale = group.Average(t => t.DeliveryCharges),
                                              SumWallet = group.Sum(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee),
                                              AvgWallet = group.Average(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee),
                                              MinWallet = group.Min(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee),
                                              MaxWallet = group.Max(t => t.ProviderCommision + t.DriverCommision + t.ServiceFee)
                                          });

            financeResponse.AverageOrder = CreateGeneralData(groupedTransactions, DateTime.Today, tx => tx.SumAmount, tx => tx.AvgAmount, tx => tx.MinAmount, tx => tx.MaxAmount);
            financeResponse.CumulativeCommision = CreateGeneralData(groupedTransactions, DateTime.Today, tx => tx.SumCommision, tx => tx.AvgCommision, tx => tx.MinCommision, tx => tx.MaxCommision);
            financeResponse.Commision = new CommisionData
            {
                Driver = CreateGraphDetail(groupedTransactions, tx => tx.SumDriverCommision, tx => tx.AvgDriverCommision),
                Provider = CreateGraphDetail(groupedTransactions, tx => tx.SumProviderCommision, tx => tx.AvgProviderCommision)
            };
            financeResponse.CumulativeSale = CreateGeneralData(groupedTransactions, DateTime.Today, tx => tx.SumSale, tx => tx.AvgSale, tx => tx.MinSale, tx => tx.MaxSale);
            financeResponse.Sale = new SaleData
            {
                Order = CreateGraphDetail(groupedTransactions, tx => tx.SumOrderSale, tx => tx.AvgOrderSale),
                Delivery = CreateGraphDetail(groupedTransactions, tx => tx.SumDeliverySale, tx => tx.AvgDeliverySale)
            };
            financeResponse.CumulativeWallet = CreateGeneralData(groupedTransactions, DateTime.Today, tx => tx.SumWallet, tx => tx.AvgWallet, tx => tx.MinWallet, tx => tx.MaxWallet);
            financeResponse.Wallet = new WalletData
            {
                User = CreateGraphDetail(groupedTransactions, tx => tx.SumWallet, tx => tx.AvgWallet),
                Baetoti = CreateGraphDetail(groupedTransactions, tx => tx.SumWallet, tx => tx.AvgWallet)
            };

            return financeResponse;
        }

        private GeneralData CreateGeneralData
            (
                IEnumerable<dynamic> groupedTransactions, DateTime today,
                Func<dynamic, decimal> sumSelector, Func<dynamic, decimal> avgSelector,
                Func<dynamic, decimal> minSelector, Func<dynamic, decimal> maxSelector
            )
        {
            decimal curAmount = groupedTransactions.FirstOrDefault(tx => tx.Date == today)?.SumAmount ?? 0;
            decimal avgAmount = groupedTransactions.Average(avgSelector);
            decimal minAmount = groupedTransactions.Min(minSelector);
            decimal maxAmount = groupedTransactions.Max(maxSelector);

            return new GeneralData
            {
                Cur = curAmount,
                Avg = avgAmount,
                Min = minAmount,
                Max = maxAmount,
                GraphData = CreateGraphData(groupedTransactions, sumSelector, avgSelector)
            };
        }

        private GraphDetail CreateGraphDetail(IEnumerable<dynamic> groupedTransactions, Func<dynamic, decimal> sumSelector, Func<dynamic, decimal> avgSelector)
        {
            var graphData = CreateGraphData(groupedTransactions, sumSelector, avgSelector);
            return new GraphDetail
            {
                Total = graphData.Sum(c => c.Value),
                GraphData = graphData
            };
        }

        private List<GeneralGraphData> CreateGraphData(IEnumerable<dynamic> groupedTransactions, Func<dynamic, decimal> sumSelector, Func<dynamic, decimal> avgSelector)
        {
            return groupedTransactions.Select(tx => new GeneralGraphData
            {
                Date = tx.Date.ToString("MMM dd"),
                Value = sumSelector(tx),
                Avg = avgSelector(tx)
            }).ToList();
        }

        public async Task<List<CohortResponse>> GetCohortData(CohortFilter filter)
        {
            List<CohortResponse> cohortAnalysisResults = new List<CohortResponse>();

            // Create a list of all months for the year
            List<CohortResponse> allMonths = new List<CohortResponse>();
            DateTime currentDate = DateTime.Now;

            for (int i = 0; i < 12; i++)
            {
                var month = currentDate.AddMonths(-i);
                allMonths.Add(new CohortResponse
                {
                    Year = month.Year,
                    Month = month.Month,
                    MonthLabel = month.ToString("MMM"),
                    SignUps = 0,
                    SignIns = 0
                });
            }

            // Fetch and process sign-up counts
            var signUpCounts = await _dbContext.Users
                .Where(u => u.CreatedAt != null)
                .GroupBy(u => new { u.CreatedAt.Value.Year, u.CreatedAt.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    SignUps = g.Count()
                })
                .ToListAsync();

            foreach (var signUp in signUpCounts)
            {
                var month = allMonths.FirstOrDefault(x => x.Year == signUp.Year && x.Month == signUp.Month);
                if (month != null)
                {
                    month.SignUps = signUp.SignUps;
                }
            }

            // Fetch and process sign-in counts
            var signInCounts = await _dbContext.UserLoginHistory
                .GroupBy(u => new { u.Date.Year, u.Date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    SignIns = g.Count()
                })
                .ToListAsync();

            foreach (var signIn in signInCounts)
            {
                var month = allMonths.FirstOrDefault(x => x.Year == signIn.Year && x.Month == signIn.Month);
                if (month != null)
                {
                    month.SignIns = signIn.SignIns;
                }
            }

            // Generate the cohort analysis response in tabular format
            CohortResponse header = new CohortResponse
            {
                MonthLabel = "Month",
                CohortValue = new List<string> { "CohortValue" }
            };

            for (int i = 1; i <= 12; i++)
            {
                header.CohortValue.Add(i.ToString());
            }

            cohortAnalysisResults.Add(header);

            foreach (var month in allMonths)
            {
                CohortResponse result = new CohortResponse
                {
                    MonthLabel = month.MonthLabel,
                    CohortValue = new List<string> { month.SignUps.ToString() }
                };

                for (int i = 1; i <= 12; i++)
                {
                    int cohortData = month.SignIns;
                    if (i < month.Month)
                    {
                        cohortData = 0; // No sign-ins for months before the sign-up month
                    }
                    result.CohortValue.Add(cohortData.ToString());
                }

                cohortAnalysisResults.Add(result);
            }

            return cohortAnalysisResults;
        }

    }
}
