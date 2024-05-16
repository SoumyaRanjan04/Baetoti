using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.API.Helpers;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Request.Driver;
using Baetoti.Shared.Request.DriverConfig;
using Baetoti.Shared.Request.FavouriteDriver;
using Baetoti.Shared.Request.Notification;
using Baetoti.Shared.Request.OrderRequest;
using Baetoti.Shared.Request.Provider;
using Baetoti.Shared.Response.DriverConfig;
using Baetoti.Shared.Response.Fence;
using Baetoti.Shared.Response.FileUpload;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Driver = Baetoti.Core.Entites.Driver;
using NotificationType = Baetoti.Shared.Enum.NotificationType;

namespace Baetoti.API.Controllers
{
    public class DriverController : ApiBaseController
    {
        public readonly IEmployeeRepository _employeeRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IDriverConfigRepository _driverConfigRepository;
        private readonly IOrderRequestRepository _orderRequestRepository;
        private readonly IDriverOrderRepository _driverOrderRepository;
        private readonly IFavouriteDriverRepository _favouriteDriverRepository;
        private readonly IAccountVisitRepository _accountVisitRepository;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPushNotificationRepository _pushNotificationRepository;
        private readonly IProviderOrderRepository _providerOrderRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IFenceRepository _fenceRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IMapper _mapper;

