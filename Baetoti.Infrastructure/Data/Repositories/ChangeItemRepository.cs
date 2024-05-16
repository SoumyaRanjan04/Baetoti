using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.ChangeItem;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Baetoti.Shared.Request.Shared;
using System.Xml.Linq;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class ChangeItemRepository : EFRepository<ChangeItem>, IChangeItemRepository
	{

		private readonly BaetotiDbContext _dbContext;

		public ChangeItemRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<ChangeItemResponse> GetAllChangeRequest()
		{
			ChangeItemResponse changeItemResponse = new ChangeItemResponse();

			changeItemResponse.itemUpdateRequest = new ItemUpdateRequest
			{
				Pending = (from req in _dbContext.ChangeItem where req.ItemStatus == 2 select req).Count(),
				Approved = (from req in _dbContext.ChangeItem where req.ItemStatus == 3 select req).Count(),
				Rejected = (from req in _dbContext.ChangeItem where req.ItemStatus == 4 select req).Count()
			};

			changeItemResponse.closeItemRequest = await (from i in _dbContext.ChangeItem
														 where i.ItemStatus == 3 || i.ItemStatus == 4
														 join tu in _dbContext.Employee on i.RequestClosedBy equals tu.ID
														 into temp
														 from u in temp.DefaultIfEmpty()
														 select new CloseItemRequest
														 {
															 ID = i.ID,
															 ItemID = i.ItemId,
															 Name = i.Name,
                                                             DateAndTimeOfRequest = i.RequestDate,
															 DateAndTimeOfClose = i.RequestCloseDate,
															 By = u == null ? "" : $"{u.FirstName} {u.LastName}"
														 }).ToListAsync();

			changeItemResponse.openItemRequest = await (from i in _dbContext.ChangeItem
														where i.ItemStatus == 2
														join c in _dbContext.Categories on i.CategoryID equals c.ID
														join sc in _dbContext.SubCategories on i.SubCategoryID equals sc.ID
														join orderitem in _dbContext.OrderItems on i.ID equals orderitem.ID
														into tempOrder
														from oi in tempOrder.DefaultIfEmpty()
														select new OpenItemRequest
														{
															ID = i.ID,
															ItemID = i.ItemId,
															ProviderId = i.ProviderID,
															Title = i.Name,
															Caegory = c.CategoryName,
															SubCaegory = sc.SubCategoryName,
															OrderQuantity = oi == null ? 0 : oi.Quantity,
															DateAndTimeOfRequest = i.RequestDate,
															AveragePreparationTime = i.AveragePreparationTime,
															Price = $"{i.Price} SAR/{_dbContext.Units.Where(x => x.ID == i.UnitID).FirstOrDefault().UnitEnglishName}",
															Rating = i.Rating
														}).ToListAsync();

			return changeItemResponse;
		}

		public async Task<ChangeItemResponseByID> GetByItemID(long ItemID)
		{
			var changeitem = await (from ci in _dbContext.ChangeItem
									join c in _dbContext.Categories on ci.CategoryID equals c.ID
									join sc in _dbContext.SubCategories on ci.SubCategoryID equals sc.ID
									join p in _dbContext.Providers on ci.ProviderID equals p.ID
									join un in _dbContext.Units on ci.UnitID equals un.ID
									join u in _dbContext.Users on p.UserID equals u.ID
									join s in _dbContext.Stores on p.ID equals s.ProviderID
									where ci.ItemId == ItemID
									select new ChangeItemResponseByID
									{
										ID = ci.ID,
										ItemId = ci.ItemId,
										StoreName = s.Name,
										Location = s.Location,
										Title = ci.Name,
										Description = ci.Description,
										CategoryID = ci.CategoryID,
										Category = c.CategoryName,
										SubCategoryID = ci.SubCategoryID,
										SubCategory = sc.SubCategoryName,
										Quantity = 0,
										Picture1 = ci.Picture1,
										Picture2 = ci.Picture2,
										Picture3 = ci.Picture3,
										TotalRevenue = 0,
										AveragePreparationTime = "0",
										Price = $"{ci.Price} SAR/{_dbContext.Units.Where(x => x.ID == ci.UnitID).FirstOrDefault().UnitEnglishName}",
										AverageRating = 0,
										UnitID = ci.UnitID,
										Unit = un.UnitEnglishName,
										Sold = 0,
										AvailableNow = 0,
										Tags = (from t in _dbContext.Tags
												join cit in _dbContext.ChangeItemTag
												on t.ID equals cit.ItemID
												where cit.ItemID == ItemID
												select new ChanngeItemTagResponse
												{
													ID = t.ID,
													Name = t.TagEnglish
												}).ToList(),
										Reviews = (from ir in _dbContext.ItemReviews
												   join u in _dbContext.Users on ir.UserID equals u.ID
												   where ir.ItemID == ItemID
												   select new ChangeItemReviewResponse
												   {
													   UserName = $"{u.FirstName} {u.LastName}",
													   Picture = u.Picture,
													   Rating = ir.Rating,
													   Reviews = ir.Review,
													   ReviewDate = ir.RecordDateTime
												   }).ToList(),
										RecentOrder = (from O in _dbContext.Orders
													   join oi in _dbContext.OrderItems on O.ID equals oi.OrderID
													   join i in _dbContext.Items on oi.ItemID equals i.ID
													   join u in _dbContext.Users on O.UserID equals u.ID
													   where i.ID == ItemID
													   select new ChangeRecentOrder
													   {
														   OrderID = Convert.ToInt32(O.ID),
														   Driver = $"{u.FirstName} {u.LastName}",
														   Buyer = $"{u.FirstName} {u.LastName}",
														   PickUp = O.OrderPickUpTime,
														   Delivery = O.ActualDeliveryTime,
														   Rating = O.Rating,
														   Reviews = O.Review
													   }).ToList(),
									}).FirstOrDefaultAsync();

			return changeitem;
		}

	}
}
