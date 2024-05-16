using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Shared;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using Baetoti.Shared.Enum;
using System.Collections.Generic;
using Baetoti.Shared.Extentions;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class BuyerReviewRepository : EFRepository<BuyerReview>, IBuyerReviewRepository
	{

		private readonly BaetotiDbContext _dbContext;

		public BuyerReviewRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<PaginationResponse> GetReviews(RatingAndReviewRequest request, long? UserID)
		{
			var reviews = from br in _dbContext.BuyerReviews.Where(x => x.UserID == UserID)
						  join dr in _dbContext.Drivers on br.DriverID equals dr.ID into driverTemp
						  from dt in driverTemp.DefaultIfEmpty()
						  join du in _dbContext.Users on dt.UserID equals du.ID into driverUserTemp
						  from dut in driverUserTemp.DefaultIfEmpty()
						  join pr in _dbContext.Providers on br.ProviderID equals pr.ID into providerTemp
						  from pt in providerTemp.DefaultIfEmpty()
						  join pu in _dbContext.Users on pt.UserID equals pu.ID into providerUserTemp
						  from put in providerUserTemp.DefaultIfEmpty()
						  select new RatingAndReviewResponseData
						  {
							  OrderID = br.OrderID,
							  DriverID = br.DriverID,
							  ProviderID = br.ProviderID,
							  UserID = br.UserID,
							  Rating = br.Rating,
							  Reviews = br.Reviews,
							  RecordDateTime = br.RecordDateTime,
							  ReviewerName = put == null ? $"{dut.FirstName} {dut.LastName}" : $"{put.FirstName} {put.LastName}",
							  ReviewerPicture = put == null ? dut.Picture : put.Picture
						  };

			List<RatingAndReviewResponseData> ratingAndReviews;
			if (request.RatingFilter == (int)RatingFilter.Recent)
				ratingAndReviews = await reviews.Where(x => x.RecordDateTime >= DateTime.Now.ToTimeZoneTime("Arab Standard Time").AddDays(-7)).ToListAsync();
			else if (request.RatingFilter == (int)RatingFilter.Positive)
				ratingAndReviews = await reviews.Where(x => x.Rating > 2).ToListAsync();
			else if (request.RatingFilter == (int)RatingFilter.Negative)
				ratingAndReviews = await reviews.Where(x => x.Rating <= 2).ToListAsync();
			else
				ratingAndReviews = await reviews.ToListAsync();

			RatingAndReviewResponse response = new RatingAndReviewResponse()
			{
				Data = ratingAndReviews,
				AverageRating = ratingAndReviews.Count > 0 ? ratingAndReviews.Average(x => x.Rating) : 0,
				RatingCount = ratingAndReviews.Count,
			};

			var totalRecords = ratingAndReviews.Count();
			var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

			return new PaginationResponse
			{
				CurrentPage = request.PageNumber,
				TotalPages = totalPages,
				PageSize = request.PageSize,
				TotalCount = totalRecords,
				Data = response
			};
		}

	}
}
