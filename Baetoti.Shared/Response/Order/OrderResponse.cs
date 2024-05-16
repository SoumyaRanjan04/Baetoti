using Baetoti.Shared.Request.Order;
using Baetoti.Shared.Response.Driver;
using Baetoti.Shared.Response.Provider;
using Baetoti.Shared.Response.Store;
using Baetoti.Shared.Response.User;
using System;
using System.Collections.Generic;
using DriverResponse = Baetoti.Shared.Response.Driver.DriverResponse;

namespace Baetoti.Shared.Response.Order
{
    public class OrderResponse
    {
        public int Completed { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int InProgress { get; set; }
        public int Ready { get; set; }
        public int PickedUp { get; set; }
        public int Delivered { get; set; }
        public int Complaints { get; set; }
        public int CancelledByDriver { get; set; }
        public int CancelledByProvider { get; set; }
        public int CancelledByCustomer { get; set; }
        public int CancelledBySystem { get; set; }
        public decimal AverageRating { get; set; }
        public ProviderOrderStates ProviderOrder { get; set; }
        public DriverOrderStates DriverOrder { get; set; }
        public List<OrderGraph> orderGraph { get; set; }
    }

    public class ProviderOrderStates
    {
        public int Approved { get; set; }
        public int Rejected { get; set; }
        public int Canceled { get; set; }
    }

    public class DriverOrderStates
    {
        public int Pending { get; set; }
        public int Delivered { get; set; }
        public int PickedUp { get; set; }
    }

    public class OrderStates
    {
        public long OrderID { get; set; }
        public long UserID { get; set; }
        public long ProviderUserID { get; set; }
        public long DriverUserID { get; set; }
        public long ProviderID { get; set; }
        public long DriverID { get; set; }
        public string Buyer { get; set; }
        public string Provider { get; set; }
        public string Driver { get; set; }
        public decimal OrderAmount { get; set; }
        public string PaymentType { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? ExpectedDeliveryTime { get; set; }
        public string DeliverOrPickup { get; set; }
        public string OrderStatus { get; set; }
    }

    public class OrderGraph
    {
        public string Name { get; set; }
        public int Orders { get; set; }
    }

    public class UserOrderStates
    {
        public long OrderID { get; set; }
        public long UserID { get; set; }
        public long DriverID { get; set; }
        public long ProviderID { get; set; }
        public string Buyer { get; set; }
        public string Provider { get; set; }
        public string Driver { get; set; }
        public int OrderAmount { get; set; }
        public string PaymentType { get; set; }
        public DateTime? Date { get; set; }
        public DateTime? ExpectedDeliveryTime { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string DeliverOrPickup { get; set; }
        public string OrderStatus { get; set; }
        public int OrderStatusKey { get; set; }
        public StoreResponse store { get; set; }
        public decimal DeliveryCharges { get; set; }
        public decimal TotalDriverCharges { get; set; }
        public decimal ServiceFees { get; set; }
        public decimal OrderPrice { get; set; }
        public int TotalItemsCount { get; set; }
        public bool IsPickBySelf { get; set; }
        public double OrderAddressLongitude { get; set; }
        public double OrderAddressLatitude { get; set; }
        public string OrderAddress { get; set; }
        public double Distance { get; set; }
        public string ProviderComments { get; set; }
        public string DriverComments { get; set; }
        public string NotesForDriver { get; set; }
        public List<OrderItemsResponse> orderItems { get; set; }
        public BuyerDetail buyerDetail { get; set; }
        public UserOrderStates()
        {
            orderItems = new List<OrderItemsResponse>();
            buyerDetail = new BuyerDetail();
            store = new StoreResponse();
        }
    }

    public class BuyerDetail
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Picture { get; set; }
    }

}
