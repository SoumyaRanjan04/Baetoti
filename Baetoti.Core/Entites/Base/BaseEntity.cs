using System.ComponentModel.DataAnnotations;

namespace Baetoti.Core.Entites.Base
{
    public abstract class BaseEntity
    {
        [Key]
        public long ID { get; set; }
    }
}
