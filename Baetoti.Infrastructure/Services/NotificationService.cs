using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Services;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Services.Base;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Request.Notification;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Notification;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Services
{
    public class NotificationService : BaseAPIService, INotificationService
    {

        private readonly BaetotiDbContext _dbContext;

        public NotificationService(
            HttpClient httpClient,
            BaetotiDbContext dbContext
            ) : base(httpClient)
        {
            _dbContext = dbContext;
        }

        public override async Task<TResponse> CallAPI<TRequest, TResponse>(string apiUrl, TRequest requestBody, HttpMethod httpMethod, HttpContext httpContext = null)
        {
            var responseBody = await InvokeAPI(apiUrl, requestBody, httpMethod, httpContext);
            return JsonConvert.DeserializeObject<TResponse>(responseBody);
        }

        public async Task<SharedResponse> CreatePushNotification(CreateNotificationRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.NotificationAPIURL}Notification/CreateNotification", request, HttpMethod.Post, httpContext);
        }

        public async Task<SharedResponse> CreateServiceNotification(ServiceNotificationRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.NotificationAPIURL}ServiceNotification/CreateNotification", request, HttpMethod.Post, httpContext);
        }

        public async Task<SharedResponse> RelayUserSignout(string userID, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.NotificationAPIURL}SignoutRelay?userID={userID}", null, HttpMethod.Get, httpContext);
        }

        public async Task<PaginationResponse> GetNotifications(GetAllNotificationRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            SharedResponse response = await CallAPI<object, SharedResponse>($"{siteConfig.NotificationAPIURL}Notification/GetNotifications?userType={request.UserType}&pageNumber={request.PageNumber}&pageSize={request.PageSize}", request, HttpMethod.Get, httpContext);
            PaginationResponse paginationResponse = JsonConvert.DeserializeObject<PaginationResponse>(Convert.ToString(response.Record));
            List<MobileNotificationResponse> notificationList = JsonConvert.DeserializeObject<List<MobileNotificationResponse>>(Convert.ToString(paginationResponse.Data));

            List<MobileNotificationResponse> finalResponse = new List<MobileNotificationResponse>();
            foreach (var notification in notificationList)
            {
                MobileNotificationResponse mobileNotificationResponse = new MobileNotificationResponse();
                mobileNotificationResponse.Id = notification.Id;
                mobileNotificationResponse.ReceiverUser = notification.ReceiverUser;
                mobileNotificationResponse.ReceiverUserType = notification.ReceiverUserType;
                mobileNotificationResponse.SenderUser = notification.SenderUser;
                mobileNotificationResponse.SenderUserType = notification.SenderUserType;
                mobileNotificationResponse.Type = notification.Type;
                mobileNotificationResponse.Title = notification.Title;
                mobileNotificationResponse.Body = notification.Body;
                mobileNotificationResponse.IsRead = notification.IsRead;
                mobileNotificationResponse.CreationTime = notification.CreationTime;
                mobileNotificationResponse.SubjectID = notification.SubjectID;
                long ReceiverID = long.Parse(notification.ReceiverUser);
                long SenderID = long.Parse(notification.SenderUser);

                if (notification.ReceiverUserType == UserType.Buyer)
                {
                    mobileNotificationResponse.Receiver = await (from u in _dbContext.Users
                                                                 where u.ID == ReceiverID
                                                                 select new NotificationUser
                                                                 {
                                                                     ID = u.ID,
                                                                     Name = $"{u.FirstName}",
                                                                     Image = u.Picture,
                                                                     IsOnline = true
                                                                 }).FirstOrDefaultAsync();
                }
                else if (notification.ReceiverUserType == UserType.Driver)
                {
                    mobileNotificationResponse.Receiver = await (from u in _dbContext.Users
                                                                 join d in _dbContext.Drivers on u.ID equals d.UserID
                                                                 where u.ID == ReceiverID
                                                                 select new NotificationUser
                                                                 {
                                                                     ID = u.ID,
                                                                     Name = $"{u.FirstName}",
                                                                     Image = u.Picture,
                                                                     IsOnline = d.IsOnline
                                                                 }).FirstOrDefaultAsync();
                }
                else if (notification.ReceiverUserType == UserType.Provider)
                {
                    mobileNotificationResponse.Receiver = await (from u in _dbContext.Users
                                                                 join p in _dbContext.Providers on u.ID equals p.UserID
                                                                 join s in _dbContext.Stores on p.ID equals s.ProviderID
                                                                 where u.ID == ReceiverID
                                                                 select new NotificationUser
                                                                 {
                                                                     ID = u.ID,
                                                                     Name = s.Name,
                                                                     Image = s.BusinessLogo,
                                                                     ProviderImage = u.Picture,
                                                                     IsOnline = p.IsOnline
                                                                 }).FirstOrDefaultAsync();
                }

                if (notification.SenderUserType == UserType.Buyer)
                {
                    mobileNotificationResponse.Sender = await (from u in _dbContext.Users
                                                               where u.ID == SenderID
                                                               select new NotificationUser
                                                               {
                                                                   ID = u.ID,
                                                                   Name = $"{u.FirstName}",
                                                                   Image = u.Picture,
                                                                   IsOnline = true
                                                               }).FirstOrDefaultAsync();
                }
                else if (notification.SenderUserType == UserType.Driver)
                {
                    mobileNotificationResponse.Sender = await (from u in _dbContext.Users
                                                               join d in _dbContext.Drivers on u.ID equals d.UserID
                                                               where u.ID == SenderID
                                                               select new NotificationUser
                                                               {
                                                                   ID = u.ID,
                                                                   Name = $"{u.FirstName}",
                                                                   Image = u.Picture,
                                                                   IsOnline = d.IsOnline
                                                               }).FirstOrDefaultAsync();
                }
                else if (notification.SenderUserType == UserType.Provider)
                {
                    mobileNotificationResponse.Sender = await (from u in _dbContext.Users
                                                               join p in _dbContext.Providers on u.ID equals p.UserID
                                                               join s in _dbContext.Stores on p.ID equals s.ProviderID
                                                               where u.ID == SenderID
                                                               select new NotificationUser
                                                               {
                                                                   ID = u.ID,
                                                                   Name = s.Name,
                                                                   Image = s.BusinessLogo,
                                                                   ProviderImage = u.Picture,
                                                                   IsOnline = p.IsOnline
                                                               }).FirstOrDefaultAsync();
                }

                finalResponse.Add(mobileNotificationResponse);
            }
            paginationResponse.Data = finalResponse;
            return paginationResponse;
        }

        public async Task<SharedResponse> GetNotificationToken(long UserID, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.NotificationAPIURL}UserNotificationTokenEntry/GetNotificationToken?UserID={UserID}", null, HttpMethod.Get, httpContext);
        }

        public async Task<SharedResponse> MarkNotificationAsRead(RequestID request, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.NotificationAPIURL}Notification/MarkNotificationAsRead?ID={request.ID}", null, HttpMethod.Get, httpContext);
        }

        public async Task<SharedResponse> UpdateNotificationToken(UpdateNotificationTokenRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.NotificationAPIURL}UserNotificationTokenEntry/UpdateNotificationToken", request, HttpMethod.Post, httpContext);
        }

    }
}
