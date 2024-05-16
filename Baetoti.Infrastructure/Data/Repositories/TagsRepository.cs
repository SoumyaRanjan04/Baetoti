using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.TagResponse;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Baetoti.Shared.Enum;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class TagsRepository : EFRepository<Tags>, ITagsRepository
	{

		private readonly BaetotiDbContext _dbContext;

		public TagsRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<List<TagResponse>> GetAllStoreTags()
		{
			return await (from t in _dbContext.Tags.Where(t => t.MarkAsDeleted == false)
						  join c in _dbContext.Categories on t.CategoryID equals c.ID
						  join sc in _dbContext.SubCategories on t.SubCategoryID equals sc.ID
						  where t.TagType == (int)TagType.Store
						  select new TagResponse
						  {
							  ID = t.ID,
							  TagType = t.TagType,
							  TagTypeName = ((TagType)t.TagType).ToString(),
							  CategoryID = t.CategoryID,
							  CategoryEnglish = c.CategoryName,
							  CategoryArabic = c.CategoryArabicName,
							  SubCategoryID = t.SubCategoryID,
							  SubCategoryEnglish = sc.SubCategoryName,
							  SubCategoryArabic = sc.SubCategoryArabicName,
							  TagEnglish = t.TagEnglish,
							  TagArabic = t.TagArabic,
							  CreatedBy = t.CreatedBy,
							  UpdatedBy = t.UpdatedBy,
							  CreatedAt = t.CreatedAt,
							  LastUpdatedAt = t.LastUpdatedAt,
							  TagStatus = t.TagStatus
						  }).ToListAsync();
		}

		public async Task<List<TagResponse>> GetAllTagsAsync()
		 {
			return await (from t in _dbContext.Tags.Where(t=>t.MarkAsDeleted ==false)
						  select new TagResponse
						  {
							  ID = t.ID,
							  TagType = t.TagType,
							  TagTypeName = ((TagType)t.TagType).ToString(),
							  CategoryID = t.CategoryID,
							  CategoryEnglish = t.CategoryID != 0 ?_dbContext.Categories.Where(ww => ww.ID == t.CategoryID).Select(s => s.CategoryName).FirstOrDefault() : "",
							  CategoryArabic = t.CategoryID != 0 ? _dbContext.Categories.Where(ww => ww.ID == t.CategoryID).Select(s => s.CategoryArabicName).FirstOrDefault() : "",
							  SubCategoryID = t.SubCategoryID,
							  SubCategoryEnglish = t.SubCategoryID != 0 ? _dbContext.SubCategories.Where(ww => ww.ID == t.SubCategoryID).Select(s => s.SubCategoryName).FirstOrDefault() : "",
							  SubCategoryArabic = t.SubCategoryID != 0 ? _dbContext.SubCategories.Where(ww => ww.ID == t.SubCategoryID).Select(s => s.SubCategoryArabicName).FirstOrDefault() : "",
							  TagEnglish = t.TagEnglish,
							  TagArabic = t.TagArabic,
							  CreatedBy = t.CreatedBy,
							  UpdatedBy = t.UpdatedBy,
							  CreatedByName = t.CreatedBy != null ? _dbContext.Employee.Where(ww=>ww.ID == t.CreatedBy).Select(ss=>ss.FirstName +" "+ ss.LastName).FirstOrDefault() :"",
							  UpdatedByName = t.UpdatedBy != null ? _dbContext.Employee.Where(ww => ww.ID == t.UpdatedBy).Select(ss => ss.FirstName + " " + ss.LastName).FirstOrDefault() : "",
							  CreatedAt = t.CreatedAt,
							  LastUpdatedAt = t.LastUpdatedAt,
							  TagStatus = t.TagStatus
						  }).ToListAsync();
		}

		public async Task<TagResponse> GetTagByIDAsync(int ID)
		{
			return await (from t in _dbContext.Tags.Where(t => t.MarkAsDeleted == false && t.ID == ID)
						  
						  select new TagResponse
						  {
							  ID = t.ID,
							  TagType = t.TagType,
							  TagTypeName = ((TagType)t.TagType).ToString(),
							  CategoryID = t.CategoryID,
							  CategoryEnglish = t.CategoryID != 0 ? _dbContext.Categories.Where(ww => ww.ID == t.CategoryID).Select(s => s.CategoryName).FirstOrDefault() : "",
							  CategoryArabic = t.CategoryID != 0 ? _dbContext.Categories.Where(ww => ww.ID == t.CategoryID).Select(s => s.CategoryArabicName).FirstOrDefault() : "",
							  SubCategoryID = t.SubCategoryID,
							  SubCategoryEnglish = t.SubCategoryID != 0 ? _dbContext.SubCategories.Where(ww => ww.ID == t.SubCategoryID).Select(s => s.SubCategoryName).FirstOrDefault() : "",
							  SubCategoryArabic = t.SubCategoryID != 0 ? _dbContext.SubCategories.Where(ww => ww.ID == t.SubCategoryID).Select(s => s.SubCategoryArabicName).FirstOrDefault() : "",
							  TagEnglish = t.TagEnglish,
							  TagArabic = t.TagArabic,
							  CreatedBy = t.CreatedBy,
							  UpdatedBy = t.UpdatedBy,
							  CreatedByName = t.CreatedBy != null ? _dbContext.Employee.Where(ww => ww.ID == t.CreatedBy).Select(ss => ss.FirstName + " " + ss.LastName).FirstOrDefault() : "",
							  UpdatedByName = t.UpdatedBy != null ? _dbContext.Employee.Where(ww => ww.ID == t.UpdatedBy).Select(ss => ss.FirstName + " " + ss.LastName).FirstOrDefault() : "",
							  CreatedAt = t.CreatedAt,
							  LastUpdatedAt = t.LastUpdatedAt,
							  TagStatus = t.TagStatus,
						  }).FirstOrDefaultAsync();
		}

		public async Task<List<TagResponse>> GetTagBySubCategoryIDAsync(int SubCategoryID)
		{
			return await (from t in _dbContext.Tags.Where(t => t.MarkAsDeleted == false)
						  join c in _dbContext.Categories on t.CategoryID equals c.ID
						  join sc in _dbContext.SubCategories on t.SubCategoryID equals sc.ID
						  where sc.ID == SubCategoryID
						  select new TagResponse
						  {
							  ID = t.ID,
							  TagType = t.TagType,
							  TagTypeName = ((TagType)t.TagType).ToString(),
							  CategoryID = t.CategoryID,
							  CategoryEnglish = c.CategoryName,
							  CategoryArabic = c.CategoryArabicName,
							  SubCategoryID = t.SubCategoryID,
							  SubCategoryEnglish = sc.SubCategoryName,
							  SubCategoryArabic = sc.SubCategoryArabicName,
							  TagEnglish = t.TagEnglish,
							  TagArabic = t.TagArabic,
							  CreatedBy = t.CreatedBy,
							  UpdatedBy = t.UpdatedBy,
							  CreatedAt = t.CreatedAt,
							  LastUpdatedAt = t.LastUpdatedAt,
							  TagStatus = t.TagStatus
						  }).ToListAsync();
		}

	}
}
