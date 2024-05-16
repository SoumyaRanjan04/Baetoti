using System;

namespace Baetoti.Shared.Request.Promotion
{
	public class GetPromotionRequest
	{
        public int PromotionFilter { get; set; }

        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }

    }
}
