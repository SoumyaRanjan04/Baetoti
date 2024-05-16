using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.StoreSchedule;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class StoreScheduleRepository : EFRepository<StoreSchedule>, IStoreScheduleRepository
	{

		private readonly BaetotiDbContext _dbContext;

		public StoreScheduleRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<StoreScheduleResponse>> GetByStoreID(long StoreID)
		{
			return await (from s in _dbContext.StoreSchedules
						  where s.StoreID == StoreID && s.MarkAsDeleted == false
						  select new StoreScheduleResponse
						  {
							  ID = s.ID,
							  StoreID = s.StoreID,
							  Day = s.Day,
							  StartTime = s.StartTime,
							  EndTime = s.EndTime,
						  }).ToListAsync();
		}
	}
}
