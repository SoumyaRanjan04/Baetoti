using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Helper;
using Baetoti.Shared.Request.Store;
using Baetoti.Shared.Response.Order;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.Store;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RatingAndReviews = Baetoti.Shared.Response.Store.RatingAndReviews;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class StoreRepository : EFRepository<Store>, IStoreRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public StoreRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CheckDuplicateURL(StoreRequest request)
        {
            Store store = null;
            if (request.ID == 0)
            {
                if (!string.IsNullOrEmpty(request.FacebookURL))
                {
                    store = await _dbContext.Stores.Where(s => s.FacebookURL == request.FacebookURL).FirstOrDefaultAsync();
                    if (store != null)
                        throw new Exception("Facebook URL already exists.");
                }
                if (!string.IsNullOrEmpty(request.TwitterURL))
                {
                    store = await _dbContext.Stores.Where(s => s.TwitterURL == request.TwitterURL).FirstOrDefaultAsync();
                    if (store != null)
                        throw new Exception("Twitter URL already exists.");
                }
                if (!string.IsNullOrEmpty(request.TikTokURL))
                {
                    store = await _dbContext.Stores.Where(s => s.TikTokURL == request.TikTokURL).FirstOrDefaultAsync();
                    if (store != null)
                        throw new Exception("TikTok URL already exists.");
                }
                if (!string.IsNullOrEmpty(request.InstagramURL))
                {
                    store = await _dbContext.Stores.Where(s => s.InstagramURL == request.InstagramURL).FirstOrDefaultAsync();
                    if (store != null)
                        throw new Exception("Instagram URL already exists.");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(request.FacebookURL))
                {
                    store = await _dbContext.Stores.Where(s => s.FacebookURL == request.FacebookURL).FirstOrDefaultAsync();
                    if (store != null)
                        if (store.ID != request.ID)
                            throw new Exception("Facebook URL already exists.");
                }
                if (!string.IsNullOrEmpty(request.TwitterURL))
                {
                    store = await _dbContext.Stores.Where(s => s.TwitterURL == request.TwitterURL).FirstOrDefaultAsync();
                    if (store != null)
                        if (store.ID != request.ID)
                            throw new Exception("Twitter URL already exists.");
                }
                if (!string.IsNullOrEmpty(request.TikTokURL))
                {
                    store = await _dbContext.Stores.Where(s => s.TikTokURL == request.TikTokURL).FirstOrDefaultAsync();
                    if (store != null)
                        if (store.ID != request.ID)
                            throw new Exception("TikTok URL already exists.");
                }
                if (!string.IsNullOrEmpty(request.InstagramURL))
                {
                    store = await _dbContext.Stores.Where(s => s.InstagramURL == request.InstagramURL).FirstOrDefaultAsync();
                    if (store != null)
                        if (store.ID != request.ID)
                            throw new Exception("Instagram URL already exists.");
                }
            }
            return true;
        }

        public async Task<List<StoreResponse>> GetAllByUserId(long id)
        {
            return await (from s in _dbContext.Stores
                          join p in _dbContext.Providers on s.ProviderID equals p.ID
                          join u in _dbContext.Users on p.UserID equals u.ID
                          where u.ID == id
                          select new StoreResponse
                          {
                              ID = s.ID,
                              ProviderID = s.ProviderID,
                              Name = s.Name,
                              Description = s.Description,
                              Location = s.Location,
                              Longitude = s.Longitude,
                              Latitude = s.Latitude,
                              IsAddressHidden = s.IsAddressHidden,
                              VideoURL = s.VideoURL,
                              FacebookURL = s.FacebookURL,
                              TwitterURL = s.TwitterURL,
                              TikTokURL = s.TikTokURL,
                              InstagramURL = s.InstagramURL,
                              Images = (from si in _dbContext.StoreImages
                                        where si.StoreID == s.ID
                                        select new ImageResponse
                                        {
                                            Image = si.Image
                                        }).ToList(),
                              BusinessLogo = s.BusinessLogo,
                              InstagramGallery = s.InstagramGallery,
                              Status = s.Status,
                          }).ToListAsync();
        }

        public async Task<Store> GetByProviderId(long providerId)
        {
            return await _dbContext.Stores.Where(s => s.ProviderID == providerId).FirstOrDefaultAsync();
        }

        public async Task<StoreResponse> GetByStoreId(long id, long? UserId)
        {
            var res = await (from s in _dbContext.Stores
                             join p in _dbContext.Providers on s.ProviderID equals p.ID
                             join fs in _dbContext.FavouriteStores.Where(x => x.UserID == UserId) on s.ID equals fs.StoreID into favtStores
                             from ufs in favtStores.DefaultIfEmpty()
                             join it in _dbContext.InstragramTokens on s.ID equals it.StoreID into tempToken
                             from it in tempToken.DefaultIfEmpty()
                             where s.ID == id
                             select new StoreResponse
                             {
                                 ID = s.ID,
                                 ProviderID = s.ProviderID,
                                 ProviderUserID = p.UserID,
                                 IsOnline = p.IsOnline,
                                 Name = s.Name,
                                 Description = s.Description,
                                 Location = s.Location,
                                 Longitude = s.Longitude,
                                 Latitude = s.Latitude,
                                 IsAddressHidden = s.IsAddressHidden,
                                 VideoURL = s.VideoURL,
                                 FacebookURL = s.FacebookURL,
                                 TwitterURL = s.TwitterURL,
                                 TikTokURL = s.TikTokURL,
                                 InstagramURL = s.InstagramURL,
                                 InstagramToken = it != null ? it.Token : "",
                                 Images = (from si in _dbContext.StoreImages
                                           where si.StoreID == s.ID
                                           select new ImageResponse
                                           {
                                               Image = si.Image
                                           }).ToList(),
                                 RatingAndReviews = (from sr in _dbContext.StoreReviews
                                                     where sr.StoreID == s.ID
                                                     select new RatingAndReviews
                                                     {
                                                         Rating = sr.Rating,
                                                         Reviews = sr.Reviews,
                                                         RecordDateTime = sr.RecordDateTime
                                                     }).ToList(),
                                 OrderAcceptanceRate = (
                                    from od in _dbContext.Orders
                                    join po in _dbContext.ProviderOrders.Where(x => x.ProviderID == p.ID) on od.ID equals po.OrderID
                                    group new { od, po } by od.Status into statusGroup
                                    select statusGroup.Count(po => po.po.ID > 0) > 0 ?
                                           (double)statusGroup.Sum(od => od.od.Status == 2 ? 1 : 0) /
                                           statusGroup.Count(po => po.po.ID > 0) * 100 : 0.0
                                ).FirstOrDefault(),
                                 Tags = (from st in _dbContext.StoreTags
                                         join t in _dbContext.Tags on st.TagID equals t.ID
                                         where st.StoreID == s.ID
                                         select new StoreTagResponse
                                         {
                                             TagNameEnglish = t.TagEnglish,
                                             TagNameArabic = t.TagArabic
                                         }).ToList(),
                                 TotalFavourites = (from f in _dbContext.FavouriteStores
                                                    where f.StoreID == s.ID
                                                    select s.ID).Count(),
                                 BusinessLogo = s.BusinessLogo,
                                 InstagramGallery = s.InstagramGallery,
                                 Status = s.Status,
                                 IsFavourite = ufs != null
                             }).FirstOrDefaultAsync();

            var items = from i in _dbContext.Items
                        join p in _dbContext.Providers on i.ProviderID equals p.ID
                        join s in _dbContext.Stores on p.ID equals s.ProviderID
                        select i;

            var minItem = await items.OrderBy(x => x.Price).FirstOrDefaultAsync();
            var maxItem = await items.OrderByDescending(x => x.Price).FirstOrDefaultAsync();

            res.TotalRating = res.RatingAndReviews.Count();
            res.AverageRating = res.TotalRating > 0 ? res.RatingAndReviews.Average(x => x.Rating) : 0;
            res.MinimumItemPrice = minItem == null ? 0 : minItem.Price;
            res.MaximumItemPrice = maxItem == null ? 0 : maxItem.Price;

            return res;
        }

        public async Task<List<StoreResponse>> GetFavouriteStore(long UserID)
        {
            return await (from s in _dbContext.Stores
                          join fs in _dbContext.FavouriteStores on s.ID equals fs.StoreID
                          where fs.UserID == UserID
                          select new StoreResponse
                          {
                              ID = s.ID,
                              ProviderID = s.ProviderID,
                              Name = s.Name,
                              Description = s.Description,
                              Location = s.Location,
                              Longitude = s.Longitude,
                              Latitude = s.Latitude,
                              IsAddressHidden = s.IsAddressHidden,
                              VideoURL = s.VideoURL,
                              FacebookURL = s.FacebookURL,
                              TwitterURL = s.TwitterURL,
                              TikTokURL = s.TikTokURL,
                              InstagramURL = s.InstagramURL,
                              Images = (from si in _dbContext.StoreImages
                                        where si.StoreID == s.ID
                                        select new ImageResponse
                                        {
                                            Image = si.Image
                                        }).ToList(),
                              BusinessLogo = s.BusinessLogo,
                              InstagramGallery = s.InstagramGallery,
                              Status = s.Status,
                              AverageRating = _dbContext.StoreReviews
                                .Where(sr => sr.StoreID == s.ID)
                                .Select(sr => (decimal?)sr.Rating)
                                .Average() ?? 0
                          }).ToListAsync();
        }

        public async Task<List<StoreResponse>> GetNearBy(StoreFilterRequest storeFilterRequest, long? UserID)
        {
            UserLocation userLocation = new UserLocation();
            if (UserID == null)
            {
                userLocation.Latitude = (double)storeFilterRequest.Latitude;
                userLocation.Longitude = (double)storeFilterRequest.Longitude;
            }
            else
            {
                userLocation = _dbContext.UserLocations.FirstOrDefault(x => x.UserID == UserID && x.IsDefault == true);
            }
            var res = await (from s in _dbContext.Stores
                             join p in _dbContext.Providers on s.ProviderID equals p.ID
                             join i in _dbContext.Items on p.ID equals i.ProviderID
                             where (string.IsNullOrEmpty(storeFilterRequest.StoreName) || s.Name.Trim().ToLower().Contains(storeFilterRequest.StoreName.Trim().ToLower())) &&
                             (storeFilterRequest.ProviderOnlineStatus == 0 || storeFilterRequest.ProviderOnlineStatus == 1 ? p.IsOnline == true : p.IsOnline == false) &&
                             (storeFilterRequest.MaximumPrice == 0 || (i.Price >= storeFilterRequest.MinimumPrice && i.Price <= storeFilterRequest.MaximumPrice)) &&
                              UserID == null || p.UserID != UserID
                             select new StoreResponse
                             {
                                 ID = s.ID,
                                 ProviderID = s.ProviderID,
                                 ProviderUserID = p.UserID,
                                 IsOnline = p.IsOnline,
                                 Name = s.Name,
                                 Description = s.Description,
                                 Location = s.Location,
                                 Longitude = s.Longitude,
                                 Latitude = s.Latitude,
                                 IsAddressHidden = s.IsAddressHidden,
                                 VideoURL = s.VideoURL,
                                 FacebookURL = s.FacebookURL,
                                 TwitterURL = s.TwitterURL,
                                 TikTokURL = s.TikTokURL,
                                 InstagramURL = s.InstagramURL,
                                 Images = (from si in _dbContext.StoreImages
                                           where si.StoreID == s.ID
                                           select new ImageResponse
                                           {
                                               Image = si.Image
                                           }).ToList(),
                                 BusinessLogo = s.BusinessLogo,
                                 InstagramGallery = s.InstagramGallery,
                                 Status = s.Status,
                                 AverageRating = _dbContext.StoreReviews
                                .Where(sr => sr.StoreID == s.ID)
                                .Select(sr => (decimal?)sr.Rating)
                                .Average() ?? 0,
                                 Distance = userLocation == null ? 0 : Helper.GetDistance(userLocation.Latitude, userLocation.Longitude, s.Latitude, s.Longitude)
                             }).ToListAsync();
            return storeFilterRequest.LocationRange == 0 ? res.Where(x => x.Distance <= 10).DistinctBy(x => x.ID).ToList() :
                res.Where(x => x.Distance <= storeFilterRequest.LocationRange).DistinctBy(x => x.ID).ToList();
        }

        public async Task<List<StoreResponse>> GetOnlineProvidersStore(decimal LocationRange, long? UserID)
        {
            var result = await _dbContext.Stores
            .Join(_dbContext.Providers, s => s.ProviderID, p => p.ID, (s, p) => new { s, p })
            .Join(_dbContext.Items, sp => sp.p.ID, i => i.ProviderID, (sp, i) => new { sp.s, sp.p, i })
            .Join(_dbContext.Categories, spi => spi.i.CategoryID, c => c.ID, (spi, c) => new { spi.s, spi.p, spi.i, c })
            .Join(_dbContext.StoreImages, spic => spic.s.ID, si => si.StoreID, (spic, si) => new { spic.s, spic.p, spic.i, spic.c, si })
            .Where(spic => spic.p.IsOnline == true && (spic.p.ProviderStatus == 1 || spic.p.ProviderStatus == 3) && (UserID == null || spic.p.UserID != UserID) )
            .Select(spic => new StoreResponse
            {
                ID = spic.s.ID,
                ProviderID = spic.s.ProviderID,
                Name = spic.s.Name,
                Description = spic.s.Description,
                Location = spic.s.Location,
                Longitude = spic.s.Longitude,
                Latitude = spic.s.Latitude,
                IsAddressHidden = spic.s.IsAddressHidden,
                VideoURL = spic.s.VideoURL,
                FacebookURL = spic.s.FacebookURL,
                TwitterURL = spic.s.TwitterURL,
                TikTokURL = spic.s.TikTokURL,
                InstagramURL = spic.s.InstagramURL,
                Images = _dbContext.StoreImages
                    .Where(si => si.StoreID == spic.s.ID)
                    .Select(si => new ImageResponse { Image = si.Image })
                    .ToList(),
                BusinessLogo = spic.s.BusinessLogo,
                InstagramGallery = spic.s.InstagramGallery,
                Status = spic.s.Status,
                Category = spic.c.CategoryName,
                AverageRating = _dbContext.StoreReviews
                                .Where(sr => sr.StoreID == spic.s.ID)
                                .Select(sr => (decimal?)sr.Rating)
                                .Average() ?? 0
            })
            .ToListAsync();

            var distinctResult = result.GroupBy(s => s.ID).Select(g => g.First());

            return distinctResult.ToList();
        }

        public async Task<StoreReviewsAndRatingsResponse> GetReviewsAndRatings(StoreReviewAndRatingFilterRequest request)
        {
            var result = from sr in _dbContext.StoreReviews.Where(x => x.StoreID == request.StoreID)
                         join s in _dbContext.Stores on sr.StoreID equals s.ID
                         join u in _dbContext.Users on sr.UserID equals u.ID into userTemp
                         from tu in userTemp.DefaultIfEmpty()
                         join dr in _dbContext.Drivers on sr.DriverID equals dr.ID into driverTemp
                         from dt in driverTemp.DefaultIfEmpty()
                         join du in _dbContext.Users on dt.UserID equals du.ID into driverUserTemp
                         from dut in driverUserTemp.DefaultIfEmpty()
                         select new RatingAndReviews
                         {
                             Rating = sr.Rating,
                             Reviews = sr.Reviews,
                             StoreName = s.Name,
                             StoreLogo = s.BusinessLogo,
                             RecordDateTime = sr.RecordDateTime,
                             OrderID = sr.OrderID,
                             ReviewerName = tu == null ? $"{dut.FirstName} {dut.LastName}" : $"{tu.FirstName} {tu.LastName}",
                             ReviewerPicture = tu == null ? dut.Picture : tu.Picture,
                             items = (from ir in _dbContext.ItemReviews
                                      join i in _dbContext.Items on ir.ItemID equals i.ID
                                      join c in _dbContext.Categories on i.CategoryID equals c.ID
                                      where ir.OrderID == sr.OrderID
                                      select new ItemReviewList
                                      {
                                          ItemID = i.ID,
                                          ItemName = i.Name,
                                          ItemNameArabic = i.ArabicName,
                                          Rating = ir.Rating,
                                          Reviews = ir.Review,
                                          ReviewImage = ir.Image,
                                          ItemCategory = c.CategoryName,
                                          ItemCategoryArabic = c.CategoryArabicName,
                                          RecordDateTime = ir.RecordDateTime,
                                          ItemImages = (from im in _dbContext.ItemImages
                                                        where im.ItemID == i.ID
                                                        select new ImageResponse
                                                        {
                                                            Image = im.Image
                                                        }).ToList()
                                      }).ToList()
                         };

            StoreReviewsAndRatingsResponse response = new StoreReviewsAndRatingsResponse();

            if (request.RatingType == (int)RatingFilter.All)
                response.StoreRatingAndReviews = await result.ToListAsync();
            else if (request.RatingType == (int)RatingFilter.Positive)
                response.StoreRatingAndReviews = await result.Where(x => x.Rating > 2).ToListAsync();
            else if (request.RatingType == (int)RatingFilter.Negative)
                response.StoreRatingAndReviews = await result.Where(x => x.Rating <= 2).ToListAsync();
            else if (request.RatingType == (int)RatingFilter.Recent)
                response.StoreRatingAndReviews = await result.Where(x => x.RecordDateTime >= DateTime.Now.ToTimeZoneTime("Arab Standard Time").AddDays(-7)).ToListAsync();
            else
                throw new Exception("Invalid Rating Filter");

            response.RatingCount = response.StoreRatingAndReviews.Count();
            if (response.StoreRatingAndReviews.Count > 0)
            {
                response.AverageRating = response.StoreRatingAndReviews.Select(x => x.Rating).Average();
            }
            else
            {
                response.AverageRating = 0;
            }

            return response;
        }

        public async Task<List<StoreTagResponse>> GetStoreTags(long storeID)
        {
            return await (from st in _dbContext.StoreTags
                          join t in _dbContext.Tags on st.TagID equals t.ID
                          where st.StoreID == storeID
                          select new StoreTagResponse
                          {
                              TagNameEnglish = t.TagEnglish,
                              TagNameArabic = t.TagArabic
                          }).ToListAsync();
        }

    }
}
