using Baetoti.Core.Entites.Base;
using Baetoti.Shared.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Baetoti.Core.Entites
{
    [Table("UserLoginHistory", Schema = "baetoti")]
    public partial class UserLoginHistory : BaseEntity
    {
        public long UserID { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public DateTime LoginTime { get; set; }

        public DateTime LogoutTime { get; set; }

        public string IPAddress { get; set; }

    }
}
