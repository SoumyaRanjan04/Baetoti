using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.Role;
using System.Linq;
using MoreLinq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class RolePrivilegeRepository : EFRepository<RolePrivilege>, IRolePrivilegeRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public RolePrivilegeRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<MenuResponse>> GetAllMenuWithSubMenu()
        {
            return await (from m in _dbContext.Menus
                          select new MenuResponse
                          {
                              ID = m.ID,
                              Name = m.Name,
                              SelectedPrivileges = _dbContext.Privileges.Select(x => new PrivilegeResponse
                              {
                                  ID = x.ID,
                                  Name = x.Name
                              }).ToList(),
                              SelectedSubMenu = _dbContext.SubMenus.Where(x => x.MenuID == m.ID).Select(x => new SubMenuResponse
                              {
                                  ID = x.ID,
                                  Name = x.Name,
                                  SelectedPrivileges = _dbContext.Privileges.Select(x => new PrivilegeResponse
                                  {
                                      ID = x.ID,
                                      Name = x.Name
                                  }).ToList()
                              }).ToList()
                          }).ToListAsync();
        }

        public async Task<RolePrivilegeByIDResponse> GetRoleWithPrivileges(long id)
        {
            var roleAndPrivileges = await (from rp in _dbContext.RolePrivileges
                                           join r in _dbContext.Roles on rp.RoleID equals r.ID
                                           where r.ID == id
                                           select new
                                           {
                                               ID = r.ID,
                                               RoleName = r.Name,
                                           }).FirstOrDefaultAsync();

            var menu = _dbContext.RolePrivileges.Where(x => x.RoleID == id).Select(ss => ss.Menu).ToList();
            var roleResponse = new RolePrivilegeByIDResponse();
            if (menu.Any() && menu.Count > 0)
            {
                roleResponse.ID = roleAndPrivileges.ID;
                roleResponse.RoleName = roleAndPrivileges.RoleName;
                roleResponse.Menu = menu;
                return roleResponse;
            }
            else
            {
                return new RolePrivilegeByIDResponse();
            }
        }

        Task<List<RolePrivilegeResponse>> IRolePrivilegeRepository.GetAllRoleWithPrivileges()
        {
            return (from rp in _dbContext.RolePrivileges
                    join r in _dbContext.Roles on rp.RoleID equals r.ID
                    select new RolePrivilegeResponse
                    {
                        ID = rp.ID,
                        RoleName = r.Name,
                        CreatedDate = r.CreatedAt,
                        MenuAuthorization = "Access to all menu"
                    }).ToListAsync();
        }

    }
}
