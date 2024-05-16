using Baetoti.Core.Entites.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("EmployeeLoginHistory", Schema = "baetoti")]
    public partial class EmployeeLoginHistory : BaseEntity
    {
        public long EmployeeID { get; set; }

        public string Employee { get; set; }

        public string Date { get; set; }

        public string LoginTime { get; set; }

        public string LogoutTime { get; set; }

        public string IPAddress { get; set; }

    }
}
