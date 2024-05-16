using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.SubMenu;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface ISubMenuRepository : IAsyncRepository<SubMenu>
    {
        Task<List<SubMenuResponse>> GetAll();
    }
}
