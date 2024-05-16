using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Request.ChangeItem
{
    public class ChangeItemRequest
    {
        public long ID { get; set; }

        public long ItemID { get; set; }

        public string Name { get; set; }

        public string ArabicName { get; set; }

        public string Description { get; set; }

        public long CategoryID { get; set; }

        public long SubCategoryID { get; set; }

        public long UnitID { get; set; }

        public long ProviderID { get; set; }

        public decimal Price { get; set; }

        public string Picture { get; set; }

        public List<ChangeItemTagRequest> Tags { get; set; }

        public ChangeItemRequest()
        {
            Tags = new List<ChangeItemTagRequest>();
        }

    }

    public class ChangeItemTagRequest
    {
        public long ID { get; set; }

        public string Name { get; set; }
    }
}
