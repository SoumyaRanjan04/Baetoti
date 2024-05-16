using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Request.Payment
{
    public class GetAllAuthPaymentsRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
