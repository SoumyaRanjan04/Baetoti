using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.API.Helpers;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Core.Interface.Services;
using Baetoti.Infrastructure.Data.Repositories;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Request.ChangeItem;
using Baetoti.Shared.Request.Item;
using Baetoti.Shared.Request.Notification;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.ChangeItem;
using Baetoti.Shared.Response.Dashboard;
using Baetoti.Shared.Response.FileUpload;
using Baetoti.Shared.Response.Item;
using Baetoti.Shared.Response.Shared;
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
    public class ItemController : ApiBaseController
    {

        public readonly IItemRepository _itemRepository;
        public readonly IItemImageRepository _itemImageRepository;
        public readonly IChangeItemImageRepository _changeItemImageRepository;
        public readonly IItemTagRepository _itemTagRepository;
        public readonly IChangeItemRepository _ChangeitemRepository;
        public readonly IChangeItemTagRepository _ChangeitemTagRepository;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPushNotificationRepository _pushNotificationRepository;
        private readonly IProviderOrderRepository _providerOrderRepository;
        private readonly IDriverConfigRepository _driverConfigRepository;
        private readonly IItemVisitRepository _itemVisitRepository;
        public readonly IMapper _mapper;

        public ItemController(
            ISiteConfigService siteConfigService,
            IItemRepository itemRepository,
            IItemImageRepository itemImageRepository,
            IChangeItemImageRepository changeItemImageRepository,
            IItemTagRepository itemTagRepository,
            IChangeItemRepository changeitemRepository,
            IChangeItemTagRepository changeitemTagRepository,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,
            IPushNotificationRepository pushNotificationRepository,
            IProviderOrderRepository providerOrderRepository,
            IDriverConfigRepository driverConfigRepository,
            IItemVisitRepository itemVisitRepository,
            IMapper mapper
            ) : base(siteConfigService)
        {
            _itemRepository = itemRepository;
            _itemTagRepository = itemTagRepository;
            _itemImageRepository = itemImageRepository;
            _ChangeitemRepository = changeitemRepository;
            _ChangeitemTagRepository = changeitemTagRepository;
            _changeItemImageRepository = changeItemImageRepository;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _pushNotificationRepository = pushNotificationRepository;
            _providerOrderRepository = providerOrderRepository;
            _driverConfigRepository = driverConfigRepository;
            _itemVisitRepository = itemVisitRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(decimal Longitude, decimal Latitude, int ItemFilter = 0, int PageNumber = 1, int PageSize = 10)
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
                var itemList = await _itemRepository.GetAll(Longitude,Latitude, ItemFilter, result, UserRole, PageNumber, PageSize);
                return Ok(new SharedResponse(true, 200, "", itemList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [AllowAnonymous]
        [HttpGet("GetAllForDashboad")]
        public async Task<IActionResult> GetAllForDashboad(decimal Longitude, decimal Latitude)
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

                var itemList = await _itemRepository.GetAllForDashboard(result, Longitude,  Latitude);
                return Ok(new SharedResponse(true, 200, "", itemList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [AllowAnonymous]
        [HttpPost("GetFilteredData")]
        public async Task<IActionResult> GetFilteredData([FromBody] ItemFilterRequest filterRequest)
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
                var itemList = await _itemRepository.GetFilteredItemsDataAsync(filterRequest, result);
                return Ok(new SharedResponse(true, 200, "", itemList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {
            try
            {
                var item = await _itemRepository.GetByID(Id, long.Parse(UserId));

                if (string.IsNullOrEmpty(UserRole))
                {
                    ItemVisit itemVisit = new ItemVisit
                    {
                        ItemID = Id,
                        UserID = long.Parse(UserId),
                        FirstVisitDate = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                    };
                    await _itemVisitRepository.LogItemVisit(itemVisit);
                }

                return Ok(new SharedResponse(true, 200, "", item));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] ItemRequest itemRequest)
        {
            try
            {
                DriverConfig config = (await _driverConfigRepository.ListAllAsync()).OrderByDescending(oo => oo.UpdatedBy).FirstOrDefault();
                var item = new Core.Entites.Item
                {
                    Name = itemRequest.Name,
                    ArabicName = itemRequest.ArabicName,
                    Description = itemRequest.Description,
                    CategoryID = itemRequest.CategoryID,
                    SubCategoryID = itemRequest.SubCategoryID,
                    ProviderID = itemRequest.ProviderID,
                    UnitID = itemRequest.UnitID,
                    Price = itemRequest.Price,
                    AvailableQuantity = itemRequest.AvailableQuantity,
                    BaetotiPrice = config == null ? itemRequest.BaetotiPrice : (config.ProviderComission / 100 * itemRequest.Price) + itemRequest.Price,
                    ItemStatus = (int)ItemStatus.Pending,
                    MarkAsDeleted = false,
                    CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time"),
                    CreatedBy = Convert.ToInt32(UserId),
                    RequestDate = DateTime.Now.ToTimeZoneTime("Arab Standard Time"),
                    IsCheckQuantity = itemRequest.IsCheckQuantity
                };
                var addedItem = await _itemRepository.AddAsync(item);

                var itemTags = new List<ItemTag>();
                foreach (var tag in itemRequest.Tags)
                {
                    var itemTag = new ItemTag
                    {
                        ItemID = addedItem.ID,
                        TagID = tag.ID
                    };
                    itemTags.Add(itemTag);
                }
                var addedItemTags = await _itemTagRepository.AddRangeAsync(itemTags);

                var itemImages = new List<ItemImage>();
                foreach (var image in itemRequest.Images)
                {
                    var itemImage = new ItemImage
                    {
                        Image = image.Image,
                        ItemID = addedItem.ID
                    };
                    itemImages.Add(itemImage);
                }
                await _itemImageRepository.AddRangeAsync(itemImages);

                return Ok(new SharedResponse(true, 200, "Item Created Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] ItemRequest itemRequest)
        {
            try
            {
                DriverConfig config = (await _driverConfigRepository.ListAllAsync()).OrderByDescending(o => o.ID).FirstOrDefault();
                var changeitem = new ChangeItem
                {
                    ItemId = itemRequest.ID,
                    Name = itemRequest.Name,
                    ArabicName = itemRequest.ArabicName,
                    Description = itemRequest.Description,
                    CategoryID = itemRequest.CategoryID,
                    SubCategoryID = itemRequest.SubCategoryID,
                    ProviderID = itemRequest.ProviderID,
                    UnitID = itemRequest.UnitID,
                    Price = itemRequest.Price,
                    AvailableQuantity = itemRequest.AvailableQuantity,
                    BaetotiPrice = config == null ? itemRequest.BaetotiPrice : (config.ProviderComission / 100 * itemRequest.Price) + itemRequest.Price,
                    ItemStatus = 2,
                    IsCheckQuantity = itemRequest.IsCheckQuantity,
                    RequestDate = DateTime.Now.ToTimeZoneTime("Arab Standard Time"),
                    RequestedBy = long.Parse(UserId)
                };
                var addedChangeItem = await _ChangeitemRepository.AddAsync(changeitem);

                var changeitemTags = new List<ChangeItemTag>();
                foreach (var tag in itemRequest.Tags)
                {
                    var changeitemTag = new ChangeItemTag
                    {
                        ItemID = itemRequest.ID,
                        ChangeItemID = addedChangeItem.ID,
                        TagID = tag.ID,

                    };
                    changeitemTags.Add(changeitemTag);
                }
                var addedChangeItemTags = await _ChangeitemTagRepository.AddRangeAsync(changeitemTags);

                var changeItemImages = new List<ChangeItemImage>();
                foreach (var image in itemRequest.Images)
                {
                    var changeItemImage = new ChangeItemImage
                    {
                        Image = image.Image,
                        ItemID = itemRequest.ID,
                        ChangeItemID = addedChangeItem.ID
                    };
                    changeItemImages.Add(changeItemImage);
                }
                await _changeItemImageRepository.AddRangeAsync(changeItemImages);

                return Ok(new SharedResponse(true, 200, "Item Update Request Sent Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost("LogVisit")]
        public async Task<IActionResult> LogVisit([FromBody] RequestID request)
        {
            try
            {
                if (string.IsNullOrEmpty(UserId))
                {
                    return Ok(new SharedResponse(true, 200, "Item Visit Logged Successfully"));
                }
                else
                {
                    ItemVisit itemVisit = new ItemVisit
                    {
                        ItemID = request.ID,
                        UserID = Convert.ToInt64(UserId),
                        FirstVisitDate = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                    };
                    await _itemVisitRepository.LogItemVisit(itemVisit);
                    return Ok(new SharedResponse(true, 200, "Item Visit Logged Successfully"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpDelete("Delete/{ID}")]
        public async Task<IActionResult> Delete(long ID)
        {
            try
            {
                var item = await _itemRepository.GetByIdAsync(ID);
                if (item != null)
                {
                    item.MarkAsDeleted = true;
                    item.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    item.UpdatedBy = Convert.ToInt32(UserId);
                    await _itemRepository.UpdateAsync(item);
                    return Ok(new SharedResponse(true, 200, "Item Deleted Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Item"));
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
                    FileUploadResponse _RESPONSE = await obj.UploadImageFile(file, "Item");
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

        [HttpPost("GetItemsOnBoardingRequest")]
        public async Task<IActionResult> GetItemsOnBoardingRequest(ItemOnBoardingRequest itemOnBoardingRequest)
        {
            try
            {
                ItemsOnBoardingResponse response = await _itemRepository.GetAllItemsOnBoardingRequest(itemOnBoardingRequest);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        //Change Item  Requests
        [HttpGet("GetAllChangeRequest")]
        public async Task<IActionResult> GetAllChangeRequest()
        {
            try
            {
                var changeitemList = await _ChangeitemRepository.GetAllChangeRequest();
                return Ok(new SharedResponse(true, 200, "", changeitemList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("View")]
        public async Task<IActionResult> View(long Id)
        {
            try
            {
                var changeitem = await _ChangeitemRepository.GetByItemID(Id);
                return Ok(new SharedResponse(true, 200, "", changeitem));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("ViewRequest")]
        public async Task<IActionResult> ViewRequest(long ItemID, long ID)
        {
            try
            {
                var comparisonResponse = await _itemRepository.ViewByID(ItemID, ID);
                return Ok(new SharedResponse(true, 200, "", comparisonResponse));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] ItemRequestApprovel itemRequest)
        {
            try
            {
                // Active or InActive
                if (itemRequest.StatusValue == (int)ItemStatus.Active || itemRequest.StatusValue == (int)ItemStatus.Inactive)
                {
                    var item = await _itemRepository.GetByIdAsync(itemRequest.ItemID);
                    item.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    item.UpdatedBy = Convert.ToInt32(UserId);
                    item.ItemStatus = itemRequest.StatusValue;
                    await _itemRepository.UpdateAsync(item);
                    return Ok(new SharedResponse(true, 200, "Item Status Updated Successfully"));
                }
                // OnBoarding Request
                if (itemRequest.ItemRequestType == (int)ItemRequestType.Onboarding)
                {
                    var item = await _itemRepository.GetByIdAsync(itemRequest.ItemID);
                    item.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    item.UpdatedBy = Convert.ToInt32(UserId);
                    item.ItemStatus = itemRequest.StatusValue;
                    item.RequestCloseDate = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    item.RequestClosedBy = Convert.ToInt32(UserId);
                    await _itemRepository.UpdateAsync(item);
                    return Ok(new SharedResponse(true, 200, "Item Status Updated Successfully"));
                }
                else // Change Request
                {
                    var changeitem = await _ChangeitemRepository.GetByIdAsync(itemRequest.ID);
                    if (changeitem != null)
                    {
                        if (itemRequest.StatusValue == (int)ItemStatus.Approved)
                        {
                            var item = await _itemRepository.GetByIdAsync(changeitem.ItemId);
                            item.Name = changeitem.Name;
                            item.ArabicName = changeitem.ArabicName;
                            item.Description = changeitem.Description;
                            item.CategoryID = changeitem.CategoryID;
                            item.SubCategoryID = changeitem.SubCategoryID;
                            item.ProviderID = changeitem.ProviderID;
                            item.UnitID = changeitem.UnitID;
                            item.Price = changeitem.Price;
                            item.BaetotiPrice = changeitem.BaetotiPrice;
                            item.AvailableQuantity = changeitem.AvailableQuantity;
                            item.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                            item.UpdatedBy = Convert.ToInt32(UserId);
                            item.ItemStatus = (int)ItemStatus.Approved;
                            item.RequestCloseDate = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                            item.RequestClosedBy = Convert.ToInt32(UserId);
                            item.IsCheckQuantity = changeitem.IsCheckQuantity;
                            await _itemRepository.UpdateAsync(item);

                            var existingchangeItemTags = (await _ChangeitemTagRepository.ListAllAsync()).Where(x => x.ItemID == itemRequest.ItemID && x.ChangeItemID == itemRequest.ID).ToList();
                            if (existingchangeItemTags.Any())
                            {
                                var existingItemTags = (await _itemTagRepository.ListAllAsync()).Where(x => x.ItemID == itemRequest.ItemID).ToList();
                                await _itemTagRepository.DeleteRangeAsync(existingItemTags);

                                var itemTags = new List<ItemTag>();
                                foreach (var tag in existingchangeItemTags)
                                {
                                    var itemTag = new ItemTag
                                    {
                                        ItemID = tag.ItemID,
                                        TagID = tag.TagID
                                    };
                                    itemTags.Add(itemTag);
                                }
                                var addedItemTags = await _itemTagRepository.UpdateRangeAsync(itemTags);
                            }

                            var existingChanedImages = (await _changeItemImageRepository.ListAllAsync()).Where(x => x.ItemID == itemRequest.ItemID && x.ChangeItemID == itemRequest.ID).ToList();
                            if (existingChanedImages.Any())
                            {
                                var existingImages = (await _itemImageRepository.ListAllAsync()).Where(x => x.ItemID == itemRequest.ItemID).ToList();
                                await _itemImageRepository.DeleteRangeAsync(existingImages);
                                var itemImages = new List<ItemImage>();
                                foreach (var image in existingChanedImages)
                                {
                                    var itemImage = new ItemImage
                                    {
                                        Image = image.Image,
                                        ItemID = itemRequest.ItemID
                                    };
                                    itemImages.Add(itemImage);
                                }
                                await _itemImageRepository.AddRangeAsync(itemImages);
                            }

                            changeitem.ItemStatus = itemRequest.StatusValue;
                            changeitem.Comments = itemRequest.Comments;
                            changeitem.RequestClosedBy = Convert.ToInt32(UserId);
                            changeitem.RequestCloseDate = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                            await _ChangeitemRepository.UpdateAsync(changeitem);
                        }
                        else
                        {
                            changeitem.ItemStatus = itemRequest.StatusValue;
                            changeitem.Comments = itemRequest.Comments;
                            changeitem.RequestClosedBy = Convert.ToInt32(UserId);
                            changeitem.RequestCloseDate = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                            await _ChangeitemRepository.UpdateAsync(changeitem);
                        }

                        // Notification

                        string DestinationUserID = "";
                        int DestinationUserType = 0;
                        int NotificationTypeID = 0;
                        string SenderUser = "";
                        int SenderUserType = 0;
                        string SubjectID = "";

                        if (itemRequest.StatusValue == (int)ItemStatus.Approved)
                        {
                            Core.Entites.Provider provider = await _providerOrderRepository.GetProviderByItemID(itemRequest.ID);
                            DestinationUserID = provider.UserID.ToString();
                            DestinationUserType = (int)UserType.Provider;
                            NotificationTypeID = (int)NotificationType.ItemChangeRequestApproved;
                            SenderUser = UserId;
                            SenderUserType = (int)UserType.Admin;
                            SubjectID = changeitem.ItemId.ToString();
                        }
                        else if (itemRequest.StatusValue == (int)ItemStatus.Rejected)
                        {
                            Core.Entites.Provider provider = await _providerOrderRepository.GetProviderByItemID(itemRequest.ID);
                            DestinationUserID = provider.UserID.ToString();
                            DestinationUserType = (int)UserType.Provider;
                            NotificationTypeID = (int)NotificationType.ItemChangeRequestRejected;
                            SenderUser = UserId;
                            SenderUserType = (int)UserType.Admin;
                            SubjectID = changeitem.ItemId.ToString();
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

                        return Ok(new SharedResponse(true, 200, "Item Status Updated Successfully"));
                    }
                    else
                    {
                        return Ok(new SharedResponse(false, 400, "Unable to Find Change Request"));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }


        [HttpPost("UpdateAllBaetotiPrice")]
        public async Task<IActionResult> UpdateAllBaetotiPrice()
        {
            try
            {
                var config = (await _driverConfigRepository.ListAllAsync()).OrderByDescending(o => o.ID).FirstOrDefault();
                var itemList = await _itemRepository.GetListofAllItems();
                //itemList.ForEach(item => item.BaetotiPrice = (config.ProviderComission / 100 * item.Price) + item.Price);
                foreach (var item in itemList)
                {
                    item.BaetotiPrice = (config.ProviderComission / 100 * item.Price) + item.Price;
                   await _itemRepository.UpdateAsync(item);
                }
               
                return Ok(new SharedResponse(true, 200, "Item Baetoti Prise is Updated."));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }
    }
}
