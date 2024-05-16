using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.API.Helpers;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Request.Notification;
using Baetoti.Shared.Request.ReportedStore;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Request.Store;
using Baetoti.Shared.Response.FileUpload;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NotificationType = Baetoti.Shared.Enum.NotificationType;

namespace Baetoti.API.Controllers
{
    public class StoreController : ApiBaseController
    {
        public readonly IStoreRepository _storeRepository;
        public readonly IReportedStoreRepository _reportedStoreRepository;
        public readonly IFavouriteStoreRepository _favouriteStoreRepository;
        public readonly IStoreTagRepository _storeTagRepository;
        public readonly IStoreImageRepository _storeImageRepository;
        public readonly IStoreReviewRepository _storeReviewRepository;
        public readonly IItemReviewRepository _itemReviewRepository;
        public readonly IAccountVisitRepository _accountVisitRepository;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPushNotificationRepository _pushNotificationRepository;
        private readonly IProviderOrderRepository _providerOrderRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IDriverOrderRepository _driverOrderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStoreAddressRepository _storeAddressRepository;
        public readonly IMapper _mapper;

        public StoreController(
            ISiteConfigService siteConfigService,
            IStoreRepository storeRepository,
            IStoreTagRepository storeTagRepository,
            IReportedStoreRepository reportedStoreRepository,
            IFavouriteStoreRepository favouriteStoreRepository,
            IStoreImageRepository storeImageRepository,
            IStoreReviewRepository storeReviewRepository,
            IItemReviewRepository itemReviewRepository,
            IAccountVisitRepository accountVisitRepository,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,
            IPushNotificationRepository pushNotificationRepository,
            IProviderOrderRepository providerOrderRepository,
            IOrderRepository orderRepository,
            IDriverOrderRepository driverOrderRepository,
            IUserRepository userRepository,
            IStoreAddressRepository storeAddressRepository,
            IMapper mapper
            ) : base(siteConfigService)
        {
            _storeRepository = storeRepository;
            _reportedStoreRepository = reportedStoreRepository;
            _favouriteStoreRepository = favouriteStoreRepository;
            _storeTagRepository = storeTagRepository;
            _storeImageRepository = storeImageRepository;
            _storeReviewRepository = storeReviewRepository;
            _itemReviewRepository = itemReviewRepository;
            _accountVisitRepository = accountVisitRepository;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _pushNotificationRepository = pushNotificationRepository;
            _providerOrderRepository = providerOrderRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _driverOrderRepository = driverOrderRepository;
            _storeAddressRepository = storeAddressRepository;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var storeList = (await _storeRepository.ListAllAsync()).
                    Where(x => x.MarkAsDeleted == false).ToList();
                return Ok(new SharedResponse(true, 200, "", _mapper.Map<List<StoreResponse>>(storeList)));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [AllowAnonymous]
        [HttpGet("GetOnlineProvidersStore")]
        public async Task<IActionResult> GetOnlineProvidersStore(decimal LocationRange)
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
                var storeList = await _storeRepository.GetOnlineProvidersStore(LocationRange, result);
                return Ok(new SharedResponse(true, 200, "", storeList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [AllowAnonymous]
        [HttpPost("GetNearBy")]
        public async Task<IActionResult> GetNearBy([FromBody] StoreFilterRequest storeFilterRequest)
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
                var storeList = await _storeRepository.GetNearBy(storeFilterRequest, result);
                return Ok(new SharedResponse(true, 200, "", storeList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetTags")]
        public async Task<IActionResult> GetTags(int StoredID)
        {
            try
            {
                var storeTags = await _storeRepository.GetStoreTags(StoredID);
                return Ok(new SharedResponse(true, 200, "", storeTags));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetAllByUserID")]
        public async Task<IActionResult> GetAllByUserID(long Id)
        {
            try
            {
                var storeList = await _storeRepository.GetAllByUserId(Id);
                return Ok(new SharedResponse(true, 200, "", _mapper.Map<List<StoreResponse>>(storeList)));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [AllowAnonymous]
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int Id)
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
                var store = await _storeRepository.GetByStoreId(Id, result);

                if (result == null)
                    return Ok(new SharedResponse(true, 200, "", store));

                AccountVisit accountVisit = new AccountVisit
                {
                    ProviderID = store.ProviderID,
                    UserID = Convert.ToInt64(UserId),
                    UserType = (int)UserType.Provider,
                    FirstVisitDate = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                };
                await _accountVisitRepository.LogAccountVisit(accountVisit);

                return Ok(new SharedResponse(true, 200, "", store));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] StoreRequest storeRequest)
        {
            try
            {
                if (await _storeRepository.CheckDuplicateURL(storeRequest))
                {
                    var store = _mapper.Map<Store>(storeRequest);
                    store.MarkAsDeleted = false;
                    store.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    store.CreatedBy = Convert.ToInt32(UserId);
                    store.Status = (int)StoreStatus.Public;
                    var result = await _storeRepository.AddAsync(store);

                    var storeTags = new List<StoreTag>();
                    foreach (var tag in storeRequest.Tags)
                    {
                        var storeTag = new StoreTag
                        {
                            StoreID = result.ID,
                            TagID = tag.ID
                        };
                        storeTags.Add(storeTag);
                    }
                    var addedStoreTags = await _storeTagRepository.AddRangeAsync(storeTags);

                    var storeImages = new List<StoreImage>();
                    foreach (var image in storeRequest.Images)
                    {
                        var storeImage = new StoreImage
                        {
                            StoreID = result.ID,
                            Image = image.Image
                        };
                        storeImages.Add(storeImage);
                    }
                    await _storeImageRepository.AddRangeAsync(storeImages);
                    return Ok(new SharedResponse(true, 200, "Store Created Successfully", result));
                }
                return Ok(new SharedResponse(false, 400, "Duplicate Links"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] StoreRequest storeRequest)
        {
            try
            {
                if (await _storeRepository.CheckDuplicateURL(storeRequest))
                {
                    var store = await _storeRepository.GetByIdAsync(storeRequest.ID);
                    if (storeRequest.isAddressChanged)
                    {
                        var obj = new StoreAddress();
                        obj.StoreID = store.ID;
                        obj.Location = store.Location;
                        obj.Latitude = store.Latitude;
                        obj.Longitude = store.Latitude;
                        obj.CreatedAt = DateTime.Now;
                        await _storeAddressRepository.AddAsync(obj);

                    }
                    if (store != null)
                    {
                        store.Name = storeRequest.Name;
                        store.Description = storeRequest.Description;
                        store.Location = storeRequest.Location;
                        store.Longitude = storeRequest.Longitude;
                        store.Latitude = storeRequest.Latitude;
                        store.IsAddressHidden = storeRequest.IsAddressHidden;
                        store.VideoURL = storeRequest.VideoURL;
                        store.FacebookURL = storeRequest.FacebookURL;
                        store.TwitterURL = storeRequest.TwitterURL;
                        store.TikTokURL = storeRequest.TikTokURL;
                        store.InstagramURL = storeRequest.InstagramURL;
                        if (!string.IsNullOrEmpty(storeRequest.BusinessLogo))
                        {
                            store.BusinessLogo = storeRequest.BusinessLogo;
                        }
                        if (!string.IsNullOrEmpty(storeRequest.InstagramGallery))
                        {
                            store.InstagramGallery = storeRequest.InstagramGallery;
                        }
                        store.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                        store.UpdatedBy = Convert.ToInt32(UserId);
                        await _storeRepository.UpdateAsync(store);

                        var existingstoreTags = (await _storeTagRepository.ListAllAsync()).Where(x => x.StoreID == storeRequest.ID).ToList();
                        await _storeTagRepository.DeleteRangeAsync(existingstoreTags);
                        var storeTags = new List<StoreTag>();
                        foreach (var tag in storeRequest.Tags)
                        {
                            var storeTag = new StoreTag
                            {
                                StoreID = storeRequest.ID,
                                TagID = tag.ID
                            };
                            storeTags.Add(storeTag);
                        }
                        var addedItemTags = await _storeTagRepository.AddRangeAsync(storeTags);

                        var existingImages = (await _storeImageRepository.ListAllAsync()).Where(x => x.StoreID == storeRequest.ID).ToList();
                        if (storeRequest.Images.Any())
                        {
                            await _storeImageRepository.DeleteRangeAsync(existingImages);
                            var storeImages = new List<StoreImage>();
                            foreach (var image in storeRequest.Images)
                            {
                                var storeImage = new StoreImage
                                {
                                    StoreID = store.ID,
                                    Image = image.Image
                                };
                                storeImages.Add(storeImage);
                            }
                            await _storeImageRepository.AddRangeAsync(storeImages);
                        }

                        return Ok(new SharedResponse(true, 200, "Store Updated Successfully"));
                    }
                    else
                    {
                        return Ok(new SharedResponse(false, 400, "unable to find Store"));
                    }
                }
                return Ok(new SharedResponse(false, 400, "Duplicate Links"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Report")]
        public async Task<IActionResult> Report([FromBody] ReportStoreRequest reportStoreRequest)
        {
            try
            {
                var store = await _storeRepository.GetByIdAsync(reportStoreRequest.StoreID);
                if (store != null)
                {
                    ReportedStore reportedStore = new ReportedStore
                    {
                        StoreID = reportStoreRequest.StoreID,
                        UserID = Convert.ToInt64(UserId),
                        Comments = reportStoreRequest.Comments,
                        RecordDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                    };
                    await _reportedStoreRepository.AddAsync(reportedStore);
                    return Ok(new SharedResponse(true, 200, "Store Reported Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable to Find Store"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("AddFavourite")]
        public async Task<IActionResult> AddFavourite([FromBody] RequestID request)
        {
            try
            {
                var store = await _favouriteStoreRepository.GetByStoreID(request.ID, Convert.ToInt64(UserId));
                if (store == null)
                {
                    FavouriteStore favouriteStore = new FavouriteStore
                    {
                        StoreID = request.ID,
                        UserID = Convert.ToInt64(UserId),
                        RecordDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                    };
                    await _favouriteStoreRepository.AddAsync(favouriteStore);
                    return Ok(new SharedResponse(true, 200, "Store Added in Favourite List Successfully"));
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
        public async Task<IActionResult> RemoveFavourite([FromBody] RequestID request)
        {
            try
            {
                var store = await _favouriteStoreRepository.GetByStoreID(request.ID, Convert.ToInt64(UserId));
                if (store != null)
                {
                    await _favouriteStoreRepository.DeleteAsync(store);
                    return Ok(new SharedResponse(true, 200, "Store Removed from Favourite List Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable to Find Store in Favourite List"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpGet("GetFavourite")]
        public async Task<IActionResult> GetFavourite()
        {
            try
            {
                var storeList = await _storeRepository.GetFavouriteStore(Convert.ToInt64(UserId));
                return Ok(new SharedResponse(true, 200, "", storeList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpDelete("Delete/{ID}")]
        public async Task<IActionResult> Delete(long ID)
        {
            try
            {
                var store = await _storeRepository.GetByIdAsync(ID);
                if (store != null)
                {
                    store.MarkAsDeleted = true;
                    store.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    store.UpdatedBy = Convert.ToInt32(UserId);
                    await _storeRepository.UpdateAsync(store);
                    return Ok(new SharedResponse(true, 200, "Store Deleted Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Store"));
                }
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
                    FileUploadResponse _RESPONSE = await obj.UploadImageFile(file, "StoreImage");
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

        [AllowAnonymous]
        // Rating and Reviews
        [HttpPost("AddRatingAndReview")]
        public async Task<IActionResult> AddRatingAndReview([FromBody] StoreRatingAndReviewRequest request)
        {
            try
            {
                var storeReview = new StoreReview
                {
                    StoreID = request.StoreID,
                    UserID = request.UserID,
                    DriverID = request.DriverID,
                    OrderID = request.OrderID,
                    Rating = request.Rating,
                    Reviews = request.Reviews,
                    RecordDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                };
                await _storeReviewRepository.AddAsync(storeReview);

                List<ItemReview> itemReviews = new List<ItemReview>();
                foreach (var item in request.Items)
                {
                    ItemReview itemReview = new ItemReview
                    {
                        UserID = request.UserID,
                        OrderID = request.OrderID,
                        ItemID = item.ItemID,
                        Rating = item.Rating,
                        Review = item.Reviews,
                        Image = item.Image,
                        RecordDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time"),
                        MarkAsDeleted = false
                    };
                    itemReviews.Add(itemReview);
                }
                await _itemReviewRepository.AddRangeAsync(itemReviews);

                // Notification

                Order order = await _orderRepository.GetByIdAsync(request.OrderID);
                Provider provider = await _providerOrderRepository.GetProviderByOrderID(order.ID);
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

                if (request.UserID > 0)
                {
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.ProviderRatedByBuyer;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Buyer;
                    SubjectID = request.OrderID.ToString();
                }
                else if (request.DriverID > 0)
                {
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.ProviderRatedByDriver;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Driver;
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
        [HttpPost("GetReviewsAndRatings")]
        public async Task<IActionResult> GetReviewsAndRatings([FromBody] StoreReviewAndRatingFilterRequest request)
        {
            try
            {
                var response = await _storeRepository.GetReviewsAndRatings(request);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost]
        [Route("GetStoreOldAddress")]
        public async Task<IActionResult> GetStoreOldAddress(int storeID)
        {
            try
            {
                if (storeID > 0)
                {
                    var getAddresslist = await _storeAddressRepository.GetAllByStoreId(storeID);
                    if (getAddresslist.Count > 0)
                    {

                        return Ok(new SharedResponse(true, 200, "", getAddresslist));
                    }
                    else
                    {
                        return Ok(new SharedResponse(false, 400, "No Record found"));
                    }

                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Invalid ID!"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }
    }
}
