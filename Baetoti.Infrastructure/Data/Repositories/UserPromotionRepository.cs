﻿using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class UserPromotionRepository : EFRepository<UserPromotion>, IUserPromotionRepository
	{
		private readonly BaetotiDbContext _dbContext;

		public UserPromotionRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

	}
}
