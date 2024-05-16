using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Item
{
    public class ItemsOnBoardingResponse
    {
        public ItemStatistic itemStatistic { get; set; }

        public List<OpenItemOnBoardingRequest> openItemOnBoardingRequest { get; set; }

        public List<CloseItemOnBoardingRequest> closeItemOnBoardingRequest { get; set; }

        public GraphData graphData { get; set; }

        public ItemsOnBoardingResponse()
        {
            openItemOnBoardingRequest = new List<OpenItemOnBoardingRequest>();
            closeItemOnBoardingRequest = new List<CloseItemOnBoardingRequest>();
            graphData = new GraphData();
        }
    }

    public class GraphData
    {
        public int Current { get; set; }

        public double Avg { get; set; }

        public int Min { get; set; }

        public int Max { get; set; }

        public List<Data> Data { get; set; }

        public GraphData()
        {
            Data = new List<Data>();
        }

    }

    public class Data
    {
        public string Date { get; set; }

        public int Item { get; set; }

    }

    public class ItemStatistic
    {
        public int Approved { get; set; }

        public int Pending { get; set; }

        public int Rejected { get; set; }
    }

    public class OpenItemOnBoardingRequest
    {
        public long ID { get; set; }

        public long ProviderId { get; set; }

        public string Caegory { get; set; }

        public string SubCaegory { get; set; }

        public decimal Price { get; set; }

        public DateTime DateAndTimeOfRequest { get; set; }

    }

    public class CloseItemOnBoardingRequest
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public DateTime DateAndTimeOfRequest { get; set; }

        public DateTime? DateAndTimeOfClose { get; set; }

        public string By { get; set; }

        public string Status { get; set; }

    }

}
