using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.Role;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IRolePrivilegeRepository : IAsyncRepository<RolePrivilege>
    {
        Task<List<RolePrivilegeResponse>> GetAllRoleWithPrivileges();

        Task<RolePrivilegeByIDResponse> GetRoleWithPrivileges(long id);

        Task<List<MenuResponse>> GetAllMenuWithSubMenu();

    }
}
