using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Helper;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Provider;
using Baetoti.Shared.Response.Shared;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class ProviderRepository : EFRepository<Provider>, IProviderRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public ProviderRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<GetAllProviderResponse>> GetAllAsync()
        {
            return await (from p in _dbContext.Providers
                          join u in _dbContext.Users on p.UserID equals u.ID
                          join s in _dbContext.Stores on p.ID equals s.ProviderID
                          select new GetAllProviderResponse
                          {
                              ID = p.ID,
                              Name = $"{u.FirstName} {u.LastName}",
                              StoreName = s.Name,
                              Location = s.Location
                          }).ToListAsync();
        }

        public async Task<PaginationResponse> GetAllOnlineAsync(int PageNumber, int PageSize)
        {
            var query = (from p in _dbContext.Providers
                         where p.IsOnline == true && p.IsPublic == true
                         select p).Distinct();

            var totalRecords = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            var pagedResults = await (from p in query
                                      join u in _dbContext.Users on p.UserID equals u.ID
                                      join s in _dbContext.Stores on p.ID equals s.ProviderID
                                      select new GetAllProviderResponse
                                      {
                                          ID = p.ID,
                                          Name = $"{u.FirstName} {u.LastName}",
                                          StoreName = s.Name,
                                          Location = s.Location
                                      })
                                      .Skip((PageNumber - 1) * PageSize)
                                      .Take(PageSize)
                                      .ToListAsync();
            return new PaginationResponse
            {
                CurrentPage = PageNumber,
                TotalPages = totalPages,
                PageSize = PageSize,
                TotalCount = totalRecords,
                Data = pagedResults
            };
        }

        public async Task<Provider> GetByUserID(long UserID)
        {
            return await _dbContext.Providers.Where(x => x.UserID == UserID).FirstOrDefaultAsync();
        }

        public async Task<ProviderResponse> GetDetailByUserID(long UserID)
        {
            return await (from p in _dbContext.Providers
                          join u in _dbContext.Users on p.UserID equals u.ID
                          where p.UserID == UserID
                          select new ProviderResponse
                          {
                              UserID = u.ID,
                              ProviderID = p.ID,
                              MaroofID = p.MaroofID,
                              MaroofLink = p.MaroofLink,
                              GovernmentID = p.GovernmentID,
                              GovernmentIDPicture = p.GovernmentIDPicture,
                              ExpirationDate = p.ExpirationDate,
                              CreatedAt = p.CreatedAt,
                              Email = u.Email,
                              Name = $"{u.FirstName} {u.LastName}",
                              Address = u.Address,
                              IsPublic = p.IsPublic,
                              IsOnline = p.IsOnline,
                              Phone = u.Phone,
                              ProfileImage = u.Picture
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<ProviderUserlatlngResponse>> GetUserlatlng(double lat, double lng)
        {
            var users = _dbContext.Users
                        .AsEnumerable()
                        .Where(u => Helper.GetDistance(
                           Latitud1: lat,
                           Longitude1: lng,
                           Latitude2: u.Latitude,
                           Longitude2: u.Longitude) >= 10).Select(ss => new ProviderUserlatlngResponse { UserID = ss.ID.ToString(), UserType = Shared.Enum.UserType.Buyer }).ToList();


            return users;
        }

        public async Task<ProviderAndBusinessResponse> GetProviderBusinessDetailByUserID(long UserID)
        {
            return await (from p in _dbContext.Providers
                          join u in _dbContext.Users on p.UserID equals u.ID
                          join pb in _dbContext.ProviderBusiness on p.ID equals pb.ProviderID
                          join c in _dbContext.Countries on p.CountryId equals c.ID
                          join r in _dbContext.Regions on p.RegionId equals r.Id
                          join ct in _dbContext.Cities on p.CityId equals ct.Id
                          where p.UserID == UserID
                          select new ProviderAndBusinessResponse
                          {
                              UserID = u.ID,
                              ProviderID = p.ID,
                              MaroofID = p.MaroofID,
                              MaroofLink = p.MaroofLink,
                              GovernmentID = p.GovernmentID,
                              GovernmentIDPicture = p.GovernmentIDPicture,
                              ExpirationDate = p.ExpirationDate,
                              ProviderCreatedAt = p.CreatedAt,
                              Email = u.Email,
                              FirstName = u.FirstName,
                              LastName = u.LastName,
                              Name = $"{u.FirstName} {u.LastName}",
                              Address = u.Address,
                              IsPublic = p.IsPublic,
                              IsOnline = p.IsOnline,
                              Phone = u.Phone,
                              ProfileImage = u.Picture,
                              BussinessFirstName = pb.FirstName,
                              BusinessMiddleName = pb.MiddleName,
                              BusinessLastName = pb.LastName,
                              BusinessName = $"{pb.FirstName} {pb.MiddleName} {pb.LastName}",
                              IsCorpoarate = pb.IsCorpoarate,
                              ProviderBusinessCreatedAt = pb.CreatedAt,
                              ProviderBusinessLastUpdatedAt = pb.LastUpdatedAt,
                              Signature = pb.Signature,
                              IsCivilID = pb.IsCivilID,
                              CopyofIDOrPassport = pb.CopyofIDOrPassport,
                              OwnerAdrress = pb.OwnerAdrress,
                              CommercialRegistrationLicense = pb.CommercialRegistrationLicense,
                              CompanyAddress = pb.CompanyAddress,
                              VATRegistrationCertificate = pb.VATRegistrationCertificate,
                              FreelanceCertificate = pb.FreelanceCertificate,
                              IBANNumber = pb.IBANNumber,
                              BankAccountName = pb.BankAccountName,
                              BankAccountNumber = pb.BankAccountNumber,
                              BeneficiaryAddress = pb.BeneficiaryAddress,
                              SwiftCode = pb.SwiftCode,
                              CommercialExpiry = pb.CommercialExpiry,
                              CommercialNumber = pb.CommercialNumber,
                              VATRegistrationNumber = pb.VATRegistrationNumber,
                              BankAccountCertificate = pb.BankAccountCertificate,
                              CountryID = p.CountryId,
                              RegionID = p.RegionId,
                              CityID = p.CityId,
                              CountryName = c.CountryName,
                              RegionName = r.NameAr,
                              CityName = ct.NameAr,
                              TapCompanyStoreName = pb.TapCompanyStoreName
                          }).FirstOrDefaultAsync();
        }

        public async Task<ProviderResponse> GetDetailByProviderID(long ProviderID)
        {
            return await (from p in _dbContext.Providers
                          join u in _dbContext.Users on p.UserID equals u.ID
                          where p.ID == ProviderID
                          select new ProviderResponse
                          {
                              UserID = u.ID,
                              ProviderID = p.ID,
                              MaroofID = p.MaroofID,
                              MaroofLink = p.MaroofLink,
                              GovernmentID = p.GovernmentID,
                              GovernmentIDPicture = p.GovernmentIDPicture,
                              ExpirationDate = p.ExpirationDate,
                              CreatedAt = p.CreatedAt,
                              Email = u.Email,
                              Name = $"{u.FirstName} {u.LastName}",
                              Address = u.Address,
                              IsPublic = p.IsPublic,
                              IsOnline = p.IsOnline,
                              Phone = u.Phone,
                              ProfileImage = u.Picture
                          }).FirstOrDefaultAsync();
        }

        public async Task<ProviderDashboardResponse> GetDashboardData(long UserID)
        {
            var orders = from u in _dbContext.Users
                         where u.ID == UserID
                         join p in _dbContext.Providers on u.ID equals p.UserID
                         join po in _dbContext.ProviderOrders on p.ID equals po.ProviderID
                         select po;

            var today = DateTime.Now.ToTimeZoneTime("Arab Standard Time").Date;
            ProviderDashboardResponse response = new ProviderDashboardResponse();
            response.AvailableBalance = 100;
            response.TotalOrder = await orders.CountAsync();
            response.TodaysOrder = await orders.CountAsync(x => x.CreatedAt.Date == today);

            return response;
        }

        public async Task<GraphDataResponse> GetSalesGraphData(int SaleGraphFilter, long UserID)
        {
            DateTime startDate;
            DateTime endDate;

            if (SaleGraphFilter == 1) // By Week
            {
                startDate = DateTime.Today.AddDays(-7);
                endDate = DateTime.Today.AddDays(1);

                var providerOrdersQuery = _dbContext.Users
                    .Where(u => u.ID == UserID)
                    .Join(
                        _dbContext.Providers,
                        u => u.ID,
                        p => p.UserID,
                        (u, p) => new { User = u, Provider = p }
                    )
                    .Join(
                        _dbContext.ProviderOrders,
                        up => up.Provider.ID,
                        po => po.ProviderID,
                        (up, po) => new { UserProvider = up, ProviderOrder = po }
                    )
                    .Where(upo => upo.ProviderOrder.CreatedAt >= startDate && upo.ProviderOrder.CreatedAt <= endDate)
                    .Select(upo => upo.ProviderOrder);

                var accountVisitsQuery = _dbContext.Users
                    .Where(u => u.ID == UserID)
                    .Join(
                        _dbContext.Providers,
                        u => u.ID,
                        p => p.UserID,
                        (u, p) => new { User = u, Provider = p }
                    )
                    .Join(
                        _dbContext.AccountVisits,
                        up => up.Provider.ID,
                        av => av.ProviderID,
                        (up, av) => new { UserProvider = up, AccountVisit = av }
                    )
                    .Where(upav => upav.AccountVisit.FirstVisitDate >= startDate && upav.AccountVisit.FirstVisitDate <= endDate)
                    .Select(upav => upav.AccountVisit);

                var providerOrders = await providerOrdersQuery.ToListAsync();
                var accountVisits = await accountVisitsQuery.ToListAsync();

                var dayOfWeekCounts = Enum.GetValues(typeof(DayOfWeek)).Cast<DayOfWeek>()
                    .Select(day => new
                    {
                        DayOfWeek = day,
                        SalesCount = providerOrders.Count(po => po.CreatedAt.DayOfWeek == day),
                        ViewsCount = accountVisits.Count(av => av.FirstVisitDate.DayOfWeek == day)
                    })
                    .OrderBy(d => (int)d.DayOfWeek)
                    .ToList();

                var graphDataResponse = new GraphDataResponse
                {
                    Label = "By Week",
                    Data = new GraphDataObject
                    {
                        Labels = dayOfWeekCounts.Select(d => d.DayOfWeek.ToString().Substring(0, 3)).ToList(),
                        Sales = dayOfWeekCounts.Select(d => d.SalesCount).ToList(),
                        Views = dayOfWeekCounts.Select(d => d.ViewsCount).ToList()
                    }
                };

                return graphDataResponse;
            }
            else if (SaleGraphFilter == 2) // By Month
            {
                startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                endDate = startDate.AddMonths(1).AddDays(-1);

                var providerOrdersQuery = _dbContext.Users
                    .Where(u => u.ID == UserID)
                    .Join(
                        _dbContext.Providers,
                        u => u.ID,
                        p => p.UserID,
                        (u, p) => new { User = u, Provider = p }
                    )
                    .Join(
                        _dbContext.ProviderOrders,
                        up => up.Provider.ID,
                        po => po.ProviderID,
                        (up, po) => new { UserProvider = up, ProviderOrder = po }
                    )
                    .Where(upo => upo.ProviderOrder.CreatedAt >= startDate && upo.ProviderOrder.CreatedAt <= endDate)
                    .Select(upo => upo.ProviderOrder);

                var accountVisitsQuery = _dbContext.Users
                    .Where(u => u.ID == UserID)
                    .Join(
                        _dbContext.Providers,
                        u => u.ID,
                        p => p.UserID,
                        (u, p) => new { User = u, Provider = p }
                    )
                    .Join(
                        _dbContext.AccountVisits,
                        up => up.Provider.ID,
                        av => av.ProviderID,
                        (up, av) => new { UserProvider = up, AccountVisit = av }
                    )
                    .Where(upav => upav.AccountVisit.FirstVisitDate >= startDate && upav.AccountVisit.FirstVisitDate <= endDate)
                    .Select(upav => upav.AccountVisit);

                var providerOrderData = providerOrdersQuery
                    .GroupBy(po => (po.CreatedAt.Day - 1) / 5) // Group by 5-day intervals
                    .Select(g => new { Interval = g.Key, SalesCount = g.Count() })
                    .OrderBy(d => d.Interval)
                    .ToList();

                var accountVisitData = accountVisitsQuery
                    .GroupBy(av => (av.FirstVisitDate.Day - 1) / 5) // Group by 5-day intervals
                    .Select(g => new { Interval = g.Key, ViewsCount = g.Count() })
                    .OrderBy(d => d.Interval)
                    .ToList();

                var labels = Enumerable.Range(1, DateTime.DaysInMonth(startDate.Year, startDate.Month))
                                            .Select(i => $"{i}")
                                            .ToList();

                var sales = new List<int>();
                var views = new List<int>();

                for (int i = 0; i < labels.Count; i++)
                {
                    int salesCount = providerOrderData.FirstOrDefault(d => d.Interval == i)?.SalesCount ?? 0;
                    int viewsCount = accountVisitData.FirstOrDefault(d => d.Interval == i)?.ViewsCount ?? 0;

                    sales.Add(salesCount);
                    views.Add(viewsCount);
                }

                return new GraphDataResponse
                {
                    Label = "By Month",
                    Data = new GraphDataObject
                    {
                        Labels = labels,
                        Sales = sales,
                        Views = views
                    }
                };
            }
            else if (SaleGraphFilter == 3) // By Year
            {
                startDate = DateTime.Today.AddYears(-1);
                endDate = DateTime.Today.AddDays(1);

                var providerOrdersQuery = _dbContext.Users
                    .Where(u => u.ID == UserID)
                    .Join(
                        _dbContext.Providers,
                        u => u.ID,
                        p => p.UserID,
                        (u, p) => new { User = u, Provider = p }
                    )
                    .Join(
                        _dbContext.ProviderOrders,
                        up => up.Provider.ID,
                        po => po.ProviderID,
                        (up, po) => new { UserProvider = up, ProviderOrder = po }
                    )
                    .Where(upo => upo.ProviderOrder.CreatedAt >= startDate && upo.ProviderOrder.CreatedAt <= endDate)
                    .Select(upo => upo.ProviderOrder);

                var accountVisitsQuery = _dbContext.Users
                    .Where(u => u.ID == UserID)
                    .Join(
                        _dbContext.Providers,
                        u => u.ID,
                        p => p.UserID,
                        (u, p) => new { User = u, Provider = p }
                    )
                    .Join(
                        _dbContext.AccountVisits,
                        up => up.Provider.ID,
                        av => av.ProviderID,
                        (up, av) => new { UserProvider = up, AccountVisit = av }
                    )
                    .Where(upav => upav.AccountVisit.FirstVisitDate >= startDate && upav.AccountVisit.FirstVisitDate <= endDate)
                    .Select(upav => upav.AccountVisit);

                var providerOrderData = providerOrdersQuery
                    .GroupBy(po => po.CreatedAt.Month) // Group by month
                    .Select(g => new { Month = g.Key, SalesCount = g.Count() })
                    .OrderBy(d => d.Month)
                    .ToDictionary(d => d.Month, d => d.SalesCount);

                var accountVisitData = accountVisitsQuery
                    .GroupBy(av => av.FirstVisitDate.Month) // Group by month
                    .Select(g => new { Month = g.Key, ViewsCount = g.Count() })
                    .OrderBy(d => d.Month)
                    .ToDictionary(d => d.Month, d => d.ViewsCount);

                var labels = Enumerable.Range(1, 12)
                    .Select(i => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i))
                    .ToList();

                var sales = Enumerable.Range(1, 12)
                    .Select(i => providerOrderData.TryGetValue(i, out var count) ? count : 0)
                    .ToList();

                var views = Enumerable.Range(1, 12)
                    .Select(i => accountVisitData.TryGetValue(i, out var count) ? count : 0)
                    .ToList();

                return new GraphDataResponse
                {
                    Label = "By Year",
                    Data = new GraphDataObject
                    {
                        Labels = labels,
                        Sales = sales,
                        Views = views
                    }
                };
            }
            throw new Exception("Invalid Filter Value");
        }

        public async Task<EarnGraphDataResponse> GetEarnGraphData(int EarnGraphFilter, long UserID)
        {
            var transactionsQuery = from po in _dbContext.ProviderOrders
                                    join p in _dbContext.Providers.Where(p => p.UserID == UserID) on po.ProviderID equals p.ID
                                    join t in _dbContext.Transactions on po.OrderID equals t.OrderID
                                    select t;

            EarnGraphDataResponse response = new EarnGraphDataResponse
            {
                Label = "All"
            };

            if (EarnGraphFilter == 1)
            {
                DateTime startDate;
                DateTime endDate;
                startDate = DateTime.Today.AddMonths(-1);
                endDate = DateTime.Today.AddDays(1);
                transactionsQuery = transactionsQuery.Where(t => t.TransactionTime >= startDate && t.TransactionTime <= endDate);
                response.Label = "Last Month";
            }

            // Group transactions by date and calculate the sum of the amount for each date
            var groupedTransactions = transactionsQuery
                .GroupBy(t => new { Date = t.TransactionTime.Date })
                .Select(g => new { Date = g.Key.Date, AmountSum = g.Sum(t => t.Amount) })
                .OrderBy(g => g.Date);

            // Filter transactions for each 5-day interval and generate data for labels and earnings
            var labels = new List<string>();
            var earnings = new List<decimal>();

            DateTime intervalStart = groupedTransactions.Min(g => g.Date).AddDays(-1);
            DateTime intervalEnd = intervalStart.AddDays(4);

            while (intervalEnd <= DateTime.Today.AddDays(1))
            {
                var transactionsInRange = groupedTransactions
                    .Where(g => g.Date >= intervalStart && g.Date <= intervalEnd);

                if (EarnGraphFilter == 1)
                    labels.Add($"{intervalStart.Date.Day} - {intervalEnd.Date.Day}");
                earnings.Add(transactionsInRange.Sum(g => g.AmountSum));

                intervalStart = intervalEnd.AddDays(1);
                intervalEnd = intervalStart.AddDays(4);
            }

            if (EarnGraphFilter != 1)
            {
                for (int i = 0; i < 6; i++)
                    labels.Add("");
                labels.Add("Life Time");
                for (int i = 0; i < 6; i++)
                    labels.Add("");
            }

            // Create the EarnGraphDataObject
            var earnGraphDataObject = new EarnGraphDataObject
            {
                Labels = labels,
                Earning = earnings
            };

            // Set the EarnGraphDataObject in the response
            response.Data = earnGraphDataObject;

            return response;
        }

        public async Task<Provider> GetByGovtID(string GovtID)
        {
            return await _dbContext.Providers.Where(p => p.GovernmentID == GovtID).FirstOrDefaultAsync();
        }

    }
}
