using Baetoti.Core.Entites.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Baetoti.Core.Entites
{
    [Table("InstragramToken", Schema = "baetoti")]
    public partial class InstragramToken : BaseEntity
    {
        public long StoreID { get; set; }

        public string Token { get; set; }

        public DateTime LastTimeTokenUpdated { get; set; }

    }
}
