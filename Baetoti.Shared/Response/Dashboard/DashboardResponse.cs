using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.Dashboard
{
    public class DashboardResponse
    {
        public Provider Providers { get; set; }

        public Driver Drivers { get; set; }

        public Item Items { get; set; }

        public List<RevenueVsCommision> RevenueVsCommisions { get; set; }

        public List<Location> Locations { get; set; }

        public TotalVsOnline TotalVsOnlines { get; set; }

        public List<NewUser> NewUsers { get; set; }

        public List<Sale> Sales { get; set; }

        public List<PopularItem> PopularItems { get; set; }

        public List<PopularProvider> PopularProviders { get; set; }

        public OrderSummary OrderSummary { get; set; }

        public List<LatestOrder> LatestOrders { get; set; }

        public DashboardResponse()
        {
            Providers = new Provider();
            Drivers = new Driver();
            Items = new Item();
            RevenueVsCommisions = new List<RevenueVsCommision>();
            Locations = new List<Location>();
            TotalVsOnlines = new TotalVsOnline();
            NewUsers = new List<NewUser>();
            Sales = new List<Sale>();
            PopularItems = new List<PopularItem>();
            PopularProviders = new List<PopularProvider>();
            OrderSummary = new OrderSummary();
            LatestOrders = new List<LatestOrder>();
        }

    }

    public class Provider
    {
        public int TotalCount { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }

    public class Driver
    {
        public int TotalCount { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }

    public class Item
    {
        public int TotalCount { get; set; }
        public int Pending { get; set; }
        public int Approved { get; set; }
        public int Rejected { get; set; }
    }

    public class RevenueVsCommision
    {
        public string date { get; set; }

        public decimal Commision { get; set; }

        public int Revenue { get; set; }

    }

    public class Location
    {
        public double Lat { get; set; }

        public double Lng { get; set; }

    }

    public class TotalVsOnline
    {


        public TotalVsOnline()
        {
            Users = new List<UserCount>();

            TotalProviders = new List<UserCount>();

            TotalDrivers = new List<UserCount>();
        }

        public List<UserCount> Users { get; set; }

        public List<UserCount> TotalProviders { get; set; }

        public List<UserCount> TotalDrivers { get; set; }

        public int TotalUser { get; set; }

        public int TotalProvider { get; set; }

        public int TotalDriver { get; set; }

    }

    public class UserCount
    {
        public string Date { get; set; }

        public int TotalUser { get; set; }

        public int OnlineUser { get; set; }

    }

    public class NewUser
    {
        public string Name { get; set; }

        public string Role { get; set; }

        public string Picture { get; set; }

    }

    public class Sale
    {
        public string Date { get; set; }

        public decimal Sales { get; set; }

        public decimal Revenue { get; set; }
    }

    public class PopularItem
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public decimal Count { get; set; }

        public decimal Percentage { get; set; }
    }

    public class PopularProvider
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }

        public int Count { get; set; }

        public decimal Percentage { get; set; }
    }

    public class OrderSummary
    {
        public int OrderCompleted { get; set; }

        public int Pending { get; set; }

        public int Approved { get; set; }

        public int OnTheWay { get; set; }

        public int Cancelled { get; set; }
    }

    public class LatestOrder
    {
        public long OrderId { get; set; }

        public long ProviderId { get; set; }

        public long UserID { get; set; }

        public long ProviderUserID { get; set; }

        public long DriverUserID { get; set; }

        public string Provider { get; set; }

        public string ProviderAddress { get; set; }

        public string Driver { get; set; }

        public string Date { get; set; }

        public string Status { get; set; }

        public string Buyer { get; set; }

        public string BuyerAddress { get; set; }

    }

}
