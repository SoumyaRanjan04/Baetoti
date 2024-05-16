using System;

namespace Baetoti.Shared.Response.Role
{
    public class RoleResponse
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }

    }
}
