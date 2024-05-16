using Baetoti.API.Controllers.Base;
using Baetoti.Core.Entites;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Shared.Request.Shared;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Extentions;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Request.Notification;
using Microsoft.AspNetCore.Http;
using NotificationType = Baetoti.Shared.Enum.NotificationType;
using Baetoti.Shared.Response.Dashboard;
using Driver = Baetoti.Core.Entites.Driver;
using Baetoti.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace Baetoti.API.Controllers
{
    public class RatingAndReviewController : ApiBaseController
    {

        private readonly IBuyerReviewRepository _buyerReviewRepository;
        private readonly IDriverReviewRepository _driverReviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPushNotificationRepository _pushNotificationRepository;
        private readonly IDriverOrderRepository _driverOrderRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProviderOrderRepository _providerOrderRepository;
        private readonly IStoreRepository _storeRepository;

        public RatingAndReviewController(
            ISiteConfigService siteConfigService,
            IBuyerReviewRepository buyerReviewRepository,
            IDriverReviewRepository driverReviewRepository,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,
            IPushNotificationRepository pushNotificationRepository,
            IUserRepository userRepository,
            IDriverOrderRepository driverOrderRepository,
            IOrderRepository orderRepository,
            IProviderOrderRepository providerOrderRepository,
            IStoreRepository storeRepository
            ) : base(siteConfigService)
        {
            _buyerReviewRepository = buyerReviewRepository;
            _driverReviewRepository = driverReviewRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _pushNotificationRepository = pushNotificationRepository;
            _driverOrderRepository = driverOrderRepository;
            _orderRepository = orderRepository;
            _storeRepository = storeRepository;
            _providerOrderRepository = providerOrderRepository;
        }

        [HttpPost("AddRatingAndReview")]
        public async Task<IActionResult> AddRatingAndReview([FromBody] RatingAndReviews request)
        {
            try
            {
                if (request.UserType == (int)UserType.Buyer)
                {
                    BuyerReview buyerReview = new BuyerReview
                    {
                        OrderID = request.OrderID,
                        DriverID = request.DriverID,
                        ProviderID = request.ProviderID,
                        UserID = request.UserID,
                        Rating = request.Rating,
                        Reviews = request.Reviews,
                        RecordDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                    };
                    await _buyerReviewRepository.AddAsync(buyerReview);
                }
                else if (request.UserType == (int)UserType.Driver)
                {
                    DriverReview driverReview = new DriverReview
                    {
                        OrderID = request.OrderID,
                        DriverID = request.DriverID,
                        ProviderID = request.ProviderID,
                        UserID = request.UserID,
                        Rating = request.Rating,
                        Reviews = request.Reviews,
                        RecordDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                    };
                    await _driverReviewRepository.AddAsync(driverReview);
                }

                // Notification

                Order order = await _orderRepository.GetByIdAsync(request.OrderID);
                Core.Entites.Provider provider = await _providerOrderRepository.GetProviderByOrderID(order.ID);
                Driver driver = await _driverOrderRepository.GetDriverByOrderID(order.ID);
                User user = await _userRepository.GetByIdAsync(order.UserID);
                Store store = await _storeRepository.GetByProviderId(provider.ID);
                User driverUser = driver != null ? await _userRepository.GetByIdAsync(driver.UserID) : null;

                string DestinationUserID = "";
                int DestinationUserType = 0;
                int NotificationTypeID = 0;
                string SenderUser = "";
                int SenderUserType = 0;
                string SubjectID = "";

                if (request.UserType == (int)UserType.Buyer && request.ProviderID > 0)
                {
                    DestinationUserID = request.UserID.ToString();
                    DestinationUserType = (int)UserType.Buyer;
                    NotificationTypeID = (int)NotificationType.BuyerRatedByProvider;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Provider;
                    SubjectID = request.OrderID.ToString();
                }
                else if (request.UserType == (int)UserType.Buyer && request.DriverID > 0)
                {
                    DestinationUserID = request.UserID.ToString();
                    DestinationUserType = (int)UserType.Buyer;
                    NotificationTypeID = (int)NotificationType.BuyerRatedByDriver;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Driver;
                    SubjectID = request.OrderID.ToString();
                }
                else if (request.UserType == (int)UserType.Driver && request.UserID > 0)
                {
                    DestinationUserID = driver.UserID.ToString();
                    DestinationUserType = (int)UserType.Driver;
                    NotificationTypeID = (int)NotificationType.DriverRatedByBuyer;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Buyer;
                    SubjectID = request.OrderID.ToString();
                }
                else if (request.UserType == (int)UserType.Driver && request.ProviderID > 0)
                {
                    DestinationUserID = driver.UserID.ToString();
                    DestinationUserType = (int)UserType.Driver;
                    NotificationTypeID = (int)NotificationType.DriverRatedByProvider;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Provider;
                    SubjectID = request.OrderID.ToString();
                }

                var notification = await _pushNotificationRepository.GetByNotificationType(NotificationTypeID);
                CreateNotificationRequest createNotificationRequest = new CreateNotificationRequest()
                {
                    DestUserID = DestinationUserID,
                    Type = notification.NotificationTypeID,
                    DestUserType = DestinationUserType,
                    NotificationTitle = notification.Title.Replace("{buyername}", $"{user.FirstName}")
                    .Replace("{providername}", $"{store.Name}").Replace("{drivername}", driverUser != null ? $"{driverUser.FirstName}" : "Driver"),
                    NotificationBody = notification.Text.Replace("{buyername}", $"{user.FirstName}")
                    .Replace("{providername}", $"{store.Name}").Replace("{drivername}", driverUser != null ? $"{driverUser.FirstName}" : "Driver"),
                    SenderUser = SenderUser,
                    SenderUserType = SenderUserType,
                    SubjectID = SubjectID
                };
                await _notificationService.CreatePushNotification(createNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);


                return Ok(new SharedResponse(true, 200, "Rating and Reviews Submitted Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost("GetRatingAndReview")]
        public async Task<IActionResult> GetRatingAndReview([FromBody] RatingAndReviewRequest request)
        {
            try
            {
                long? result = 0;
                if (string.IsNullOrEmpty(UserId))
                {
                    result = null;
                }
                else
                {
                    result = Convert.ToInt64(UserId);

                }
                if (request.UserType == (int)UserType.Buyer)
                {
                    var response = await _buyerReviewRepository.GetReviews(request, result);
                    return Ok(new SharedResponse(true, 200, "", response));
                }
                else if (request.UserType == (int)UserType.Driver)
                {
                    var response = await _driverReviewRepository.GetReviews(request, result);
                    return Ok(new SharedResponse(true, 200, "", response));
                }
                return Ok(new SharedResponse(true, 400, "Invalid User Type"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetBookmarkAndAccountVisit")]
        public async Task<IActionResult> GetBookmarkAndAccountVisit(int UserType)
        {
            try
            {
                var response = await _userRepository.GetBookmarkAndAccountVisit(Convert.ToInt64(UserId), UserType);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
