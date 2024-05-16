using Baetoti.Shared.Response.Shared;
using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Item
{
    public class ItemResponseByID
    {
        public long ID { get; set; }
        public long ProviderID { get; set; }
        public long StoreID { get; set; }
        public string StoreName { get; set; }
        public string StoreLogo { get; set; }
        public List<ImageResponse> StoreImages { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public long CategoryID { get; set; }
        public string Category { get; set; }
        public long SubCategoryID { get; set; }
        public string SubCategory { get; set; }
        public decimal Price { get; set; }
        public decimal BaetotiPrice { get; set; }
        public long UnitID { get; set; }
        public string Unit { get; set; }
        public long Sold { get; set; }
        public long AvailableNow { get; set; }
        public long Quantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<ImageResponse> ItemImages { get; set; }
        public string AveragePreparationTime { get; set; }
        public string CategoryImage { get; set; }
        public double Distance { get; set; }
        public int VisitCount { get; set; }
        public string Description { get; set; }
        public List<TagResponse> Tags { get; set; }
        public List<ReviewResponse> Reviews { get; set; }
        public List<RecentOrder> RecentOrder { get; set; }
        public decimal AverageRating { get; set; }
        public bool isAddedToCart { get; set; }
        public bool IsCheckQuantity { get; set; }
        public ItemResponseByID()
        {
            ItemImages = new List<ImageResponse>();
            StoreImages = new List<ImageResponse>();
            Reviews = new List<ReviewResponse>();
            RecentOrder = new List<RecentOrder>();
            Tags = new List<TagResponse>();
        }
    }

    public class RecentOrder
    {
        public int OrderID { get; set; }
        public long UserID { get; set; }
        public long DriverUserID { get; set; }
        public string Driver { get; set; }
        public string Buyer { get; set; }
        public DateTime PickUp { get; set; }
        public DateTime Delivery { get; set; }
        public decimal Rating { get; set; }
        public string Review { get; set; }
    }

    public class ReviewResponse
    {
        public string UserName { get; set; }
        public string Picture { get; set; }
        public decimal Rating { get; set; }
        public string Reviews { get; set; }
        public DateTime ReviewDate { get; set; }
    }

    public class TagResponse
    {
        public long ID { get; set; }
        public string Name { get; set; }
    }
}