        public DriverController(
            ISiteConfigService siteConfigService,
            IDriverRepository driverRepository,
            IDriverConfigRepository driverConfigRepository,
            IOrderRequestRepository orderRequestRepository,
            IDriverOrderRepository driverOrderRepository,
            IFavouriteDriverRepository favouriteDriverRepository,
            IAccountVisitRepository accountVisitRepository,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,
            IPushNotificationRepository pushNotificationRepository,
            IProviderOrderRepository providerOrderRepository,
            IOrderRepository orderRepository,
            IEmployeeRepository employeeRepository,
            IFenceRepository fenceRepository,
            IItemRepository itemRepository,
            IMapper mapper
            ) : base(siteConfigService)
        {
            _driverRepository = driverRepository;
            _driverConfigRepository = driverConfigRepository;
            _favouriteDriverRepository = favouriteDriverRepository;
            _driverOrderRepository = driverOrderRepository;
            _orderRequestRepository = orderRequestRepository;
            _accountVisitRepository = accountVisitRepository;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _pushNotificationRepository = pushNotificationRepository;
            _providerOrderRepository = providerOrderRepository;
            _orderRepository = orderRepository;
            _employeeRepository = employeeRepository;
            _fenceRepository = fenceRepository;
            _itemRepository = itemRepository;
            _mapper = mapper;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] DriverRequest driverRequest)
        {
            try
            {
                Driver alreadyExist = await _driverRepository.GetByGovtID(driverRequest.Nationality);
                FenceResponse isNotExisit = await _fenceRepository.GetFenceByCityID(driverRequest.CityId);
                if (isNotExisit == null)
                    return Ok(new SharedResponse(false, 400, "Services are not available in your city."));
                if (alreadyExist != null)
                    return Ok(new SharedResponse(false, 400, "Another driver with same govt id already exists."));

                var driver = _mapper.Map<Driver>(driverRequest);
                if (!string.IsNullOrEmpty(driverRequest.Gender))
                    driver.Title = driverRequest.Gender == "Male" ? "Mr" : "Ms";
                driver.DriverStatus = (int)DriverStatus.Pending;
                driver.MarkAsDeleted = false;
                driver.IsOnline = true;
                driver.IsPublic = true;
                driver.IsAcceptJob = true;
                driver.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                driver.CreatedBy = Convert.ToInt32(UserId);
                var result = await _driverRepository.AddAsync(driver);
                if (result == null)
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Submit Driver Request"));
                }
                return Ok(new SharedResponse(true, 200, "Driver Request Submitted Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] DriverRequest driverRequest)
        {
            try
            {
                Driver alreadyExist = await _driverRepository.GetByGovtID(driverRequest.Nationality);
                if (alreadyExist != null && alreadyExist.UserID != driverRequest.UserID)
                    return Ok(new SharedResponse(false, 400, "Another driver with same govt id already exists."));

                var driver = await _driverRepository.GetDriverByUserID(driverRequest.UserID);
                driver.IDNumber = driverRequest.Nationality;
                driver.DOB = driverRequest.DOB;
                driver.Nationality = driverRequest.Nationality;
                driver.MobileNumber = driverRequest.MobileNumber;
                driver.RegistrationDate = driverRequest.RegistrationDate;
                driver.RegionId = driverRequest.RegionId;
                driver.CityId = driverRequest.CityId;
                driver.CountryId = driverRequest.CountryId;
                driver.CarTypeId = driverRequest.CarTypeId;
                driver.CarNumber = driverRequest.CarNumber;
                if (!string.IsNullOrEmpty(driverRequest.IDIssueDate))
                    driver.IDIssueDate = driverRequest.IDIssueDate;
                if (!string.IsNullOrEmpty(driverRequest.IDExpiryDate))
                    driver.IDExpiryDate = driverRequest.IDExpiryDate;
                if (!string.IsNullOrEmpty(driverRequest.FrontSideofIDPic))
                    driver.FrontSideofIDPic = driverRequest.FrontSideofIDPic;
                if (!string.IsNullOrEmpty(driverRequest.BackSideofIDPic))
                    driver.BackSideofIDPic = driverRequest.BackSideofIDPic;
                if (!string.IsNullOrEmpty(driverRequest.Gender))
                    driver.Title = driverRequest.Gender == "Male" ? "Mr" : "Ms";
                driver.DriverStatus = driverRequest.DriverStatus;
                driver.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                driver.UpdatedBy = Convert.ToInt32(UserId);
                await _driverRepository.UpdateAsync(driver);
                return Ok(new SharedResponse(true, 200, "Driver Request Submitted Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpGet("GetByUserID")]
        public async Task<IActionResult> GetByUserID(long Id)
        {
            try
            {
                var driver = await _driverRepository.GetByUserID(Id);

                AccountVisit accountVisit = new AccountVisit
                {
                    DriverID = driver.ID,
                    UserID = Id,
                    UserType = (int)UserType.Driver,
                    FirstVisitDate = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                };
                await _accountVisitRepository.LogAccountVisit(accountVisit);

                return Ok(new SharedResponse(true, 200, "", driver));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var drivers = await _driverRepository.GetAllAsync();
                return Ok(new SharedResponse(true, 200, "", drivers));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetNearBy")]
        public async Task<IActionResult> GetNearBy(double LocationRange)
        {
            try
            {
                var driverList = await _driverRepository.GetNearBy(Convert.ToInt64(UserId), LocationRange);
                return Ok(new SharedResponse(true, 200, "", driverList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetEarnGraphData")]
        public async Task<IActionResult> GetEarnGraphData(int EarnGraphFilter)
        {
            try
            {
                var response = await _driverRepository.GetEarnGraphData(EarnGraphFilter, long.Parse(UserId));
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] DriverApprovalRequest request)
        {
            try
            {
                var driver = await _driverRepository.GetDriverByUserID(request.UserID);
                if (driver != null)
                {
                    driver.DriverStatus = request.StatusValue;
                    driver.Comments = request.Comments;
                    driver.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    driver.UpdatedBy = Convert.ToInt32(UserId);
                    await _driverRepository.UpdateAsync(driver);

                    // Notification

                    string DestinationUserID = "";
                    int DestinationUserType = 0;
                    int NotificationTypeID = 0;
                    string SenderUser = "";
                    int SenderUserType = 0;
                    string SubjectID = "";

                    if (driver.DriverStatus == (int)DriverStatus.Approved)
                    {
                        DestinationUserID = driver.UserID.ToString();
                        DestinationUserType = (int)UserType.Driver;
                        NotificationTypeID = (int)NotificationType.CaptainOnboardingApproved;
                        SenderUser = UserId;
                        SenderUserType = (int)UserType.Admin;
                        SubjectID = "";
                    }
                    else if (driver.DriverStatus == (int)DriverStatus.Rejected)
                    {
                        DestinationUserID = driver.UserID.ToString();
                        DestinationUserType = (int)UserType.Driver;
                        NotificationTypeID = (int)NotificationType.CaptainOnboardingRejected;
                        SenderUser = UserId;
                        SenderUserType = (int)UserType.Admin;
                        SubjectID = "";
                    }

                    if (NotificationTypeID > 0)
                    {
                        var notification = await _pushNotificationRepository.GetByNotificationType(NotificationTypeID);
                        CreateNotificationRequest createNotificationRequest = new CreateNotificationRequest()
                        {
                            DestUserID = DestinationUserID,
                            Type = notification.NotificationTypeID,
                            DestUserType = DestinationUserType,
                            NotificationTitle = notification.Title,
                            NotificationBody = notification.Text,
                            SenderUser = SenderUser,
                            SenderUserType = SenderUserType,
                            SubjectID = SubjectID
                        };
                        await _notificationService.CreatePushNotification(createNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);
                    }

                    return Ok(new SharedResponse(true, 200, "Driver Status Updated Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Driver"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("UpdateOnBoardingStatus")]
        public async Task<IActionResult> UpdateOnBoardingStatus([FromBody] DriverApprovalRequest request)
        {
            try
            {
                var driver = await _driverRepository.GetDriverByUserID(request.UserID);
                if (driver != null)
                {
                    driver.OnBoardingStatus = request.StatusValue;
                    if (request.StatusValue == (int)OnBoardingStatus.Approved)
                        driver.DriverStatus = (int)DriverStatus.Active;
                    driver.Comments = request.Comments;
                    driver.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    driver.UpdatedByAdminId = Convert.ToInt32(UserId);
                    await _driverRepository.UpdateAsync(driver);

                    // Notification

                    string DestinationUserID = "";
                    int DestinationUserType = 0;
                    int NotificationTypeID = 0;
                    string SenderUser = "";
                    int SenderUserType = 0;
                    string SubjectID = "";

                    if (driver.OnBoardingStatus == (int)OnBoardingStatus.Approved)
                    {
                        DestinationUserID = driver.UserID.ToString();
                        DestinationUserType = (int)UserType.Driver;
                        NotificationTypeID = (int)NotificationType.CaptainOnboardingApproved;
                        SenderUser = UserId;
                        SenderUserType = (int)UserType.Admin;
                        SubjectID = "";
                    }
                    else if (driver.OnBoardingStatus == (int)OnBoardingStatus.Rejected)
                    {
                        DestinationUserID = driver.UserID.ToString();
                        DestinationUserType = (int)UserType.Driver;
                        NotificationTypeID = (int)NotificationType.CaptainOnboardingRejected;
                        SenderUser = UserId;
                        SenderUserType = (int)UserType.Admin;
                        SubjectID = "";
                    }

                    if (NotificationTypeID > 0)
                    {
                        var notification = await _pushNotificationRepository.GetByNotificationType(NotificationTypeID);
                        CreateNotificationRequest createNotificationRequest = new CreateNotificationRequest()
                        {
                            DestUserID = DestinationUserID,
                            Type = notification.NotificationTypeID,
                            DestUserType = DestinationUserType,
                            NotificationTitle = notification.Title,
                            NotificationBody = notification.Text,
                            SenderUser = SenderUser,
                            SenderUserType = SenderUserType,
                            SubjectID = SubjectID
                        };
                        await _notificationService.CreatePushNotification(createNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);
                    }

                    return Ok(new SharedResponse(true, 200, "Driver Status Updated Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Driver"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("UpdateOrderRequestStatus")]
        public async Task<IActionResult> UpdateOrderRequestStatus([FromBody] OrderRequestApprovalRequest request)
        {
            try
            {
                OrderRequest orderRequest = await _orderRequestRepository.GetByOrderID(request.OrderID);
                if (request.OrderRequestStatus == (int)OrderRequestStatus.Accepted)
                {
                    orderRequest.RequestStatus = (int)OrderRequestStatus.Accepted;
                    orderRequest.Comments = request.Comments;
                    await _orderRequestRepository.UpdateAsync(orderRequest);

                    DriverOrder driverOrder = new DriverOrder
                    {
                        OrderID = request.OrderID,
                        DriverID = request.DriverID,
                        Status = (int)DriverOrderStatus.Pending,
                        CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                    };
                    await _driverOrderRepository.AddAsync(driverOrder);
                }
                else
                {
                    orderRequest.RequestStatus = request.OrderRequestStatus;
                    orderRequest.Comments = request.Comments;
                    await _orderRequestRepository.UpdateAsync(orderRequest);
                }

                // Notification

                string DestinationUserID = "";
                int DestinationUserType = 0;
                int NotificationTypeID = 0;
                string SenderUser = "";
                int SenderUserType = 0;
                string SubjectID = "";

                if (request.OrderRequestStatus == (int)OrderRequestStatus.Accepted)
                {
                    Provider provider = await _providerOrderRepository.GetProviderByOrderID(request.OrderID);
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.DriverAcceptDeliveryRequest;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Driver;
                    SubjectID = request.OrderID.ToString();
                }
                else if (request.OrderRequestStatus == (int)OrderRequestStatus.Rejected)
                {
                    Provider provider = await _providerOrderRepository.GetProviderByOrderID(request.OrderID);
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.DriverCancelDeliveryRequest;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Driver;
                    SubjectID = request.OrderID.ToString();
                }
                else if (request.OrderRequestStatus == (int)OrderRequestStatus.ArrivedAtStore)
                {
                    Provider provider = await _providerOrderRepository.GetProviderByOrderID(request.OrderID);
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.DriverArrivePickup;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Driver;
                    SubjectID = request.OrderID.ToString();
                }
                else if (request.OrderRequestStatus == (int)OrderRequestStatus.ArrivedAtDestination)
                {
                    Order order = await _orderRepository.GetByIdAsync(request.OrderID);
                    DestinationUserID = order.UserID.ToString();
                    DestinationUserType = (int)UserType.Buyer;
                    NotificationTypeID = (int)NotificationType.DriverArivedDeliveryPoint;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Driver;
                    SubjectID = request.OrderID.ToString();
                }
                else if (request.OrderRequestStatus == (int)OrderRequestStatus.Completed)
                {
                    Provider provider = await _providerOrderRepository.GetProviderByOrderID(request.OrderID);
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.DriverOrderCompleted;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Driver;
                    SubjectID = request.OrderID.ToString();
                }

                if (NotificationTypeID > 0)
                {
                    var notification = await _pushNotificationRepository.GetByNotificationType(NotificationTypeID);
                    CreateNotificationRequest createNotificationRequest = new CreateNotificationRequest()
                    {
                        DestUserID = DestinationUserID,
                        Type = notification.NotificationTypeID,
                        DestUserType = DestinationUserType,
                        NotificationTitle = notification.Title,
                        NotificationBody = notification.Text,
                        SenderUser = SenderUser,
                        SenderUserType = SenderUserType,
                        SubjectID = SubjectID
                    };
                    await _notificationService.CreatePushNotification(createNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);
                }

                return Ok(new SharedResponse(true, 200, "Request Submitted Successfully", new { ID = request.OrderID }));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("SetOnline")]
        public async Task<IActionResult> SetOnline([FromBody] StatusChangeRequest request)
        {
            try
            {
                var driver = await _driverRepository.GetDriverByUserID(request.UserID);
                driver.IsOnline = request.StatusValue;
                driver.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                driver.UpdatedBy = Convert.ToInt32(UserId);
                await _driverRepository.UpdateAsync(driver);
                return Ok(new SharedResponse(true, 200, "Driver Request Processed Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("SetPublic")]
        public async Task<IActionResult> SetPublic([FromBody] StatusChangeRequest request)
        {
            try
            {
                var driver = await _driverRepository.GetDriverByUserID(request.UserID);
                driver.IsPublic = request.StatusValue;
                driver.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                driver.UpdatedBy = Convert.ToInt32(UserId);
                await _driverRepository.UpdateAsync(driver);
                return Ok(new SharedResponse(true, 200, "Driver Request Processed Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("SetAcceptJob")]
        public async Task<IActionResult> SetAcceptJob([FromBody] StatusChangeRequest request)
        {
            try
            {
                var driver = await _driverRepository.GetDriverByUserID(request.UserID);
                driver.IsAcceptJob = request.StatusValue;
                driver.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                driver.UpdatedBy = Convert.ToInt32(UserId);
                await _driverRepository.UpdateAsync(driver);
                return Ok(new SharedResponse(true, 200, "Driver Request Processed Successfully"));
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
                    FileUploadResponse _RESPONSE = await obj.UploadImageFile(file, "Driver");
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

        // Configs

        [HttpGet("GetAllConfigs")]
        public async Task<IActionResult> GetAllConfigs()
        {
            try
            {
                var config = await _driverConfigRepository.GetAllConfig();
                return Ok(new SharedResponse(true, 200, "", config));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetConfigById")]
        public async Task<IActionResult> GetConfigById(int Id)
        {
            try
            {
                var config = await _driverConfigRepository.GetByIdAsync(Id);
                return Ok(new SharedResponse(true, 200, "", _mapper.Map<DriverConfigResponse>(config)));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("AddConfig")]
        public async Task<IActionResult> AddConfig([FromBody] DriverConfigRequest request)
        {
            try
            {
                var updateExistingConfig = (await _driverConfigRepository.ListAllAsync()).Where(c => !c.MarkAsDeleted);
                foreach (var item in updateExistingConfig)
                {
                    item.MarkAsDeleted = true;
                    await _driverConfigRepository.UpdateAsync(item);
                }
                var config = _mapper.Map<DriverConfig>(request);
                config.MarkAsDeleted = false;
                config.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                config.CreatedBy = Convert.ToInt32(UserId);
                var result = await _driverConfigRepository.AddAsync(config);
                if (result == null)
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Create Delivery Config"));
                }
                else
                {
                    var itemList = await _itemRepository.GetListofAllItems();
                    foreach (var item in itemList)
                    {
                        item.BaetotiPrice = (config.ProviderComission / 100 * item.Price) + item.Price;
                    }
                    await _itemRepository.UpdateRangeAsync(itemList);
                    return Ok(new SharedResponse(true, 200, "DeliveryConfig/BaetotiPrice  Updated Successfully"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("UpdateConfig")]
        public async Task<IActionResult> UpdateConfig([FromBody] DriverConfigRequest request)
        {
            try
            {
                var config = await _driverConfigRepository.GetByIdAsync(request.ID);
                if (config != null)
                {
                    config.FromKM = request.FromKM;
                    config.ToKM = request.ToKM;
                    config.RatePerKM = request.RatePerKM;
                    config.AdditionalKM = request.AdditionalKM;
                    config.AdditionalRatePerKM = request.AdditionalRatePerKM;
                    config.Currency = request.Currency;
                    config.MaximumDistance = request.MaximumDistance;
                    config.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    config.UpdatedBy = Convert.ToInt32(UserId);
                    config.DriverComission = request.DriverComission;
                    config.ProviderComission = request.ProviderComission;
                    config.ServiceFee = request.ServiceFee;
                    config.ServiceFeeFixed = request.ServiceFeeFixed;
                    await _driverConfigRepository.UpdateAsync(config);

                    var itemList = await _itemRepository.GetListofAllItems();
                    foreach (var item in itemList)
                    {
                        item.BaetotiPrice = (config.ProviderComission / 100 * item.Price) + item.Price;
                    }
                    await _itemRepository.UpdateRangeAsync(itemList);
                    return Ok(new SharedResponse(true, 200, "Config Updated Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable to Find Config"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        // Favourities

        [HttpPost("AddFavourite")]
        public async Task<IActionResult> AddFavourite([FromBody] FavouriteDriverRequest request)
        {
            try
            {
                var driver = await _favouriteDriverRepository.CheckIfExists(request);
                if (driver == null)
                {
                    FavouriteDriver favouriteDriver = new FavouriteDriver
                    {
                        DriverID = request.DriverID,
                        UserID = request.UserID,
                        ProviderID = request.ProviderID,
                        RecordDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                    };
                    await _favouriteDriverRepository.AddAsync(favouriteDriver);
                    return Ok(new SharedResponse(true, 200, "Driver Added in Favourite List Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Already in Favourite List"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("RemoveFavourite")]
        public async Task<IActionResult> RemoveFavourite([FromBody] FavouriteDriverRequest request)
        {
            try
            {
                var driver = await _favouriteDriverRepository.CheckIfExists(request);
                if (driver != null)
                {
                    await _favouriteDriverRepository.DeleteAsync(driver);
                    return Ok(new SharedResponse(true, 200, "Driver Removed from Favourite List Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Driver doesn't exists in Favourite List"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("GetFavourite")]
        public async Task<IActionResult> GetFavourite([FromBody] GetFavouriteDriverRequest request)
        {
            try
            {
                var driverList = await _favouriteDriverRepository.GetFavouriteDriver(request);
                return Ok(new SharedResponse(true, 200, "", driverList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
