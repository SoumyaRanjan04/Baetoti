using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Shared.Request.Chat;
using Microsoft.AspNetCore.Http;
using Baetoti.Shared.Request.Shared;
using Baetoti.API.Helpers;
using Baetoti.Shared.Response.FileUpload;
using Baetoti.Shared.Request.Notification;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Enum;

namespace Baetoti.API.Controllers
{
    public class ChatController : ApiBaseController
    {

        private readonly IChatAPIService _chatAPIService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INotificationService _notificationService;
        private readonly IPushNotificationRepository _pushNotificationRepository;
        private readonly IUserRepository _userRepository;

        public ChatController(
            ISiteConfigService siteConfigService,
            IChatAPIService chatAPIService,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,
            IPushNotificationRepository pushNotificationRepository,
            IUserRepository userRepository
            ) : base(siteConfigService)
        {
            _chatAPIService = chatAPIService;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _pushNotificationRepository = pushNotificationRepository;
            _userRepository = userRepository;
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var response = await _chatAPIService.SendMessage(request, _httpContextAccessor.HttpContext, _siteConfig);
                Core.Entites.User user = await _userRepository.GetByIdAsync(Convert.ToInt64(UserId));
                var notification = await _pushNotificationRepository.GetByNotificationType((int)NotificationType.NewMessage);
                CreateNotificationRequest createNotificationRequest = new CreateNotificationRequest
                {
                    DestUserID = request.DestUser,
                    Type = notification.NotificationTypeID,
                    DestUserType = request.ReceiverUserType,
                    NotificationTitle = notification.Title.Replace("{username}", $"{user.FirstName}"),
                    NotificationBody = notification.Text.Replace("{username}", $"{user.FirstName}"),
                    SenderUser = UserId,
                    SenderUserType = request.SenderUserType,
                    SubjectID = request.DestUser
                };
                await _notificationService.CreatePushNotification(createNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("SearchMessage")]
        public async Task<IActionResult> SearchMessage([FromBody] SearchMessageRequest request)
        {
            try
            {
                var response = await _chatAPIService.SearchMessageRequest(request, _httpContextAccessor.HttpContext, _siteConfig);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Block")]
        public async Task<IActionResult> Block([FromBody] UserIDRequest request)
        {
            try
            {
                var response = await _chatAPIService.Block(request, _httpContextAccessor.HttpContext, _siteConfig);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("UnBlock")]
        public async Task<IActionResult> UnBlock([FromBody] UserIDRequest request)
        {
            try
            {
                var response = await _chatAPIService.UnBlock(request, _httpContextAccessor.HttpContext, _siteConfig);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Mute")]
        public async Task<IActionResult> Mute([FromBody] UserIDRequest request)
        {
            try
            {
                var response = await _chatAPIService.Mute(request, _httpContextAccessor.HttpContext, _siteConfig);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("UnMute")]
        public async Task<IActionResult> UnMute([FromBody] UserIDRequest request)
        {
            try
            {
                var response = await _chatAPIService.UnMute(request, _httpContextAccessor.HttpContext, _siteConfig);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll([FromBody] PaginationRequest request)
        {
            try
            {
                var response = await _chatAPIService.GetAllChat(request, _httpContextAccessor.HttpContext, _siteConfig);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("GetByID")]
        public async Task<IActionResult> GetByID([FromBody] GetChatByIDRequest request)
        {
            try
            {
                var response = await _chatAPIService.GetByID(request, _httpContextAccessor.HttpContext, _siteConfig);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    UploadImage obj = new UploadImage();
                    FileUploadResponse _RESPONSE = await obj.UploadImageFile(file, $"Chat/{UserId}/");
                    if (string.IsNullOrEmpty(_RESPONSE.Message))
                    {
                        return Ok(new SharedResponse(true, 200, "File uploaded successfully!", _RESPONSE));
                    }
                    else
                    {
                        return Ok(new SharedResponse(true, 400, _RESPONSE.Message));
                    }
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "File is required!"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

    }
}
