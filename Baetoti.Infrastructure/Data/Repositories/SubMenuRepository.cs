using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.SubMenu;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class SubMenuRepository : EFRepository<SubMenu>, ISubMenuRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public SubMenuRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        Task<List<SubMenuResponse>> ISubMenuRepository.GetAll()
        {
            return (from m in _dbContext.Menus 
                    join sm in _dbContext.SubMenus
                    on m.ID equals sm.MenuID
                    select new SubMenuResponse
                    {
                        ID = sm.ID,
                        MenuID = sm.MenuID,
                        MenuName = m.Name,
                        SubMenuName = sm.Name,
                    }).ToListAsync();
        }
    }
}
