using Baetoti.Shared.Response.Cart;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.Store;
using Baetoti.Shared.Response.User;
using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Order
{
    public class OrderByUserIDResponse
    {
        public long OrderID { get; set; }
        public string PaymentType { get; set; }
        public string DeliverOrPickup { get; set; }
        public string NotesForDriver { get; set; }
        public string ProviderComments { get; set; }
        public string DriverComments { get; set; }
        public string DeliveryAddress { get; set; }
        public double AddressLongitude { get; set; }
        public double AddressLatitude { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime ExpectedDeliveryTime { get; set; }
        public DateTime ActualDeliveryTime { get; set; }
        public DateTime OrderReadyTime { get; set; }
        public DateTime OrderPickupTime { get; set; }
        public bool IsPickBySelf { get; set; }
        public string OrderStatus { get; set; }
        public int OrderRequestStatus { get; set; }
        public decimal TotalCharges { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal TotalDriverCharges { get; set; }
        public decimal ServiceFees { get; set; }
        public bool IsDriverRatedByProvider { get; set; }
        public bool IsDriverRatedByBuyer { get; set; }
        public bool IsProviderRatedByBuyer { get; set; }
        public bool IsProviderRatedByDriver { get; set; }
        public bool IsBuyerRatedByProvider { get; set; }
        public bool IsBuyerRatedByDriver { get; set; }
        public bool IsFavouriteDriver { get; set; }
        public double Distance { get; set; }
        public BuyerResponse buyer { get; set; }
        public ProviderResponse provider { get; set; }
        public DriverResponse driver { get; set; }
        public StoreResponse store { get; set; }
        public List<OrderItemsResponse> orderItems { get; set; }
        public OrderByUserIDResponse()
        {
            buyer = new BuyerResponse();
            driver = new DriverResponse();
            store = new StoreResponse();
            orderItems = new List<OrderItemsResponse>();
        }
    }

    public class OrderItemsResponse
    {
        public long ItemID { get; set; }
        public string ItemName { get; set; }
        public string ItemComments { get; set; }
        public List<ImageResponse> ItemImages { get; set; }
        public decimal Price { get; set; }
        public decimal BaetotiPrice { get; set; }
        public int Quantity { get; set; }
        public long CategoryID { get; set; }
        public string Category { get; set; }
        public string CategoryArabic { get; set; }
        public OrderItemsResponse()
        {
            ItemImages = new List<ImageResponse>();
        }
    }

}
