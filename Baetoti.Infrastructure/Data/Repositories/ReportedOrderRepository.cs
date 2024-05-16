using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Request.ReportedOrder;
using Baetoti.Shared.Response.ReportedOrder;
using Baetoti.Shared.Response.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class ReportedOrderRepository : EFRepository<ReportedOrder>, IReportedOrderRepository
	{

		private readonly BaetotiDbContext _dbContext;

		public ReportedOrderRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<ReportedOrder> CheckIfExists(ReportedOrderRequest request)
		{
			return await _dbContext.ReportedOrders.Where(o => o.ReportedByUserType == request.ReportedByUserType &&
													o.OrderID == request.OrderID).FirstOrDefaultAsync();
		}

		public async Task<PaginationResponse> GetAll(GetReportedOrderRequest request)
		{
			var list = from ro in _dbContext.ReportedOrders
					   join o in _dbContext.Orders on ro.OrderID equals o.ID
					   join po in _dbContext.ProviderOrders on ro.OrderID equals po.OrderID
					   join s in _dbContext.Stores on po.ProviderID equals s.ProviderID
					   join t in _dbContext.Transactions on ro.OrderID equals t.OrderID
					   select new ReportedOrderResponse
					   {
						   ID = ro.ID,
						   StoreName = s.Name,
						   Price = t.Amount,
						   OrderDate = o.CreatedAt,
						   OrderID = ro.OrderID,
						   Items = (from oi in _dbContext.OrderItems.Where(x => x.OrderID == ro.OrderID)
									join i in _dbContext.Items on oi.ItemID equals i.ID
									select new ReportedOrderItemResponse
									{
										ItemID = oi.ItemID,
										ItemName = i.Name,
										ItemNameArabic = i.ArabicName
									}).ToList()
					   };

			var totalRecords = list.Count();
			var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

			PaginationResponse response = new PaginationResponse
			{
				CurrentPage = request.PageNumber,
				TotalPages = totalPages,
				PageSize = request.PageSize,
				TotalCount = totalRecords
			};

			if (!string.IsNullOrEmpty(request.SearchValue))
				response.Data = (await list.ToListAsync())
								.Where(x =>
									x.StoreName.ToLower().Contains(request.SearchValue.ToLower())
								)
								.OrderByDescending(x => x.OrderDate)
								.Skip((request.PageNumber - 1) * request.PageSize)
								.Take(request.PageSize)
								.ToList();
			else
				response.Data = await list.OrderByDescending(x => x.OrderDate)
					.Skip((request.PageNumber - 1) * request.PageSize)
					.Take(request.PageSize).ToListAsync();

			return response;
		}

		public async Task<ReportedOrderByIDResponse> GetReportedOrderByID(long ID)
		{
			return await (from ro in _dbContext.ReportedOrders.Where(x => x.ID == ID)
						  join o in _dbContext.Orders on ro.OrderID equals o.ID
						  join po in _dbContext.ProviderOrders on ro.OrderID equals po.OrderID
						  join s in _dbContext.Stores on po.ProviderID equals s.ProviderID
						  join t in _dbContext.Transactions on ro.OrderID equals t.OrderID
						  select new ReportedOrderByIDResponse
						  {
							  ID = ro.ID,
							  StoreName = s.Name,
							  Price = t.Amount,
							  OrderDate = o.CreatedAt,
							  OrderID = ro.OrderID,
							  Items = (from oi in _dbContext.OrderItems.Where(x => x.OrderID == ro.OrderID)
									   join i in _dbContext.Items on oi.ItemID equals i.ID
									   select new ReportedOrderItemResponse
									   {
										   ItemID = oi.ItemID,
										   ItemName = i.Name,
										   ItemNameArabic = i.ArabicName
									   }).ToList()
						  }).FirstOrDefaultAsync();
		}

	}
}
