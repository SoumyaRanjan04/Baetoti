using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.ChangeItem
{
    public class ChangeItemResponse
    {
        public ItemUpdateRequest itemUpdateRequest { get; set; }

        public List<OpenItemRequest> openItemRequest { get; set; }

        public List<CloseItemRequest> closeItemRequest { get; set; }
        public ChangeItemResponse()
        {
            itemUpdateRequest = new ItemUpdateRequest();
            openItemRequest = new List<OpenItemRequest>();
            closeItemRequest = new List<CloseItemRequest>();
        }

    }

    public class ItemUpdateRequest
    {
        public int Approved { get; set; }

        public int Pending { get; set; }

        public int Rejected { get; set; }
    }

    public class OpenItemRequest
    {
        public long ID { get; set; }

        public long ItemID { get; set; }

        public long ProviderId { get; set; }

        public string Title { get; set; }

        public string Caegory { get; set; }

        public string SubCaegory { get; set; }

        public int OrderQuantity { get; set; }

        public string AveragePreparationTime { get; set; }

        public string Price { get; set; }

        public DateTime DateAndTimeOfRequest { get; set; }

        public decimal Rating { get; set; }

    }

    public class CloseItemRequest
    {
        public long ID { get; set; }

        public long ItemID { get; set; }

        public string Name { get; set; }

        public DateTime? DateAndTimeOfRequest { get; set; }

        public DateTime? DateAndTimeOfClose { get; set; }

        public string By { get; set; }

    }

}
