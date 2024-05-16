using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Helper;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Driver;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class DriverRepository : EFRepository<Driver>, IDriverRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public DriverRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<GetAllDriverResponse>> GetAllAsync()
        {
            return await (from d in _dbContext.Drivers
                          join u in _dbContext.Users on d.UserID equals u.ID
                          select new GetAllDriverResponse
                          {
                              ID = d.ID,
                              Name = $"{u.FirstName} {u.LastName}",
                              Location = u.Address,
                              Picture = u.Picture,
                              Latitude = u.Latitude,
                              Longitude = u.Longitude
                          }).ToListAsync();
        }

        public async Task<GetDriverByIDResponse> GetByUserID(long UserID)
        {
            return await (from d in _dbContext.Drivers.Where(d => d.UserID == UserID)
                          join u in _dbContext.Users on d.UserID equals u.ID
                          join c in _dbContext.Countries on d.CountryId equals c.ID into tempCountry
                          from c in tempCountry.DefaultIfEmpty()
                          join r in _dbContext.Regions on d.RegionId equals r.Id into tempRegions
                          from r in tempRegions.DefaultIfEmpty()
                          join ct in _dbContext.Cities on d.CityId equals ct.Id into tempCities
                          from ct in tempCities.DefaultIfEmpty()
                          join car in _dbContext.CarTypes on d.CarTypeId equals car.Id into tempCarTypes
                          from car in tempCarTypes.DefaultIfEmpty()
                          select new GetDriverByIDResponse
                          {
                              ID = d.ID,
                              UserID = UserID,
                              UserName =u.FirstName + " " + u.LastName,
                              DriverStatus = d.DriverStatus,
                              Comments = d.Comments,
                              Nationality = d.Nationality,
                              DOB = d.DOB,
                              IDNumber = d.IDNumber,
                              IDExpiryDate = d.IDExpiryDate,
                              IDIssueDate = d.IDIssueDate,
                              FrontSideofIDPic = d.FrontSideofIDPic,
                              BackSideofIDPic = d.BackSideofIDPic,
                              IBAN = d.IBAN,
                              Title = d.Title,
                              Gender = d.Gender,
                              PersonalPic = d.PersonalPic,
                              DrivingLicensePic = d.DrivingLicensePic,
                              ExpirayDateofLicense = d.ExpirayDateofLicense,
                              VehicleRegistrationPic = d.VehicleRegistrationPic,
                              ExpiryDateofVehicleRegistration = d.ExpiryDateofVehicleRegistration,
                              VehicleAuthorizationPic = d.VehicleAuthorizationPic,
                              ExpiryDateofVehicleAuthorization = d.ExpiryDateofVehicleAuthorization,
                              MedicalCheckupPic = d.MedicalCheckupPic,
                              ExpiryDateofMedicalcheckup = d.ExpiryDateofMedicalcheckup,
                              VehicleInsurancePic = d.VehicleInsurancePic,
                              ExpiryDateofVehicleInsurance = d.ExpiryDateofVehicleInsurance,
                              IdentityTypeId = d.IdentityTypeId,
                              CarType = car != null ? car.NameAr : "",
                              Region = r != null ? r.NameAr : "",
                              City = ct != null ? ct.NameAr : "",
                              Country = c != null ? c.CountryName : "",
                              RegistrationDate = d.RegistrationDate,
                              MobileNumber = d.MobileNumber,
                              CarNumber = d.CarNumber,
                              VehicleSequenceNumber = d.VehicleSequenceNumber,
                              IsOnline = d.IsOnline,
                              IsPublic = d.IsPublic,
                              IsAcceptJob = d.IsAcceptJob,
                              CreatedAt = d.CreatedAt,
                              LastUpdatedAt = d.LastUpdatedAt,
                              OnBoardingStatus = d.OnBoardingStatus
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<GetAllDriverResponse>> GetNearBy(long UserID, double LocationRange)
        {
            Store store = await (from s in _dbContext.Stores
                                 join p in _dbContext.Providers on s.ProviderID equals p.ID
                                 where p.UserID == UserID
                                 select s).FirstOrDefaultAsync();

            var res = await (from d in _dbContext.Drivers
                             join u in _dbContext.Users on d.UserID equals u.ID
                             where (d.DriverStatus == 1 || d.DriverStatus == 3) &&
                             d.IsPublic == true && d.IsOnline == true && d.IsAcceptJob == true
                             select new GetAllDriverResponse
                             {
                                 ID = d.ID,
                                 UserID = u.ID,
                                 Name = $"{u.FirstName} {u.LastName}",
                                 Location = u.Address,
                                 Picture = u.Picture,
                                 Latitude = u.Latitude,
                                 Longitude = u.Longitude,
                                 Distance = Helper.GetDistance(store.Latitude, store.Longitude, u.Latitude, u.Longitude)
                             }).ToListAsync();

            return LocationRange == 0 ? res.Where(x => x.Distance <= 10).DistinctBy(x => x.ID).ToList() :
                res.Where(x => x.Distance <= LocationRange).DistinctBy(x => x.ID).ToList();
        }

        public async Task<EarnGraphDataResponse> GetEarnGraphData(int EarnGraphFilter, long UserID)
        {
            var transactionsQuery = from dor in _dbContext.DriverOrders
                                    join d in _dbContext.Drivers.Where(d => d.UserID == UserID) on dor.DriverID equals d.ID
                                    join t in _dbContext.Transactions on dor.OrderID equals t.OrderID
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

            DateTime intervalStart = groupedTransactions.Min(g => g.Date);
            DateTime intervalEnd = intervalStart.AddDays(4);

            while (intervalEnd <= DateTime.Today)
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

        public async Task<Driver> GetByGovtID(string GovtID)
        {
            return await _dbContext.Drivers.Where(d => d.IDNumber.Trim() == GovtID.Trim()).FirstOrDefaultAsync();
        }

        public async Task<Driver> GetDriverByUserID(long UserID)
        {
            return await _dbContext.Drivers.Where(d => d.UserID == UserID).FirstOrDefaultAsync();
        }

    }
}
