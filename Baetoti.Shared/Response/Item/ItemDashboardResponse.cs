using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.Item
{
	public class ItemDashboardResponse
	{
		public AvailableItem Available { get; set; }
		public TopRatedItem TopRated { get; set; }
		public LatestItem Latest { get; set; }
		public FeaturedItem Featured { get; set; }
		public NearMeItem NearMe { get; set; }
		public ItemDashboardResponse()
		{
			Available = new AvailableItem();
			TopRated = new TopRatedItem();
			Latest = new LatestItem();
			Featured = new FeaturedItem();
			NearMe = new NearMeItem();
		}
	}

	public class AvailableItem
	{
		public List<ItemListResponse> Items { get; set; }
		public AvailableItem()
		{
			Items = new List<ItemListResponse>();
		}
	}

	public class TopRatedItem
	{
		public List<ItemListResponse> Items { get; set; }
		public TopRatedItem()
		{
			Items = new List<ItemListResponse>();
		}
	}

	public class FeaturedItem
	{
		public List<ItemListResponse> Items { get; set; }
		public FeaturedItem()
		{
			Items = new List<ItemListResponse>();
		}
	}

	public class NearMeItem
	{
		public List<ItemListResponse> Items { get; set; }
		public NearMeItem()
		{
			Items = new List<ItemListResponse>();
		}
	}

	public class LatestItem
	{
		public List<ItemListResponse> Items { get; set; }
		public LatestItem()
		{
			Items = new List<ItemListResponse>();
		}
	}

}
