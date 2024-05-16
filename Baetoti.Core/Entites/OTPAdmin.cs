using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("OTPAdmin", Schema = "baetoti")]
    public partial class OTPAdmin : BaseEntity
    {
        public long EmployeeID { get; set; }
        public string OneTimePassword { get; set; }
        public DateTime OTPGeneratedAt { get; set; }
        public int OTPStatus { get; set; }
        public string Description { get; set; }
        public int RetryCount { get; set; }
    }
}
