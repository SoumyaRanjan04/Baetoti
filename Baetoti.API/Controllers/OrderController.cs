using Baetoti.API.Controllers.Base;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Request.Notification;
using Baetoti.Shared.Request.Order;
using Baetoti.Shared.Request.OrderRequest;
using Baetoti.Shared.Request.Payment;
using Baetoti.Shared.Request.ReportedOrder;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Payment;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Driver = Baetoti.Core.Entites.Driver;
using NotificationType = Baetoti.Shared.Enum.NotificationType;
using OrderRequest = Baetoti.Shared.Request.Order.OrderRequest;
using Provider = Baetoti.Core.Entites.Provider;

namespace Baetoti.API.Controllers
{
    public class OrderController : ApiBaseController
    {

        private readonly IOrderRepository _orderRepository;
        private readonly IDriverOrderRepository _driverOrderRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly IProviderOrderRepository _providerOrderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRequestRepository _orderRequestRepository;
        private readonly ICartRepository _cartRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IReportedOrderRepository _reportedOrderRepository;
        private readonly ISupportRequestRepository _supportRequestRepository;
        private readonly INotificationService _notificationService;
        private readonly IPaymentAPIService _paymentAPIService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPushNotificationRepository _pushNotificationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;

        public OrderController(
            ISiteConfigService siteConfigService,
            IOrderRepository cartRepository,
            IOrderItemRepository orderRepository,
            IOrderRequestRepository orderRequestRepository,
            IDriverOrderRepository driverOrderRepository,
            IDriverRepository driverRepository,
            IProviderOrderRepository providerOrderRepository,
            ICartRepository usercartRepository,
            ITransactionRepository transactionRepository,
            IItemRepository itemRepository,
            IReportedOrderRepository reportedOrderRepository,
            ISupportRequestRepository supportRequestRepository,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,
            IPaymentAPIService paymentAPIService,
            IPushNotificationRepository pushNotificationRepository,
            IUserRepository userRepository,
            IStoreRepository storeRepository
            ) : base(siteConfigService)
        {
            _orderRepository = cartRepository;
            _orderItemRepository = orderRepository;
            _orderRequestRepository = orderRequestRepository;
            _driverOrderRepository = driverOrderRepository;
            _driverRepository = driverRepository;
            _providerOrderRepository = providerOrderRepository;
            _cartRepository = usercartRepository;
            _transactionRepository = transactionRepository;
            _itemRepository = itemRepository;
            _reportedOrderRepository = reportedOrderRepository;
            _supportRequestRepository = supportRequestRepository;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _paymentAPIService = paymentAPIService;
            _pushNotificationRepository = pushNotificationRepository;
            _userRepository = userRepository;
            _storeRepository = storeRepository;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] OrderRequest orderRequest)
        {
            try
            {
                foreach (var requestedItem in orderRequest.Items)
                {
                    var item = await _itemRepository.GetByIdAsync(requestedItem.ItemID);
                    if (item.IsCheckQuantity)
                    {
                        if (item.AvailableQuantity == 0)
                            return Ok(new SharedResponse(false, 400, "This item is not availabe in the stock."));
                        else if (item.AvailableQuantity < requestedItem.Quantity)
                            return Ok(new SharedResponse(false, 400, $"{item.Name} has {item.AvailableQuantity} available quantity in the stock."));
                        item.AvailableQuantity = item.AvailableQuantity - requestedItem.Quantity;
                        await _itemRepository.UpdateAsync(item);
                    }
                }
                var cart = new Order
                {
                    UserID = orderRequest.UserID,
                    NotesForDriver = orderRequest.NotesForDriver,
                    DeliveryAddress = orderRequest.DeliveryAddress,
                    AddressLatitude = orderRequest.AddressLatitude,
                    AddressLongitude = orderRequest.AddressLongitude,
                    IsPickBySelf = orderRequest.IsPickBySelf,
                    ExpectedDeliveryTime = orderRequest.ExpectedDeliveryTime,
                    Status = (int)OrderStatus.Pending,
                    CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time"),
                    CreatedBy = Convert.ToInt32(UserId)
                };
                var addedCart = await _orderRepository.AddAsync(cart);

                var orders = new List<OrderItem>();
                foreach (var requestedItem in orderRequest.Items)
                {
                    var item = await _itemRepository.GetByIdAsync(requestedItem.ItemID);
                    var order = new OrderItem
                    {
                        OrderID = addedCart.ID,
                        ItemID = requestedItem.ItemID,
                        Quantity = requestedItem.Quantity,
                        Comments = requestedItem.Comments,
                        SoldPrice = item.BaetotiPrice
                    };
                    orders.Add(order);
                }
                await _orderItemRepository.AddRangeAsync(orders);

                var existingCart = await _cartRepository.GetByUserID(Convert.ToInt64(UserId));
                if (existingCart != null)
                {
                    await _cartRepository.DeleteRangeAsync(existingCart);
                }
                
                Transaction transaction = new Transaction()
                {
                    UserID = addedCart.UserID,
                    OrderID = addedCart.ID,
                    Amount = orderRequest.OrderAmount,
                    DeliveryCharges = !orderRequest.IsPickBySelf ? orderRequest.DeliveryCharges : 0,
                    ProviderCommision = orderRequest.ProviderCommision,
                    
                    DriverCommision = orderRequest.IsPickBySelf ? 0 : orderRequest.DriverCommision,

                    ServiceFee = orderRequest.ServiceFee,
                    TransactionType = (int)TransactionType.Online,
                    TransactionTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time"),
                    Discount = orderRequest.Discount
                };
                await _transactionRepository.AddAsync(transaction);

                Provider provider = await _providerOrderRepository.GetProviderByItemID(orderRequest.Items.FirstOrDefault().ItemID);
                ProviderOrder providerOrder = new ProviderOrder
                {
                    OrderID = addedCart.ID,
                    ProviderID = provider.ID,
                    Status = (int)ProviderOrderStatus.Pending,
                    CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time"),
                    CreatedBy = Convert.ToInt32(UserId)
                };
                await _providerOrderRepository.AddAsync(providerOrder);

                User user = await _userRepository.GetByIdAsync(addedCart.UserID);
                Store store = await _storeRepository.GetByProviderId(provider.ID);

                var notification = await _pushNotificationRepository.GetByNotificationType((int)NotificationType.NewOrderForProvider);
                CreateNotificationRequest createNotificationRequest = new CreateNotificationRequest
                {
                    DestUserID = provider.UserID.ToString(),
                    Type = notification.NotificationTypeID,
                    DestUserType = (int)UserType.Provider,
                    NotificationTitle = notification.Title.Replace("{buyername}", $"{user.FirstName}")
                                                          .Replace("{providername}", $"{store.Name}"),
                    NotificationBody = notification.Text.Replace("{buyername}", $"{user.FirstName}")
                                                          .Replace("{providername}", $"{store.Name}"),
                    SenderUser = UserId,
                    SenderUserType = (int)UserType.Buyer,
                    SubjectID = providerOrder.OrderID.ToString()
                };
                await _notificationService.CreatePushNotification(createNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);

                return Ok(new SharedResponse(true, 200, "Order Submitted Successfully", cart));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("AssignDriver")]
        public async Task<IActionResult> AssignDriver([FromBody] AssignOrderRequest request)
        {
            try
            {
                Core.Entites.OrderRequest orderRequest = new Core.Entites.OrderRequest()
                {
                    OrderID = request.OrderID,
                    DriverID = request.DriverID,
                    StoreID = request.StoreID,
                    RequestStatus = (int)OrderRequestStatus.Pending,
                    RequestDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                };
                await _orderRequestRepository.AddAsync(orderRequest);

                Driver driver = await _driverRepository.GetByIdAsync(orderRequest.DriverID);
                Order order = await _orderRepository.GetByIdAsync(request.OrderID);
                Provider provider = await _providerOrderRepository.GetProviderByOrderID(order.ID);
                User user = await _userRepository.GetByIdAsync(order.UserID);
                Store store = await _storeRepository.GetByProviderId(provider.ID);
                User driverUser = driver != null ? await _userRepository.GetByIdAsync(driver.UserID) : null;

                var notification = await _pushNotificationRepository.GetByNotificationType((int)NotificationType.DriverDeliveryRequest);
                CreateNotificationRequest createNotificationRequest = new CreateNotificationRequest()
                {
                    DestUserID = driver.UserID.ToString(),
                    Type = notification.NotificationTypeID,
                    DestUserType = (int)UserType.Driver,
                    NotificationTitle = notification.Title.Replace("{buyername}", $"{user.FirstName}")
                    .Replace("{providername}", $"{store.Name}").Replace("{drivername}", driverUser != null ? $"{driverUser.FirstName}" : "Driver"),
                    NotificationBody = notification.Text.Replace("{buyername}", $"{user.FirstName}")
                    .Replace("{providername}", $"{store.Name}").Replace("{drivername}", driverUser != null ? $"{driverUser.FirstName}" : "Driver"),
                    SenderUser = UserId,
                    SenderUserType = (int)UserType.Provider,
                    SubjectID = request.OrderID.ToString()
                };
                await _notificationService.CreatePushNotification(createNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);


                return Ok(new SharedResponse(true, 200, "Order Request Submitted Successfully", orderRequest));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("AutoAssignDriver")]
        public async Task<IActionResult> AutoAssignDriver([FromBody] AssignOrderRequest request)
        {
            try
            {
                var driverList = await _driverRepository.GetNearBy(Convert.ToInt64(UserId), request.LocationRange);
                List<Core.Entites.OrderRequest> orderRequests = new List<Core.Entites.OrderRequest>();

                Order order = await _orderRepository.GetByIdAsync(request.OrderID);
                Provider provider = await _providerOrderRepository.GetProviderByOrderID(order.ID);
                User user = await _userRepository.GetByIdAsync(order.UserID);
                Store store = await _storeRepository.GetByProviderId(provider.ID);

                foreach (var driver in driverList)
                {
                    Core.Entites.OrderRequest orderRequest = new Core.Entites.OrderRequest()
                    {
                        OrderID = request.OrderID,
                        DriverID = driver.ID,
                        StoreID = request.StoreID,
                        RequestStatus = (int)OrderRequestStatus.Pending,
                        RequestDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                    };
                    orderRequests.Add(orderRequest);

                    var notification = await _pushNotificationRepository.GetByNotificationType((int)NotificationType.DriverDeliveryRequest);
                    CreateNotificationRequest createNotificationRequest = new CreateNotificationRequest()
                    {
                        DestUserID = driver.UserID.ToString(),
                        Type = notification.NotificationTypeID,
                        DestUserType = (int)UserType.Driver,
                        NotificationTitle = notification.Title.Replace("{buyername}", $"{user.FirstName}")
                                                              .Replace("{providername}", $"{store.Name}"),
                        NotificationBody = notification.Text.Replace("{buyername}", $"{user.FirstName}")
                                                              .Replace("{providername}", $"{store.Name}"),
                        SenderUser = UserId,
                        SenderUserType = (int)UserType.Provider,
                        SubjectID = request.OrderID.ToString()
                    };
                    await _notificationService.CreatePushNotification(createNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);
                }
                await _orderRequestRepository.AddRangeAsync(orderRequests);
                return Ok(new SharedResponse(true, 200, "Order Request Submitted Successfully", orderRequests));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] OrderUpdateRequest orderRequest)
        {
            try
            {
                Order order = await _orderRepository.GetByIdAsync(orderRequest.OrderId);
                if (orderRequest.DeliveryDate != null && !string.IsNullOrEmpty(orderRequest.DeliveryTime))
                {
                    order.ExpectedDeliveryTime = Convert.ToDateTime($"{Convert.ToDateTime(orderRequest.DeliveryDate).ToShortDateString()} {orderRequest.DeliveryTime}");
                    await _orderRepository.UpdateAsync(order);
                }
                if (orderRequest.DriverId > 0)
                {
                    DriverOrder driverOrder = await _driverOrderRepository.GetByOrderID(orderRequest.OrderId);
                    driverOrder.DriverID = orderRequest.DriverId;
                    await _driverOrderRepository.UpdateAsync(driverOrder);
                }
                if (orderRequest.ProviderId > 0)
                {
                    ProviderOrder providerOrder = await _providerOrderRepository.GetByOrderID(orderRequest.OrderId);
                    providerOrder.ProviderID = orderRequest.ProviderId;
                    await _providerOrderRepository.UpdateAsync(providerOrder);
                }
                return Ok(new SharedResponse(true, 200, "Order Updated Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll(DateRangeFilter DateRange)
        {
            try
            {
                var orderList = await _orderItemRepository.GetAll(DateRange);
                return Ok(new SharedResponse(true, 200, "", orderList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("TrackOrder")]
        public async Task<IActionResult> TrackOrder(TrackOrderRequest request)
        {
            try
            {
                var orderList = await _orderItemRepository
                    .TrackOrderByUserID(Convert.ToInt64(UserId), request);
                return Ok(new SharedResponse(true, 200, "", orderList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetUserOrder")]
        public async Task<IActionResult> GetUserOrder(OrderStatusSearchRequest request)
        {
            try
            {
                var orderList = await _orderItemRepository
                    .GetAllByUserID(Convert.ToInt64(UserId), request);
                return Ok(new SharedResponse(true, 200, "", orderList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetUserOrderByID")]
        public async Task<IActionResult> GetUserOrderByID(int Id)
        {
            try
            {
                var item = await _orderItemRepository.GetUserOrderByID(Id);
                return Ok(new SharedResponse(true, 200, "", item));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetByID")]
        public async Task<IActionResult> GetByID(int Id)
        {
            try
            {
                var order = await _orderItemRepository.GetByID(Id);
                return Ok(new SharedResponse(true, 200, "", order));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetFilteredData")]
        public async Task<IActionResult> GetFilteredData([FromBody] OrderFilterRequest orderFilterRequest)
        {
            try
            {
                var orderList = await _orderItemRepository.GetFilteredData(orderFilterRequest);
                return Ok(new SharedResponse(true, 200, "", orderList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            try
            {
                Order order = await _orderRepository.GetByIdAsync(request.ID);
                Provider provider = await _providerOrderRepository.GetProviderByOrderID(order.ID);
                Driver driver = await _driverOrderRepository.GetDriverByOrderID(order.ID);
                User user = await _userRepository.GetByIdAsync(order.UserID);
                Store store = await _storeRepository.GetByProviderId(provider.ID);
                User driverUser = driver != null ? await _userRepository.GetByIdAsync(driver.UserID) : null;
                Transaction transaction = await _transactionRepository.GetByOrderID(order.ID);
                if (order == null)
                    return Ok(new SharedResponse(false, 400, "Unable to find Order"));
                if (request.Value == (int)OrderStatus.CancelledByDriver)
                {
                    DriverOrder driverOrder = await _driverOrderRepository.GetByOrderID(request.ID);
                    driverOrder.Status = (int)DriverOrderStatus.Cancelled;
                    driverOrder.Comments = request.Comments;
                    await _driverOrderRepository.UpdateAsync(driverOrder);

                    Core.Entites.OrderRequest orderRequest = await _orderRequestRepository.GetByOrderID(order.ID);
                    orderRequest.RequestStatus = (int)OrderRequestStatus.Cancelled;
                    await _orderRequestRepository.UpdateAsync(orderRequest);
                }
                else
                {
                    order.Status = request.Value;
                    order.UpdatedBy = Convert.ToInt32(UserId);
                    order.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    if (request.UserType == (int)UserType.Provider)
                        order.ProviderComments = request.Comments;
                    else if (request.UserType == (int)UserType.Driver)
                        order.DriverComments = request.Comments;
                    if (request.Value == (int)OrderStatus.PickedUp)
                    {
                        Core.Entites.OrderRequest orderRequest = await _orderRequestRepository.GetByOrderID(request.ID);
                        orderRequest.RequestStatus = (int)OrderRequestStatus.PickedUp;
                        await _orderRequestRepository.UpdateAsync(orderRequest);
                        order.OrderPickUpTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    }
                    else if (request.Value == (int)OrderStatus.Delivered && order.IsPickBySelf != true)
                    {
                        Core.Entites.OrderRequest orderRequest = await _orderRequestRepository.GetByOrderID(request.ID);
                        orderRequest.RequestStatus = (int)OrderRequestStatus.Completed;
                        await _orderRequestRepository.UpdateAsync(orderRequest);
                        order.ActualDeliveryTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    }
                }

                // Call Charge Card API When Order Gets Completed
                SharedResponse ChargeCardResponse = new SharedResponse();
                if (order.Status == (int)OrderStatus.Completed)
                {
                    ChargeCardRequest chargeCardRequest = new ChargeCardRequest()
                    {
                        BaetotiOrderID = request.ID.ToString(),
                        BaetotiDriverID = driver == null ? "-1" : driver.UserID.ToString(),
                        BaetotiProviderID = provider.UserID.ToString()
                    };
                    ChargeCardResponse = await _paymentAPIService.ChargeCard(chargeCardRequest, _httpContextAccessor.HttpContext, _siteConfig);

                    if (ChargeCardResponse.Status == 200)
                    {
                        var tapCompanyChargeResponse = JsonConvert.DeserializeObject<TapCompanyChargeResponse.TapCompanyChargeResponse>(Convert.ToString(ChargeCardResponse.Record));
                        if (tapCompanyChargeResponse != null)
                        {
                            if (tapCompanyChargeResponse.status == "CAPTURED")
                            {
                                // reflect our local transaction to be "Paid" if the response is captured from tap.
                                transaction.Status = (int)TransactionStatus.Paid;
                                await _transactionRepository.UpdateAsync(transaction);
                            }
                        }
                    }
                }

                // Notification

                string DestinationUserID = "";
                int DestinationUserType = 0;
                int NotificationTypeID = 0;
                string SenderUser = "";
                int SenderUserType = 0;
                string SubjectID = "";

                if (request.Value == (int)OrderStatus.Approved)
                {
                    DestinationUserID = order.UserID.ToString();
                    DestinationUserType = (int)UserType.Buyer;
                    NotificationTypeID = (int)NotificationType.AcceptOrderByProvider;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Provider;
                    SubjectID = order.ID.ToString();
                }
                else if (request.Value == (int)OrderStatus.Rejected)
                {
                    DestinationUserID = order.UserID.ToString();
                    DestinationUserType = (int)UserType.Buyer;
                    NotificationTypeID = (int)NotificationType.RejectOrderByProvider;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Provider;
                    SubjectID = order.ID.ToString();
                }
                else if (request.Value == (int)OrderStatus.InProgress)
                {
                    DestinationUserID = order.UserID.ToString();
                    DestinationUserType = (int)UserType.Buyer;
                    NotificationTypeID = (int)NotificationType.SetOrderInprogress;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Provider;
                    SubjectID = order.ID.ToString();
                }
                else if (request.Value == (int)OrderStatus.Ready)
                {
                    DestinationUserID = order.UserID.ToString();
                    DestinationUserType = (int)UserType.Buyer;
                    NotificationTypeID = (int)NotificationType.SetOrderReady;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Provider;
                    SubjectID = order.ID.ToString();
                    order.OrderReadyTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                }
                else if (request.Value == (int)OrderStatus.PickedUp)
                {
                    DestinationUserID = order.UserID.ToString();
                    DestinationUserType = (int)UserType.Buyer;
                    NotificationTypeID = (int)NotificationType.DriverPickup;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Driver;
                    SubjectID = order.ID.ToString();
                    order.OrderPickUpTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                }
                else if (request.Value == (int)OrderStatus.Delivered)
                {
                    DestinationUserID = order.UserID.ToString();
                    DestinationUserType = (int)UserType.Buyer;
                    NotificationTypeID = (int)NotificationType.SetOrderDelivered;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Driver;
                    SubjectID = order.ID.ToString();
                    order.ActualDeliveryTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                }
                else if (request.Value == (int)OrderStatus.CancelledByCustomer)
                {
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.CancelledByBuyer;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Buyer;
                    SubjectID = order.ID.ToString();
                }
                else if (request.Value == (int)OrderStatus.CancelledByDriver)
                {
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.DriverCancelDeliveryRequest;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Driver;
                    SubjectID = order.ID.ToString();
                }
                else if (request.Value == (int)OrderStatus.CancelledByProvider)
                {
                    DestinationUserID = order.UserID.ToString();
                    DestinationUserType = (int)UserType.Buyer;
                    NotificationTypeID = (int)NotificationType.CancelledByProvider;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Provider;
                    SubjectID = order.ID.ToString();
                }
                else if (request.Value == (int)OrderStatus.Completed)
                {
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.CompleteOrderByBuyer;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Buyer;
                    SubjectID = order.ID.ToString();
                    order.ActualDeliveryTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
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

                await _orderRepository.UpdateAsync(order);
                await _notificationService.CreatePushNotification(createNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);

                if (order.Status == (int)OrderStatus.Completed)
                    return Ok(new SharedResponse(ChargeCardResponse.IsSuccess, ChargeCardResponse.Status, ChargeCardResponse.Message, order));
                else
                    return Ok(new SharedResponse(true, 200, "Order Status Updated Successfully", order));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        // Order Request For Driver

        [HttpPost("GetOrderRequest")]
        public async Task<IActionResult> GetOrderRequest(GetOrderRequest request)
        {
            try
            {
                var id = UserId;
                var orderRequestList = await _orderRequestRepository.GetOrderRequest(request);
                return Ok(new SharedResponse(true, 200, "", orderRequestList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        // Report Order

        [HttpPost("ReportOrder")]
        public async Task<IActionResult> ReportOrder([FromBody] ReportedOrderRequest request)
        {
            try
            {
                ReportedOrder reportedOrder = await _reportedOrderRepository.CheckIfExists(request);
                if (reportedOrder == null)
                {
                    reportedOrder = new ReportedOrder
                    {
                        OrderID = request.OrderID,
                        UserID = request.UserID,
                        ProviderID = request.ProviderID,
                        DriverID = request.DriverID,
                        Comments = request.Comments,
                        Picture = request.Picture,
                        ReportedByUserType = request.ReportedByUserType,
                        RecordDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                    };
                    await _reportedOrderRepository.AddAsync(reportedOrder);

                    SupportRequest supportRequest = new SupportRequest
                    {
                        UserID = long.Parse(UserId),
                        SupportRequestType = (int)SupportRequestType.ReportedOrder,
                        Title = request.Title,
                        Comments = request.Comments,
                        Picture = request.Picture,
                        RecordDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                    };
                    await _supportRequestRepository.AddAsync(supportRequest);

                    return Ok(new SharedResponse(true, 200, "Order Reported Successfully", reportedOrder));
                }
                return Ok(new SharedResponse(true, 200, "Order Already Reported"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("GetReportedOrder")]
        public async Task<IActionResult> GetReportedOrder(GetReportedOrderRequest request)
        {
            try
            {
                var orderList = await _reportedOrderRepository.GetAll(request);
                return Ok(new SharedResponse(true, 200, "", orderList));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetReportedOrderByID")]
        public async Task<IActionResult> GetReportedOrderByID(long ID)
        {
            try
            {
                var order = await _reportedOrderRepository.GetReportedOrderByID(ID);
                return Ok(new SharedResponse(true, 200, "", order));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
