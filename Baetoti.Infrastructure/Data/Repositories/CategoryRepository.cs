using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.Category;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class CategoryRepository : EFRepository<Category>, ICategoryRepository
	{

		private readonly BaetotiDbContext _dbContext;

		public CategoryRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<CategoryResponse>> GetAllByStoreID(long StoreID)
		{
			return await (from s in _dbContext.Stores
						  join i in _dbContext.Items on s.ProviderID equals i.ProviderID
						  join c in _dbContext.Categories on i.CategoryID equals c.ID
						  where s.ID == StoreID
						  select new CategoryResponse
						  {
							  ID = c.ID,
							  CategoryName = c.CategoryName,
							  CategoryArabicName = c.CategoryArabicName,
							  Color = c.Color,
							  Description = c.Description,
							  Picture = c.Picture,
							  CategoryStatus = c.CategoryStatus
						  }).Distinct().ToListAsync();
		}
	}
}
