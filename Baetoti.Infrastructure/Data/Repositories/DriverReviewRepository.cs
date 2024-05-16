using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Shared;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Baetoti.Shared.Extentions;

namespace Baetoti.Infrastructure.Data.Repositories
{
	public class DriverReviewRepository : EFRepository<DriverReview>, IDriverReviewRepository
	{

		private readonly BaetotiDbContext _dbContext;

		public DriverReviewRepository(BaetotiDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<PaginationResponse> GetReviews(RatingAndReviewRequest request, long? UserID)
		{
			var reviews = from br in _dbContext.DriverReviews
						  join dr in _dbContext.Drivers on br.DriverID equals dr.ID
						  where dr.UserID == UserID
						  join pr in _dbContext.Providers on br.ProviderID equals pr.ID into providerTemp
						  from pt in providerTemp.DefaultIfEmpty()
						  join pu in _dbContext.Users on pt.UserID equals pu.ID into providerUserTemp
						  from put in providerUserTemp.DefaultIfEmpty()
						  join u in _dbContext.Users on br.UserID equals u.ID into userTemp
						  from ut in userTemp.DefaultIfEmpty()
						  select new RatingAndReviewResponseData
						  {
							  OrderID = br.OrderID,
							  DriverID = br.DriverID,
							  ProviderID = br.ProviderID,
							  UserID = br.UserID,
							  Rating = br.Rating,
							  Reviews = br.Reviews,
							  RecordDateTime = br.RecordDateTime,
							  ReviewerName = put == null ? $"{ut.FirstName} {ut.LastName}" : $"{put.FirstName} {put.LastName}",
							  ReviewerPicture = put == null ? ut.Picture : put.Picture
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
