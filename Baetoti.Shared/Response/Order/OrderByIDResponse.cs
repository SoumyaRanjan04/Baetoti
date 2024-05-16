using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Order
{
    public class OrderByIDResponse
    {
        public OrderDetail orderDetail { get; set; }
        public CustomerDetail customerDetail { get; set; }
        public DriverDetail? driverDetail { get; set; }
        public ProviderDetail providerDetail { get; set; }
        public PaymentInfo paymentInfo { get; set; }
        public OrderStatusResponse orderStatus { get; set; }
        public List<ItemList> itemsList { get; set; }
        public List<Reviews> reviews { get; set; }
        public CostSummary costSummary { get; set; }
        public List<MapData> MapData { get; set; }
        public TwoLineGraph twoLineGraph { get; set; }
        public OrderByIDResponse()
        {
            orderDetail = new OrderDetail();
            customerDetail = new CustomerDetail();
            driverDetail = new DriverDetail();
            providerDetail = new ProviderDetail();
            paymentInfo = new PaymentInfo();
            orderStatus = new OrderStatusResponse();
            itemsList = new List<ItemList>();
            reviews = new List<Reviews>();
            costSummary = new CostSummary();
            twoLineGraph = new TwoLineGraph();
        }

        public string invoiceID { get; set; }
    }

    public class TwoLineGraph
    {
        public string[] Status { get; set; }
        public string[] Date { get; set; }
    }

    public class MapData
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class OrderDetail
    {
        public string CreateDate { get; set; }
        public string CreatedTime { get; set; }
        public string ActualDate { get; set; }
        public string ActualTime { get; set; }
        public string ScheduleDate { get; set; }
        public string ScheduleTime { get; set; }
        public string OrderReadyDate { get; set; }
        public string OrderReadyTime { get; set; }
        public string OrderPickUpDate { get; set; }
        public string OrderPickUpTime { get; set; }
        public string OrderReceivedDate { get; set; }
        public string OrderReceivedTime { get; set; }
        public bool IsDriverRatedByProvider { get; set; }
        public bool IsDriverRatedByBuyer { get; set; }
        public bool IsProviderRatedByBuyer { get; set; }
        public bool IsProviderRatedByDriver { get; set; }
        public bool IsBuyerRatedByProvider { get; set; }
        public bool IsBuyerRatedByDriver { get; set; }
    }

    public class CustomerDetail
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }

    public class DriverDetail
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Rating { get; set; }
        public long ItemCount { get; set; }
    }

    public class ProviderDetail
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Rating { get; set; }
        public long ItemCount { get; set; }
        public string StoreName { get; set; }
    }

    public class PaymentInfo
    {
        public string PaymnetMethod { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class OrderStatusResponse
    {
        public string OrderStatus { get; set; }
        public string DeliverPickUp { get; set; }
    }

    public class ItemList
    {
        public int SrNo { get; set; }
        public string ShopName { get; set; }
        public long ItemID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Rating { get; set; }
        public string Reviews { get; set; }
        public string Image { get; set; }
    }

    public class Reviews
    {
        public string Name { get; set; }
        public string Picture { get; set; }
        public decimal Rating { get; set; }
        public string Comments { get; set; }
        public DateTime Date { get; set; }
    }

    public class CostSummary
    {
        public string SubTotal { get; set; }
        public string VAT { get; set; }
        public string DeliveryCharges { get; set; }
        public string ServiceCharges { get; set; }
        public string TotalAmount { get; set; }
        public string DriverCommision { get; set; }
        public string ProviderCommision { get; set; }
        public string Amount { get; set; }
        public string Discount { get; set; }
    }
}
