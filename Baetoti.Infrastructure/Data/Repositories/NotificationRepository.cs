using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Request.Notification;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.Notification;
using Baetoti.Shared.Extentions;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class NotificationRepository : EFRepository<Notification>, INotificationRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public NotificationRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginationResponse> GetAll(GetAllNotificationRequest request, long UserID)
        {
            var list = from n in _dbContext.Notifications
                       join u in _dbContext.Users on n.UserID equals u.ID
                       join e in _dbContext.Employee on n.CreatedBy equals e.ID
                       where request.UserType == 0 || u.ID == UserID orderby n.CreatedAt descending
                       select new NotificationResponse
                       {
                           ID = n.ID,
                           UserID = u.ID,
                           Name = n.Name,
                           NameArabic = n.NameArabic,
                           Text = n.Text,
                           TextArabic = n.TextArabic,
                           Title = n.Title,
                           TitleArabic = n.TitleArabic,
                           SentBy = $"{e.FirstName} {e.LastName}",
                           CreatedAt = n.CreatedAt,
                           UserPicture = u.Picture
                       };
            var totalRecords = list.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            return new PaginationResponse
            {
                CurrentPage = request.PageNumber,
                TotalPages = totalPages,
                PageSize = request.PageSize,
                TotalCount = totalRecords,
                Data = await list.Skip((request.PageNumber - 1) * request.PageSize)
                                .Take(request.PageSize).ToListAsync()
            };
        }

        public async Task<NotificationResponse> GetByID(long ID)
        {
            return await (from n in _dbContext.Notifications
                          join u in _dbContext.Users on n.UserID equals u.ID
                          join e in _dbContext.Employee on n.CreatedBy equals e.ID
                          where n.ID == ID
                          select new NotificationResponse
                          {
                              ID = n.ID,
                              UserID = u.ID,
                              Name = n.Name,
                              NameArabic = n.NameArabic,
                              Text = n.Text,
                              TextArabic = n.TextArabic,
                              Title = n.Title,
                              TitleArabic = n.TitleArabic,
                              SentBy = $"{e.FirstName} {e.LastName}",
                              CreatedAt = n.CreatedAt,
                              UserPicture = u.Picture
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<Notification>> GetFilteredUser(NotificationRequest request, long EmployeeId)
        {
            var list = from u in _dbContext.Users
                       where (string.IsNullOrEmpty(request.Gender) || request.Gender == u.Gender)
                       select new
                       {
                           Name = request.Name,
                           NameArabic = request.NameArabic,
                           Text = request.Text,
                           TextArabic = request.TextArabic,
                           Title = request.Title,
                           TitleArabic = request.TitleArabic,
                           UserID = u.ID,
                           SentCount = 1,
                           CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time"),
                           CreatedBy = EmployeeId,
                           UserLatitude = u.Latitude,
                           UserLongitude = u.Longitude,
                           UserType = request.UserType
                       };

            if (request.UserIDs.Any() && request.UserIDs.Count > 0)
            {
                list = list.Where(user => request.UserIDs.Contains(user.UserID.ToString()));
            }
            else
            {
                if (request.UserType == (int)UserType.Buyer)
                {
                    list = from user in list
                           join driver in _dbContext.Drivers on user.UserID equals driver.UserID into driverGroup
                           join provider in _dbContext.Providers on user.UserID equals provider.UserID into providerGroup
                           from driverResult in driverGroup.DefaultIfEmpty()
                           from providerResult in providerGroup.DefaultIfEmpty()
                           where driverResult == null && providerResult == null
                           select user;
                }
                else if (request.UserType == (int)UserType.Provider)
                {
                    list = from user in list
                           join p in _dbContext.Providers on user.UserID equals p.UserID
                           where (request.CountryID == 0 || p.CountryId == request.CountryID) &&
                           (request.CityID == "" || p.CityId == request.CityID) &&
                           (request.RegionID == "" || p.RegionId == request.RegionID)
                           select user;
                }
                else if (request.UserType == (int)UserType.Driver)
                {
                    list = from user in list
                           join d in _dbContext.Drivers on user.UserID equals d.UserID
                           where (request.CountryID == 0 || d.CountryId == request.CountryID) &&
                           (request.CityID == "" || d.CityId == request.CityID) &&
                           (request.RegionID == "" || d.RegionId == request.RegionID)
                           select user;
                }
            }

            var filteredList = list;

            return await filteredList
                .Select(x => new Notification
                {
                    Name = x.Name,
                    NameArabic = x.NameArabic,
                    Text = x.Text,
                    TextArabic = x.TextArabic,
                    Title = x.Title,
                    TitleArabic = x.TitleArabic,
                    UserID = x.UserID,
                    SentCount = x.SentCount,
                    CreatedAt = x.CreatedAt,
                    CreatedBy = x.CreatedBy,
                    UserType = x.UserType
                })
                .ToListAsync();
        }

    }
}
