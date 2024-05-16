using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Shared.Request.Notification;
using System.Collections.Generic;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Extentions;
using Baetoti.Core.Interface.Services;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Baetoti.Shared.Enum;

namespace Baetoti.API.Controllers
{
    public class NotificationController : ApiBaseController
    {

        private readonly INotificationTypeRepository _notificationTypeRepository;
        private readonly IPushNotificationRepository _pushNotificationRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public NotificationController(
            ISiteConfigService siteConfigService,
            INotificationTypeRepository notificationTypeRepository,
            IPushNotificationRepository pushNotificationRepository,
            INotificationRepository notificationRepository,
            INotificationService notificationService,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
            ) : base(siteConfigService)
        {
            _notificationTypeRepository = notificationTypeRepository;
            _pushNotificationRepository = pushNotificationRepository;
            _notificationRepository = notificationRepository;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        // Push Notification

        [HttpPost("AddPushNotification")]
        public async Task<IActionResult> AddPushNotification([FromBody] PushNotificationRequest request)
        {
            try
            {
                var pushNotification = _mapper.Map<PushNotification>(request);
                pushNotification.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                pushNotification.CreatedBy = Convert.ToInt32(UserId);
                await _pushNotificationRepository.AddAsync(pushNotification);
                return Ok(new SharedResponse(true, 200, "Push Notification Created Succesfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("UpdatePushNotification")]
        public async Task<IActionResult> UpdatePushNotification([FromBody] PushNotificationRequest request)
        {
            try
            {
                var pushNotification = await _pushNotificationRepository.GetByIdAsync(request.ID);
                if (pushNotification != null)
                {
                    pushNotification.Title = request.Title;
                    pushNotification.TitleArabic = request.TitleArabic;
                    pushNotification.Text = request.Text;
                    pushNotification.TextArabic = request.TextArabic;
                    pushNotification.NotificationTypeID = request.NotificationTypeID;
                    pushNotification.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    pushNotification.UpdatedBy = Convert.ToInt32(UserId);
                    await _pushNotificationRepository.UpdateAsync(pushNotification);
                    return Ok(new SharedResponse(true, 200, "Push Notification Updated Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "unable to find push notification"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpGet("GetPushNotificationTypes")]
        public async Task<IActionResult> GetPushNotificationTypes()
        {
            try
            {
                var notificationTypes = await _notificationTypeRepository.ListAllAsync();
                return Ok(new SharedResponse(true, 200, "", notificationTypes));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetAllPushNotification")]
        public async Task<IActionResult> GetAllPushNotification()
        {
            try
            {
                var pushNotifications = await _pushNotificationRepository.GetAll();
                return Ok(new SharedResponse(true, 200, "", pushNotifications));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetPushNotificationById")]
        public async Task<IActionResult> GetPushNotificationById(long ID)
        {
            try
            {
                var pushNotification = await _pushNotificationRepository.GetByID(ID);
                return Ok(new SharedResponse(true, 200, "", pushNotification));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        // Notification

        [HttpPost("SendNotification")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            try
            {
                List<Notification> notificationList = await _notificationRepository.GetFilteredUser(request, long.Parse(UserId));
                if (notificationList.Count > 0)
                {
                    await _notificationRepository.AddRangeAsync(notificationList);

                    ServiceNotificationRequest serviceNotificationRequest = new ServiceNotificationRequest
                    {
                        NotificationTitle = request.Title,
                        NotificationBody = request.Text,
                        UserIDs = notificationList.Select(x => x.UserID.ToString()).ToList(),
                        UserTypes = request.UserTypeList

                    };
                    await _notificationService.CreateServiceNotification(serviceNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);

                    return Ok(new SharedResponse(true, 200, "Notification Sent Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(true, 200, "No User Found According to Filter Selected"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("SendNotificationByID")]
        public async Task<IActionResult> SendNotificationByID([FromBody] SendNotificationByIDRequest request)
        {
            try
            {
                Notification notification = await _notificationRepository.GetByIdAsync(request.ID);
                notification.SentCount = notification.SentCount + 1;
                await _notificationRepository.UpdateAsync(notification);

                ServiceNotificationRequest serviceNotificationRequest = new ServiceNotificationRequest
                {
                    NotificationTitle = notification.Title,
                    NotificationBody = notification.Text,
                    UserIDs = new List<string> { notification.UserID.ToString() },
                    UserTypes = new List<UserType> { (UserType)notification.UserType },
                };
                serviceNotificationRequest.UserIDs.Add(notification.UserID.ToString());
                await _notificationService.CreateServiceNotification(serviceNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);

                return Ok(new SharedResponse(true, 200, "Notification Sent Succesfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpGet("GetNotificationById")]
        public async Task<IActionResult> GetNotificationById(long ID)
        {
            try
            {
                var notification = await _notificationRepository.GetByID(ID);
                return Ok(new SharedResponse(true, 200, "", notification));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetAllNotification")]
        public async Task<IActionResult> GetAllNotification(GetAllNotificationRequest request)
        {
            try
            {
                var notifications = await _notificationRepository.GetAll(request, long.Parse(UserId));
                return Ok(new SharedResponse(true, 200, "", notifications));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        // For Mobile APP

        [HttpGet("GetNotificationToken")]
        public async Task<IActionResult> GetNotificationToken()
        {
            try
            {
                var notification = await _notificationService.GetNotificationToken(long.Parse(UserId), _httpContextAccessor.HttpContext, _siteConfig);
                return Ok(notification);
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("UpdateNotificationToken")]
        public async Task<IActionResult> UpdateNotificationToken(UpdateNotificationTokenRequest request)
        {
            try
            {
                var notifications = await _notificationService.UpdateNotificationToken(request, _httpContextAccessor.HttpContext, _siteConfig);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("MarkNotificationAsRead")]
        public async Task<IActionResult> MarkNotificationAsRead(RequestID request)
        {
            try
            {
                var notifications = await _notificationService.MarkNotificationAsRead(request, _httpContextAccessor.HttpContext, _siteConfig);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetNotifications")]
        public async Task<IActionResult> GetNotifications(GetAllNotificationRequest request)
        {
            try
            {
                var notification = await _notificationService.GetNotifications(request, _httpContextAccessor.HttpContext, _siteConfig);
                return Ok(new SharedResponse(true, 200, "", notification));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
