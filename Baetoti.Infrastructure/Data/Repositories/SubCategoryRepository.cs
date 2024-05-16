using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.SubCategory;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Baetoti.Shared.Response.Category;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class SubCategoryRepository : EFRepository<SubCategory>, ISubCategoryRepository
	{

		private readonly BaetotiDbContext _dbContext;

		public SubCategoryRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		Task<List<SubCategoryResponse>> ISubCategoryRepository.GetByCategoryAsync(long id)
		{
			return (from c in _dbContext.Categories
					join sc in _dbContext.SubCategories
					on c.ID equals sc.CategoryId
					where (id == 0 || sc.CategoryId == id)
					&& c.MarkAsDeleted == false
					&& sc.MarkAsDeleted == false
					select new SubCategoryResponse
					{
						ID = sc.ID,
						CategoryID = sc.CategoryId,
						CategoryName = sc.SubCategoryName,
						CategoryArabicName = sc.SubCategoryArabicName,
						SubCategoryName = sc.SubCategoryName,
						SubCategoryArabicName = sc.SubCategoryArabicName,
						SubCategoryStatus = sc.SubCategoryStatus,
						Picture = sc.Picture
					}).ToListAsync();
		}

		public async Task<List<SubCategoryResponse>> GetAllByStoreID(long StoreID)
		{
			return await (from s in _dbContext.Stores
						  join i in _dbContext.Items on s.ProviderID equals i.ProviderID
						  join c in _dbContext.Categories on i.CategoryID equals c.ID
						  join sc in _dbContext.SubCategories on i.SubCategoryID equals sc.ID
						  where s.ID == StoreID && sc.MarkAsDeleted == false
						  select new SubCategoryResponse
						  {
							  ID = sc.ID,
							  CategoryID = sc.CategoryId,
							  CategoryName = c.CategoryName,
							  CategoryArabicName = c.CategoryArabicName,
							  SubCategoryName = sc.SubCategoryName,
							  SubCategoryArabicName = sc.SubCategoryArabicName,
							  SubCategoryStatus = sc.SubCategoryStatus,
							  Picture = sc.Picture
						  }).Distinct().ToListAsync();
		}

	}
}
