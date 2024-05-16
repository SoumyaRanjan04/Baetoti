using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.ChangeItem;
using Baetoti.Shared.Response.Item;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IChangeItemRepository : IAsyncRepository<ChangeItem>
    {
        Task<ChangeItemResponseByID> GetByItemID(long ItemID);

        Task<ChangeItemResponse> GetAllChangeRequest();

    }
}
