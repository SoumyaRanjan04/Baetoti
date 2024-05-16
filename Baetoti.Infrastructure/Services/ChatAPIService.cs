using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Services;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Services.Base;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Request.Chat;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Chat;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Services
{
    public class ChatAPIService : BaseAPIService, IChatAPIService
    {

        private readonly BaetotiDbContext _dbContext;

        public ChatAPIService(HttpClient httpClient, BaetotiDbContext dbContext) : base(httpClient)
        {
            _dbContext = dbContext;
        }

        public override async Task<TResponse> CallAPI<TRequest, TResponse>(string apiUrl, TRequest requestBody, HttpMethod httpMethod, HttpContext httpContext = null)
        {
            var responseBody = await InvokeAPI(apiUrl, requestBody, httpMethod, httpContext);
            return JsonConvert.DeserializeObject<TResponse>(responseBody);
        }

        public async Task<SharedResponse> SendMessage(SendMessageRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.ChatAPIURL}ChatThread/hubs/chat", request, HttpMethod.Post, httpContext);
        }

        public async Task<SharedResponse> Block(UserIDRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.ChatAPIURL}ChatThread/block", request, HttpMethod.Post, httpContext);
        }

        public async Task<SharedResponse> UnBlock(UserIDRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.ChatAPIURL}ChatThread/unblock", request, HttpMethod.Post, httpContext);
        }

        public async Task<SharedResponse> Mute(UserIDRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.ChatAPIURL}ChatThread/mute", request, HttpMethod.Post, httpContext);
        }

        public async Task<SharedResponse> UnMute(UserIDRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.ChatAPIURL}ChatThread/unmute", request, HttpMethod.Post, httpContext);
        }

        public async Task<PaginationResponse> GetAllChat(PaginationRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            SharedResponse response = await CallAPI<object, SharedResponse>($"{siteConfig.ChatAPIURL}ChatThread?PageSize={request.PageSize}&PageNumber={request.PageNumber}", null, HttpMethod.Get, httpContext);
            PaginationResponse paginationResponse = JsonConvert.DeserializeObject<PaginationResponse>(Convert.ToString(response.Record));
            List<ChatThreadResponse> chatThreadResponseList = JsonConvert.DeserializeObject<List<ChatThreadResponse>>(Convert.ToString(paginationResponse.Data));
            List<ChatResponse> chatResponse = new List<ChatResponse>();
            foreach (var chatThread in chatThreadResponseList)
            {
                ChatResponse chat = new ChatResponse();
                chat.LastMessage = chatThread.ChatMessages.FirstOrDefault();
                chat.isRead = chatThread.isRead;
                chat.IsThreadMuted = chatThread.IsThreadMuted;
                chat.IsThreadBlocked = chatThread.IsThreadBlocked;
                chat.MutedByUser = chatThread.MutedByUser;
                chat.BlockedByUser = chatThread.BlockedByUser;
                chat.LastMessageTime = chatThread.LastMessageTime;
                chat.RecevierUserType = chatThread.RecevierUserType;
                chat.SenderUserType = chatThread.SenderUserType;

                // Sender
                long SenderID = long.Parse(chatThread.SenderUser);
                if (chatThread.SenderUserType == UserType.Buyer)
                {
                    chat.Sender = await (from u in _dbContext.Users
                                         where u.ID == SenderID
                                         select new ChatUser
                                         {
                                             ID = u.ID,
                                             Name = $"{u.FirstName}",
                                             Image = u.Picture,
                                             IsOnline = u.IsOnline
                                         }).FirstOrDefaultAsync();
                }
                else if (chatThread.SenderUserType == UserType.Driver)
                {
                    chat.Sender = await (from u in _dbContext.Users
                                         join d in _dbContext.Drivers on u.ID equals d.UserID
                                         where u.ID == SenderID
                                         select new ChatUser
                                         {
                                             ID = u.ID,
                                             Name = $"{u.FirstName}",
                                             Image = u.Picture,
                                             IsOnline = d.IsOnline
                                         }).FirstOrDefaultAsync();
                }
                else if (chatThread.SenderUserType == UserType.Provider)
                {
                    chat.Sender = await (from u in _dbContext.Users
                                         join p in _dbContext.Providers on u.ID equals p.UserID
                                         join s in _dbContext.Stores on p.ID equals s.ProviderID
                                         where u.ID == SenderID
                                         select new ChatUser
                                         {
                                             ID = u.ID,
                                             Name = s.Name,
                                             Image = s.BusinessLogo,
                                             ProviderImage = u.Picture,
                                             IsOnline = p.IsOnline
                                         }).FirstOrDefaultAsync();
                }

                // Receiver
                long ReceiverID = long.Parse(chatThread.ReceiverUser);
                if (chatThread.RecevierUserType == UserType.Buyer)
                {
                    chat.Receiver = await (from u in _dbContext.Users
                                           where u.ID == ReceiverID
                                           select new ChatUser
                                           {
                                               ID = u.ID,
                                               Name = $"{u.FirstName}",
                                               Image = u.Picture,
                                               IsOnline = true
                                           }).FirstOrDefaultAsync();
                }
                else if (chatThread.RecevierUserType == UserType.Driver)
                {
                    chat.Receiver = await (from u in _dbContext.Users
                                           join d in _dbContext.Drivers on u.ID equals d.UserID
                                           where u.ID == ReceiverID
                                           select new ChatUser
                                           {
                                               ID = u.ID,
                                               Name = $"{u.FirstName}",
                                               Image = u.Picture,
                                               IsOnline = d.IsOnline
                                           }).FirstOrDefaultAsync();
                }
                else if (chatThread.RecevierUserType == UserType.Provider)
                {
                    chat.Receiver = await (from u in _dbContext.Users
                                           join p in _dbContext.Providers on u.ID equals p.UserID
                                           join s in _dbContext.Stores on p.ID equals s.ProviderID
                                           where u.ID == ReceiverID
                                           select new ChatUser
                                           {
                                               ID = u.ID,
                                               Name = s.Name,
                                               Image = s.BusinessLogo,
                                               ProviderImage = u.Picture,
                                               IsOnline = p.IsOnline
                                           }).FirstOrDefaultAsync();
                }
                chatResponse.Add(chat);
            }
            paginationResponse.Data = chatResponse;
            return paginationResponse;
        }

        public async Task<SharedResponse> GetByID(GetChatByIDRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            return await CallAPI<object, SharedResponse>($"{siteConfig.ChatAPIURL}ChatThread/{request.ID}?SenderUserType={request.SenderUserType}&ReceiverUserType={request.ReceiverUserType}&PageSize={request.PageSize}&PageNumber={request.PageNumber}", null, HttpMethod.Get, httpContext);
        }

        public async Task<PaginationResponse> SearchMessageRequest(SearchMessageRequest request, HttpContext httpContext, SiteConfig siteConfig)
        {
            var UserIDs = from u in _dbContext.Users
                          where u.FirstName.ToLower().Contains(request.SearchValue.ToLower()) || u.LastName.ToLower().Contains(request.SearchValue.ToLower())
                          select u.ID.ToString();

            var StoreUserIDs = from u in _dbContext.Users
                               join p in _dbContext.Providers on u.ID equals p.UserID
                               join s in _dbContext.Stores on p.ID equals s.ProviderID
                               where s.Name.ToLower().Contains(request.SearchValue.ToLower())
                               select u.ID.ToString();

            request.UserIDs = await UserIDs.Union(StoreUserIDs).ToListAsync();

            SharedResponse response = await CallAPI<object, SharedResponse>($"{siteConfig.ChatAPIURL}ChatThread/SearchThreads", request, HttpMethod.Post, httpContext);
            PaginationResponse paginationResponse = JsonConvert.DeserializeObject<PaginationResponse>(Convert.ToString(response.Record));
            List<ChatThreadResponse> chatThreadResponseList = JsonConvert.DeserializeObject<List<ChatThreadResponse>>(Convert.ToString(paginationResponse.Data));
            List<ChatResponse> chatResponse = new List<ChatResponse>();
            foreach (var chatThread in chatThreadResponseList)
            {
                ChatResponse chat = new ChatResponse();
                chat.LastMessage = chatThread.ChatMessages?.FirstOrDefault();
                chat.isRead = chatThread.isRead;
                chat.IsThreadMuted = chatThread.IsThreadMuted;
                chat.IsThreadBlocked = chatThread.IsThreadBlocked;
                chat.MutedByUser = chatThread.MutedByUser;
                chat.BlockedByUser = chatThread.BlockedByUser;
                chat.LastMessageTime = chatThread.LastMessageTime;
                chat.RecevierUserType = chatThread.RecevierUserType;
                chat.SenderUserType = chatThread.SenderUserType;

                // Sender
                long SenderID = long.Parse(chatThread.SenderUser);
                if (chatThread.SenderUserType == UserType.Buyer)
                {
                    chat.Sender = await (from u in _dbContext.Users
                                         where u.ID == SenderID
                                         select new ChatUser
                                         {
                                             ID = u.ID,
                                             Name = $"{u.FirstName}",
                                             Image = u.Picture,
                                             IsOnline = u.IsOnline
                                         }).FirstOrDefaultAsync();
                }
                else if (chatThread.SenderUserType == UserType.Driver)
                {
                    chat.Sender = await (from u in _dbContext.Users
                                         join d in _dbContext.Drivers on u.ID equals d.UserID
                                         where u.ID == SenderID
                                         select new ChatUser
                                         {
                                             ID = u.ID,
                                             Name = $"{u.FirstName}",
                                             Image = u.Picture,
                                             IsOnline = d.IsOnline
                                         }).FirstOrDefaultAsync();
                }
                else if (chatThread.SenderUserType == UserType.Provider)
                {
                    chat.Sender = await (from u in _dbContext.Users
                                         join p in _dbContext.Providers on u.ID equals p.UserID
                                         join s in _dbContext.Stores on p.ID equals s.ProviderID
                                         where u.ID == SenderID
                                         select new ChatUser
                                         {
                                             ID = u.ID,
                                             Name = s.Name,
                                             Image = s.BusinessLogo,
                                             ProviderImage = u.Picture,
                                             IsOnline = p.IsOnline
                                         }).FirstOrDefaultAsync();
                }

                // Receiver
                long ReceiverID = long.Parse(chatThread.ReceiverUser);
                if (chatThread.RecevierUserType == UserType.Buyer)
                {
                    chat.Receiver = await (from u in _dbContext.Users
                                           where u.ID == ReceiverID
                                           select new ChatUser
                                           {
                                               ID = u.ID,
                                               Name = $"{u.FirstName}",
                                               Image = u.Picture,
                                               IsOnline = true
                                           }).FirstOrDefaultAsync();
                }
                else if (chatThread.RecevierUserType == UserType.Driver)
                {
                    chat.Receiver = await (from u in _dbContext.Users
                                           join d in _dbContext.Drivers on u.ID equals d.UserID
                                           where u.ID == ReceiverID
                                           select new ChatUser
                                           {
                                               ID = u.ID,
                                               Name = $"{u.FirstName}",
                                               Image = u.Picture,
                                               IsOnline = d.IsOnline
                                           }).FirstOrDefaultAsync();
                }
                else if (chatThread.RecevierUserType == UserType.Provider)
                {
                    chat.Receiver = await (from u in _dbContext.Users
                                           join p in _dbContext.Providers on u.ID equals p.UserID
                                           join s in _dbContext.Stores on p.ID equals s.ProviderID
                                           where u.ID == ReceiverID
                                           select new ChatUser
                                           {
                                               ID = u.ID,
                                               Name = s.Name,
                                               Image = s.BusinessLogo,
                                               ProviderImage = u.Picture,
                                               IsOnline = p.IsOnline
                                           }).FirstOrDefaultAsync();
                }
                chatResponse.Add(chat);
            }
            paginationResponse.Data = chatResponse;
            return paginationResponse;
        }

    }
}
