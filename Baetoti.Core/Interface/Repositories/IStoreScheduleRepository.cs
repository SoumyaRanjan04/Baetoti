using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.StoreSchedule;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IStoreScheduleRepository : IAsyncRepository<StoreSchedule>
    {
        Task<List<StoreScheduleResponse>> GetByStoreID(long StoreID);

    }
}
