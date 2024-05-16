using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class ChangeItemImageRepository : EFRepository<ChangeItemImage>, IChangeItemImageRepository
	{

		private readonly BaetotiDbContext _dbContext;

		public ChangeItemImageRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

	}
}
