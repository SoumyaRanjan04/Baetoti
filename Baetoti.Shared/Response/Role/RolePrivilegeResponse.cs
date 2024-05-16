using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Role
{
    public class RolePrivilegeResponse
    {
        public long ID { get; set; }

        public string RoleName { get; set; }

        public string MenuAuthorization { get; set; }

        public DateTime? CreatedDate { get; set; }

    }

    public class RolePrivilegeByIDResponse
    {
        public long ID { get; set; }

        public string RoleName { get; set; }
        
        public List<string> Menu { get; set; }
        
    }

    public class RoleMenuNameResponse
    {
        public string Menu { get; set; }
    }

    public class MenuResponse
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public bool IsSelected { get; set; }

        public List<PrivilegeResponse> SelectedPrivileges { get; set; }

        public List<SubMenuResponse> SelectedSubMenu { get; set; }

        public MenuResponse()
        {
            SelectedPrivileges = new List<PrivilegeResponse>();
            SelectedSubMenu = new List<SubMenuResponse>();
        }

    }

    public class SubMenuResponse
    {
        public long ID { get; set; }

        public string Name { get; set; }

        public bool IsSelected { get; set; }

        public List<PrivilegeResponse> SelectedPrivileges { get; set; }

        public SubMenuResponse()
        {
            SelectedPrivileges = new List<PrivilegeResponse>();
        }

    }

    public class PrivilegeResponse
    {
        public long ID { get; set; }

        public string Name { get; set; }
        public string MenuName { get; set; }

        public bool IsSelected { get; set; }

    }
}
