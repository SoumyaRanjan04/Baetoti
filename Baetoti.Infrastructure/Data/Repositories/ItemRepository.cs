using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.Item;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Request.Item;
using System;
using System.Collections.Generic;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Helper;
using MoreLinq;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class ItemRepository : EFRepository<Item>, IItemRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public ItemRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginationResponse> GetAll(decimal Longitude, decimal Latitude, int itemFilter, long? UserID, string UserRole, int PageNumber, int PageSize)
        {
            UserLocation userLocation = new UserLocation();
            if (UserID != null)
            {
                userLocation = _dbContext.UserLocations.FirstOrDefault(x => x.UserID == UserID && x.IsDefault == true);
            }
            else
            {
                userLocation.Latitude = (double)Latitude;
                userLocation.Longitude = (double)Longitude;

            }

            var itemList = from i in _dbContext.Items
                           join c in _dbContext.Categories on i.CategoryID equals c.ID
                           join sc in _dbContext.SubCategories on i.SubCategoryID equals sc.ID
                           join u in _dbContext.Units on i.UnitID equals u.ID
                           join p in _dbContext.Providers on i.ProviderID equals p.ID
                           join pu in _dbContext.Users on p.UserID equals pu.ID
                           join s in _dbContext.Stores on p.ID equals s.ProviderID
                           join ci in _dbContext.Carts.Where(x => x.UserID == UserID) on i.ID equals ci.ItemID into cartItems
                           from ct in cartItems.DefaultIfEmpty()
                           where i.MarkAsDeleted == false
                           && UserID == null || p.UserID != UserID
                           && (!string.IsNullOrEmpty(UserRole) || (i.ItemStatus == 1 || i.ItemStatus == 3))
                           select new ItemListResponse
                           {
                               ID = i.ID,
                               StoreID = s.ID,
                               StoreName = s.Name,
                               StoreLogo = s.BusinessLogo,
                               ProviderID = i.ProviderID,
                               ProviderUserID = p.UserID,
                               Title = i.Name,
                               StoreImages = (from si in _dbContext.StoreImages
                                              where si.StoreID == s.ID
                                              select new ImageResponse
                                              {
                                                  Image = si.Image
                                              }).ToList(),
                               ItemImages = (from ii in _dbContext.ItemImages
                                             where ii.ItemID == i.ID
                                             select new ImageResponse
                                             {
                                                 Image = ii.Image
                                             }).ToList(),
                               Description = i.Description,
                               CategoryID = c.ID,
                               Category = c.CategoryName,
                               CategoryArabic = c.CategoryArabicName,
                               CategoryColor = c.Color,
                               CategoryDescription = c.Description,
                               CategoryStatus = c.CategoryStatus,
                               CategoryPicture = c.Picture,
                               SubCategoryID = sc.ID,
                               SubCategory = sc.SubCategoryName,
                               SubCategoryArabic = sc.SubCategoryArabicName,
                               SubCategoryPicture = sc.Picture,
                               SubCategoryStatus = sc.SubCategoryStatus,
                               OrderedQuantity = (from o in _dbContext.OrderItems where o.ItemID == i.ID select o.ItemID).Count(),
                               Revenue = (from oi in _dbContext.OrderItems where oi.ItemID == i.ID select oi.Quantity * oi.SoldPrice).Sum(),
                               AveragePreparationTime = i.AveragePreparationTime,
                               CreateDate = i.CreatedAt,
                               Status = i.ItemStatus,
                               StatusValue = ((ItemStatus)i.ItemStatus).ToString(),
                               Price = i.Price,
                               BaetotiPrice = i.BaetotiPrice,
                               UnitID = u.ID,
                               Unit = u.UnitEnglishName,
                               UnitArabic = u.UnitArabicName,
                               UnitType = u.UnitType,
                               Rating = _dbContext.ItemReviews
                                        .Where(ir => ir.ItemID == i.ID)
                                        .Select(ir => (decimal?)ir.Rating)
                                        .Average() ?? 0,
                               CategoryImage = c.Picture,
                               Distance = userLocation == null ? 0 : Helper.GetDistance(userLocation.Latitude, userLocation.Longitude, s.Latitude, s.Longitude),
                               AvailableQuantity = i.AvailableQuantity,
                               VisitCount = _dbContext.ItemVisits.Count(iv => iv.ItemID == i.ID),
                               IsItemAlreadyViewed = _dbContext.ItemVisits.Count(iv => iv.ItemID == i.ID && (UserID == null || iv.UserID == UserID)) > 0,
                               Tags = (from t in _dbContext.Tags
                                       join it in _dbContext.ItemTags on t.ID equals it.TagID
                                       where it.ItemID == i.ID
                                       select new ItemTagsList
                                       {
                                           TagID = t.ID,
                                           TagArabic = t.TagArabic,
                                           TagEnglish = t.TagEnglish
                                       }).ToList(),
                               IsAddedToCart = ct != null
                           };

            var itemResponse = new ItemResponse();
            itemResponse.Total = await itemList.CountAsync();
            itemResponse.Active = await itemList.CountAsync(x => x.Status != (int)ItemStatus.Inactive && x.Status != (int)ItemStatus.Rejected);
            itemResponse.Inactive = await itemList.CountAsync(x => x.Status == (int)ItemStatus.Inactive);
            itemResponse.Flaged = await itemList.CountAsync(x => x.Status == (int)ItemStatus.Rejected);

            int totalRecords = 0;

            if (itemFilter == (int)ItemFilter.AvailableNow)
            {
                itemList = itemList.Where(x => x.AvailableQuantity > 0);
                totalRecords = await itemList.CountAsync();
                itemResponse.items = await itemList.Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize).ToListAsync();
            }
            else if (itemFilter == (int)ItemFilter.Featured)
            {
                totalRecords = await itemList.CountAsync();
                itemResponse.items = await itemList.Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize).ToListAsync();
            }
            else if (itemFilter == (int)ItemFilter.NearMe)
            {
                itemResponse.items = await itemList.ToListAsync();
                totalRecords = itemResponse.items.Where(x => x.Distance <= 10.0).Count();
                itemResponse.items = itemResponse.items.Where(x => x.Distance <= 10.0)
                    .Skip((PageNumber - 1) * PageSize).Take(PageSize).ToList();
            }
            else if (itemFilter == (int)ItemFilter.TopRated)
            {
                itemList = itemList.Where(x => x.Rating >= 4);
                totalRecords = await itemList.CountAsync();
                itemResponse.items = await itemList.Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize).ToListAsync();
            }
            else if (itemFilter == (int)ItemFilter.Latest)
            {
                totalRecords = await itemList.CountAsync();
                itemResponse.items = await itemList.OrderByDescending(x => x.CreateDate)
                    .Skip((PageNumber - 1) * PageSize).Take(PageSize).ToListAsync();
            }
            else
            {
                totalRecords = await itemList.CountAsync();
                itemResponse.items = await itemList.Skip((PageNumber - 1) * PageSize)
                    .Take(PageSize).ToListAsync();
            }

            var totalPages = (int)Math.Ceiling((double)totalRecords / PageSize);

            return new PaginationResponse
            {
                CurrentPage = PageNumber,
                TotalPages = totalPages,
                PageSize = PageSize,
                TotalCount = totalRecords,
                Data = itemResponse
            };
        }

        public async Task<PaginationResponse> GetFilteredItemsDataAsync(ItemFilterRequest filterRequest, long? UserID)
        {
            UserLocation userLocation = new UserLocation();
            if (UserID != null)
            {
                userLocation = _dbContext.UserLocations.FirstOrDefault(x => x.UserID == UserID && x.IsDefault);
            }
            else
            {
                userLocation.Latitude = (double)filterRequest.Location.Latitude;
                userLocation.Longitude = (double)filterRequest.Location.Longitude;
            }

            var itemList = from i in _dbContext.Items 
                           join c in _dbContext.Categories on i.CategoryID equals c.ID
                           join sc in _dbContext.SubCategories on i.SubCategoryID equals sc.ID
                           join u in _dbContext.Units on i.UnitID equals u.ID
                           join p in _dbContext.Providers on i.ProviderID equals p.ID
                           join pu in _dbContext.Users on p.UserID equals pu.ID
                           join s in _dbContext.Stores on p.ID equals s.ProviderID
                           join ci in _dbContext.Carts.Where(x => x.UserID == UserID) on i.ID equals ci.ItemID into cartItems
                           from ct in cartItems.DefaultIfEmpty()
                           where i.MarkAsDeleted == false &&
                           (string.IsNullOrEmpty(filterRequest.Name) || i.Name.ToLower().Contains(filterRequest.Name.Trim().ToLower())) &&
                           (string.IsNullOrEmpty(filterRequest.ArabicName) || i.ArabicName.ToLower().Contains(filterRequest.ArabicName.Trim().ToLower())) &&
                           (filterRequest.Price == null || filterRequest.Price == 0 || i.Price == filterRequest.Price) &&
                           (filterRequest.CategoryID == null || filterRequest.CategoryID == 0 || i.CategoryID == filterRequest.CategoryID) &&
                           (filterRequest.SubCategoryID == null || filterRequest.SubCategoryID == 0 || (i.SubCategoryID == filterRequest.SubCategoryID && p.UserID != UserID)) &&
                           (filterRequest.UnitID == null || filterRequest.UnitID == 0 || i.UnitID == filterRequest.UnitID) &&
                           (filterRequest.StoreID == null || filterRequest.StoreID == 0 || s.ID == filterRequest.StoreID)
                           && ((i.ItemStatus == 1 || i.ItemStatus == 3) || (UserID == null ? false : p.UserID == UserID))
                           
                           select new ItemListResponse
                           {
                               ID = i.ID,
                               StoreID = s.ID,
                               StoreName = s.Name,
                               StoreLogo = s.BusinessLogo,
                               ProviderID = i.ProviderID,
                               Title = i.Name,
                               StoreImages = (from si in _dbContext.StoreImages
                                              where si.StoreID == s.ID
                                              select new ImageResponse
                                              {
                                                  Image = si.Image
                                              }).ToList(),
                               ItemImages = (from ii in _dbContext.ItemImages
                                             where ii.ItemID == i.ID
                                             select new ImageResponse
                                             {
                                                 Image = ii.Image
                                             }).ToList(),
                               Description = i.Description,
                               CategoryID = c.ID,
                               Category = c.CategoryName,
                               CategoryArabic = c.CategoryArabicName,
                               CategoryColor = c.Color,
                               CategoryDescription = c.Description,
                               CategoryStatus = c.CategoryStatus,
                               CategoryPicture = c.Picture,
                               SubCategoryID = sc.ID,
                               SubCategory = sc.SubCategoryName,
                               SubCategoryArabic = sc.SubCategoryArabicName,
                               SubCategoryPicture = sc.Picture,
                               SubCategoryStatus = sc.SubCategoryStatus,
                               OrderedQuantity = (from o in _dbContext.OrderItems where o.ItemID == i.ID select o.ItemID).Count(),
                               Revenue = (from oi in _dbContext.OrderItems where oi.ItemID == i.ID select oi.Quantity * oi.SoldPrice).Sum(),
                               AveragePreparationTime = i.AveragePreparationTime,
                               CreateDate = i.CreatedAt,
                               StatusValue = ((ItemStatus)i.ItemStatus).ToString(),
                               Status = i.ItemStatus,
                               Price = i.Price,
                               BaetotiPrice = i.BaetotiPrice,
                               UnitID = u.ID,
                               Unit = u.UnitEnglishName,
                               UnitArabic = u.UnitArabicName,
                               UnitType = u.UnitType,
                               Rating = _dbContext.ItemReviews
                                        .Where(ir => ir.ItemID == i.ID)
                                        .Select(ir => (decimal?)ir.Rating)
                                        .Average() ?? 0,
                               CategoryImage = c.Picture,
                               Distance = userLocation == null ? 0 : Helper.GetDistance(userLocation.Latitude, userLocation.Longitude, s.Latitude, s.Longitude),
                               AvailableQuantity = i.AvailableQuantity,
                               VisitCount = _dbContext.ItemVisits.Count(iv => iv.ItemID == i.ID),
                               IsItemAlreadyViewed = _dbContext.ItemVisits.Count(iv => iv.ItemID == i.ID && (UserID == null || iv.UserID == UserID)) > 0,
                               Tags = (from t in _dbContext.Tags
                                       join it in _dbContext.ItemTags on t.ID equals it.TagID
                                       where it.ItemID == i.ID
                                       select new ItemTagsList
                                       {
                                           TagID = t.ID,
                                           TagArabic = t.TagArabic,
                                           TagEnglish = t.TagEnglish
                                       }).ToList(),
                               IsAddedToCart = ct != null
                           };
            var itemResponse = new ItemResponse();
            var groupedItems =  itemList.AsEnumerable().DistinctBy(x => x.ID).ToList();
           // itemList = (IQueryable<ItemListResponse>)await groupedItems.AsQueryable().ToListAsync();
            itemResponse.Total =  groupedItems.Count();
            itemResponse.Active =  groupedItems.Count(x => x.Status == (int)ItemStatus.Active);
            itemResponse.Inactive =  groupedItems.Count(x => x.Status == (int)ItemStatus.Inactive);
            itemResponse.Flaged = 0; //itemList.Count();

            int totalRecords = 0;

            if (filterRequest.itemFilter == (int)ItemFilter.AvailableNow)
            {
                groupedItems =  groupedItems.Where(x => x.AvailableQuantity > 0).ToList();
                totalRecords =  groupedItems.Count();
                itemResponse.items =  groupedItems.Skip((filterRequest.PageNumber - 1) * filterRequest.PageSize)
                    .Take(filterRequest.PageSize).ToList();
            }
            else if (filterRequest.itemFilter == (int)ItemFilter.Featured)
            {
                totalRecords =  groupedItems.Count();
                itemResponse.items =  groupedItems.Skip((filterRequest.PageNumber - 1) * filterRequest.PageSize)
                    .Take(filterRequest.PageSize).ToList();
            }
            else if (filterRequest.itemFilter == (int)ItemFilter.NearMe)
            {
                itemResponse.items =  groupedItems.ToList();
                totalRecords = itemResponse.items.Where(x => x.Distance <= 10.0).Count();
                itemResponse.items = itemResponse.items.Where(x => x.Distance <= 10.0)
                    .Skip((filterRequest.PageNumber - 1) * filterRequest.PageSize).Take(filterRequest.PageSize).ToList();
            }
            else if (filterRequest.itemFilter == (int)ItemFilter.TopRated)
            {
                groupedItems = groupedItems.Where(x => x.Rating >= 4).ToList();
                totalRecords =  groupedItems.Count();
                itemResponse.items =  groupedItems.Skip((filterRequest.PageNumber - 1) * filterRequest.PageSize)
                    .Take(filterRequest.PageSize).ToList();
            }
            else if (filterRequest.itemFilter == (int)ItemFilter.Latest)
            {
                totalRecords =  groupedItems.Count();
                itemResponse.items =  groupedItems.OrderByDescending(x => x.CreateDate)
                    .Skip((filterRequest.PageNumber - 1) * filterRequest.PageSize).Take(filterRequest.PageSize).ToList();
            }
            else
            {
                totalRecords =  groupedItems.Count();
                itemResponse.items =  groupedItems.Skip((filterRequest.PageNumber - 1) * filterRequest.PageSize)
                    .Take(filterRequest.PageSize).ToList();
            }

            var totalPages = (int)Math.Ceiling((double)totalRecords / filterRequest.PageSize);

            return new PaginationResponse
            {
                CurrentPage = filterRequest.PageNumber,
                TotalPages = totalPages,
                PageSize = filterRequest.PageSize,
                TotalCount = totalRecords,
                Data = itemResponse
            };
        }

        public async Task<ItemResponseByID> GetByID(long ItemID, long UserID)
        {
            UserLocation userLocation = _dbContext.UserLocations.FirstOrDefault(x => x.UserID == UserID && x.IsDefault);
            var item = from i in _dbContext.Items
                       join c in _dbContext.Categories on i.CategoryID equals c.ID
                       join sc in _dbContext.SubCategories on i.SubCategoryID equals sc.ID
                       join p in _dbContext.Providers on i.ProviderID equals p.ID
                       join u in _dbContext.Users on p.UserID equals u.ID
                       join un in _dbContext.Units on i.UnitID equals un.ID
                       join s in _dbContext.Stores on p.ID equals s.ProviderID
                       join ci in _dbContext.Carts on i.ID equals ci.ItemID into cartItems
                       from ct in cartItems.DefaultIfEmpty()
                       where i.ID == ItemID
                       select new ItemResponseByID
                       {
                           ID = i.ID,
                           ProviderID = i.ProviderID,
                           StoreID = s.ID,
                           StoreName = s.Name,
                           StoreLogo = s.BusinessLogo,
                           Location = s.Location,
                           Title = i.Name,
                           Description = i.Description,
                           CategoryID = i.CategoryID,
                           Category = c.CategoryName,
                           SubCategoryID = i.SubCategoryID,
                           SubCategory = sc.SubCategoryName,
                           Quantity = i.AvailableQuantity,
                           StoreImages = (from si in _dbContext.StoreImages
                                          where si.StoreID == s.ID
                                          select new ImageResponse
                                          {
                                              Image = si.Image
                                          }).ToList(),
                           ItemImages = (from ii in _dbContext.ItemImages
                                         where ii.ItemID == i.ID
                                         select new ImageResponse
                                         {
                                             Image = ii.Image
                                         }).ToList(),
                           TotalRevenue = (from oi in _dbContext.OrderItems where oi.ItemID == i.ID select oi.Quantity * oi.SoldPrice).Sum(),
                           Price = i.Price,
                           BaetotiPrice = i.BaetotiPrice,
                           //AverageRating = _dbContext.ItemReviews.Where(i => i.ItemID == ItemID).Average(i => i.Rating),
                           AveragePreparationTime = i.AveragePreparationTime,
                           UnitID = i.UnitID,
                           Unit = un.UnitEnglishName,
                           Sold = _dbContext.OrderItems.Where(i => i.ItemID == ItemID).Sum(i => i.Quantity),
                           AvailableNow = i.AvailableQuantity,
                           VisitCount = _dbContext.ItemVisits.Count(iv => iv.ItemID == i.ID),
                           CategoryImage = c.Picture,
                           Distance = userLocation == null ? 0 : Helper.GetDistance(userLocation.Latitude, userLocation.Longitude, s.Latitude, s.Longitude),
                           Tags = (from t in _dbContext.Tags
                                   join it in _dbContext.ItemTags
                                   on t.ID equals it.TagID
                                   where it.ItemID == ItemID
                                   select new TagResponse
                                   {
                                       ID = t.ID,
                                       Name = t.TagEnglish
                                   }).ToList(),
                           Reviews = (from ir in _dbContext.ItemReviews
                                      join u in _dbContext.Users on ir.UserID equals u.ID
                                      where ir.ItemID == ItemID
                                      select new ReviewResponse
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
                                          join dor in _dbContext.DriverOrders on O.ID equals dor.OrderID into tempDriverOrder
                                          from dor in tempDriverOrder.DefaultIfEmpty()
                                          join dr in _dbContext.Drivers on dor.DriverID equals dr.ID into tempDriver
                                          from dr in tempDriver.DefaultIfEmpty()
                                          join du in _dbContext.Users on dr.UserID equals du.ID into tempDriverUser
                                          from du in tempDriverUser.DefaultIfEmpty()
                                          where i.ID == ItemID
                                          select new RecentOrder
                                          {
                                              OrderID = Convert.ToInt32(O.ID),
                                              UserID = u.ID,
                                              DriverUserID = du != null ? du.ID : 0,
                                              Driver = du != null ? $"{u.FirstName} {u.LastName}" : "",
                                              Buyer = $"{u.FirstName} {u.LastName}",
                                              PickUp = O.OrderPickUpTime,
                                              Delivery = O.ActualDeliveryTime,
                                              Rating = O.Rating,
                                              Review = O.Review,
                                          }).OrderByDescending(x => x.OrderID).Take(10).ToList(),
                           isAddedToCart = ct != null
                       };
            var response = await item.FirstOrDefaultAsync();

            var ratingList = await _dbContext.ItemReviews.Where(i => i.ItemID == ItemID).ToListAsync();
            if (ratingList.Any())
            {
                response.AverageRating = ratingList.Average(i => i.Rating);
            }

            return response;
        }

        public async Task<ItemsOnBoardingResponse> GetAllItemsOnBoardingRequest(ItemOnBoardingRequest itemOnBoardingRequest)
        {
            ItemsOnBoardingResponse itemsOnBoardingResponse = new ItemsOnBoardingResponse();

            itemsOnBoardingResponse.itemStatistic = new ItemStatistic
            {
                Pending = (from req in _dbContext.Items where req.ItemStatus == 2 && (((itemOnBoardingRequest.StartDate != null && req.CreatedAt > itemOnBoardingRequest.StartDate) || (itemOnBoardingRequest.StartDate == null)) && ((itemOnBoardingRequest.EndDate != null && req.CreatedAt < itemOnBoardingRequest.EndDate) || (itemOnBoardingRequest.EndDate == null))) select req).Count(), // InActive //pending 
                Approved = (from req in _dbContext.Items where req.ItemStatus == 3 && (((itemOnBoardingRequest.StartDate != null && req.CreatedAt > itemOnBoardingRequest.StartDate) || (itemOnBoardingRequest.StartDate == null)) && ((itemOnBoardingRequest.EndDate != null && req.CreatedAt < itemOnBoardingRequest.EndDate) || (itemOnBoardingRequest.EndDate == null))) select req).Count(),
                Rejected = (from req in _dbContext.Items where req.ItemStatus == 4 && (((itemOnBoardingRequest.StartDate != null && req.CreatedAt > itemOnBoardingRequest.StartDate) || (itemOnBoardingRequest.StartDate == null)) && ((itemOnBoardingRequest.EndDate != null && req.CreatedAt < itemOnBoardingRequest.EndDate) || (itemOnBoardingRequest.EndDate == null))) select req).Count()
            };

            itemsOnBoardingResponse.closeItemOnBoardingRequest = await (from i in _dbContext.Items
                                                                        where i.ItemStatus != 2 &&
                                                                      ((itemOnBoardingRequest.StartDate != null && i.CreatedAt > itemOnBoardingRequest.StartDate) ||
                                                                       (itemOnBoardingRequest.StartDate == null)) &&
                                                                      ((itemOnBoardingRequest.EndDate != null && i.CreatedAt < itemOnBoardingRequest.EndDate) ||
                                                                       (itemOnBoardingRequest.EndDate == null))
                                                                        join tu in _dbContext.Employee on i.RequestClosedBy equals tu.ID
                                                                        into temp
                                                                        from u in temp.DefaultIfEmpty()
                                                                        select new CloseItemOnBoardingRequest
                                                                        {
                                                                            ID = i.ID,
                                                                            Name = i.Name,
                                                                            DateAndTimeOfRequest = i.RequestDate,
                                                                            DateAndTimeOfClose = i.RequestCloseDate,
                                                                            By = u == null ? "" : $"{u.FirstName} {u.LastName}",
                                                                            Status = ((ItemStatus)i.ItemStatus).ToString()
                                                                        }).OrderBy(oo => oo.DateAndTimeOfRequest).ToListAsync();

            itemsOnBoardingResponse.openItemOnBoardingRequest = await (from i in _dbContext.Items
                                                                       where i.ItemStatus == 2 &&
                                                                        ((itemOnBoardingRequest.StartDate != null && i.CreatedAt > itemOnBoardingRequest.StartDate) ||
                                                                           (itemOnBoardingRequest.StartDate == null)) &&
                                                                          ((itemOnBoardingRequest.EndDate != null && i.CreatedAt < itemOnBoardingRequest.EndDate) ||
                                                                           (itemOnBoardingRequest.EndDate == null))
                                                                       join c in _dbContext.Categories on i.CategoryID equals c.ID
                                                                       join sc in _dbContext.SubCategories on i.SubCategoryID equals sc.ID
                                                                       select new OpenItemOnBoardingRequest
                                                                       {
                                                                           ID = i.ID,
                                                                           ProviderId = i.ProviderID,
                                                                           Caegory = c.CategoryName,
                                                                           SubCaegory = sc.SubCategoryName,
                                                                           DateAndTimeOfRequest = i.RequestDate,
                                                                           Price = i.Price,
                                                                       }).OrderBy(oo => oo.DateAndTimeOfRequest).ToListAsync();

            var groupedData = _dbContext.Items
                                .GroupBy(item => item.CreatedAt.Value.Date)
                                .Select(group => new
                                {
                                    Date = group.Key,
                                    Count = group.Count()
                                })
                                .ToList(); // Execute the query and retrieve the data

            // Perform the transformations using LINQ to Objects
            List<Shared.Response.Item.Data> Data = groupedData
                                                .OrderBy(itemData => itemData.Date)
                                                .Select(itemData => new Shared.Response.Item.Data
                                                {
                                                    Date = itemData.Date.ToString("MMM dd"),
                                                    Item = itemData.Count
                                                })
                                                .ToList();
            // Calculate current date's items count
            DateTime currentDate = DateTime.Now.Date;
            int currentItemCount = Data.FirstOrDefault(data => data.Date == currentDate.ToString("MMM dd"))?.Item ?? 0;

            // Calculate min, max, and avg values
            int minValue = Data.Min(data => data.Item);
            int maxValue = Data.Max(data => data.Item);
            double avgValue = Data.Average(data => data.Item);

            itemsOnBoardingResponse.graphData.Data.AddRange(Data);
            itemsOnBoardingResponse.graphData.Current = currentItemCount;
            itemsOnBoardingResponse.graphData.Avg = avgValue;
            itemsOnBoardingResponse.graphData.Min = minValue;
            itemsOnBoardingResponse.graphData.Max = maxValue;

            return itemsOnBoardingResponse;
        }

        public async Task<ItemComparisonResponse> ViewByID(long ItemID, long ID)
        {
            var item = await (from i in _dbContext.Items
                              join c in _dbContext.Categories on i.CategoryID equals c.ID
                              join sc in _dbContext.SubCategories on i.SubCategoryID equals sc.ID
                              join un in _dbContext.Units on i.UnitID equals un.ID
                              join ci in _dbContext.ChangeItem on i.ID equals ci.ItemId
                              where i.ID == ItemID && ci.ID == ID
                              select new ItemComparisonResponse
                              {
                                  Before = new Before
                                  {
                                      ID = i.ID,
                                      Title = i.Name,
                                      Description = i.Description,
                                      Category = c.CategoryName,
                                      SubCategory = sc.SubCategoryName,
                                      Price = i.Price,
                                      BaetotiPrice = i.BaetotiPrice,
                                      Unit = un.UnitEnglishName,
                                      Tags = (from t in _dbContext.Tags
                                              join it in _dbContext.ItemTags on t.ID equals it.TagID
                                              where it.ItemID == ItemID
                                              select new TagResponse
                                              {
                                                  ID = t.ID,
                                                  Name = t.TagEnglish
                                              }).ToList(),
                                      Images = (from im in _dbContext.ItemImages
                                                where im.ItemID == ItemID
                                                select new ImageResponse
                                                {
                                                    Image = im.Image
                                                }).ToList(),
                                  },
                                  After = new After
                                  {
                                      ID = ci.ID,
                                      Title = ci.Name,
                                      Description = ci.Description,
                                      Category = c.CategoryName,
                                      SubCategory = sc.SubCategoryName,
                                      Price = ci.Price,
                                      BaetotiPrice = ci.BaetotiPrice,
                                      Unit = un.UnitEnglishName,
                                      Tags = (from t in _dbContext.Tags
                                              join it in _dbContext.ChangeItemTag on t.ID equals it.TagID
                                              where it.ItemID == ItemID && it.ChangeItemID == ID
                                              select new TagResponse
                                              {
                                                  ID = t.ID,
                                                  Name = t.TagEnglish
                                              }).ToList(),
                                      Images = (from im in _dbContext.ChangeItemImages
                                                where im.ItemID == ItemID && im.ChangeItemID == ID
                                                select new ImageResponse
                                                {
                                                    Image = im.Image
                                                }).ToList(),
                                  }
                              }).FirstOrDefaultAsync();
            return item;
        }

        public async Task<ItemDashboardResponse> GetAllForDashboard(long? UserID, decimal Longitude, decimal Latitude)
        {
            UserLocation userLocation = new UserLocation();
            if (UserID != null)
            {
                userLocation = _dbContext.UserLocations.FirstOrDefault(x => x.UserID == UserID && x.IsDefault == true);
            }
            else
            {
                userLocation.Latitude = (double)Latitude;
                userLocation.Longitude = (double)Longitude;
            }
            var itemList = from i in _dbContext.Items
                           join c in _dbContext.Categories on i.CategoryID equals c.ID
                           join sc in _dbContext.SubCategories on i.SubCategoryID equals sc.ID
                           join u in _dbContext.Units on i.UnitID equals u.ID
                           join p in _dbContext.Providers on i.ProviderID equals p.ID
                           join pu in _dbContext.Users on p.UserID equals pu.ID
                           join s in _dbContext.Stores on p.ID equals s.ProviderID
                           join ci in _dbContext.Carts.Where(x => x.UserID == UserID) on i.ID equals ci.ItemID into cartItems
                           from ct in cartItems.DefaultIfEmpty()
                           where i.MarkAsDeleted == false
                           && UserID == null || p.UserID != UserID //(UserID == null ? true : p.UserID != UserID)
                           && p.MarkAsDeleted == false
                           && (i.ItemStatus == 1 || i.ItemStatus == 3)
                           select new ItemListResponse
                           {
                               ID = i.ID,
                               StoreID = s.ID,
                               StoreName = s.Name,
                               StoreLogo = s.BusinessLogo,
                               ProviderID = i.ProviderID,
                               Title = i.Name,
                               StoreImages = (from si in _dbContext.StoreImages
                                              where si.StoreID == s.ID
                                              select new ImageResponse
                                              {
                                                  Image = si.Image
                                              }).ToList(),
                               ItemImages = (from ii in _dbContext.ItemImages
                                             where ii.ItemID == i.ID
                                             select new ImageResponse
                                             {
                                                 Image = ii.Image
                                             }).ToList(),
                               Description = i.Description,
                               CategoryID = c.ID,
                               SubCategoryID = sc.ID,
                               Category = c.CategoryName,
                               SubCategory = sc.SubCategoryName,
                               OrderedQuantity = (from o in _dbContext.OrderItems where o.ItemID == i.ID select o.ItemID).Count(),
                               Revenue = (from oi in _dbContext.OrderItems where oi.ItemID == i.ID select oi.Quantity * oi.SoldPrice).Sum(),
                               AveragePreparationTime = i.AveragePreparationTime,
                               CreateDate = i.CreatedAt,
                               Status = i.ItemStatus,
                               StatusValue = ((ItemStatus)i.ItemStatus).ToString(),
                               Price = i.Price,
                               BaetotiPrice = i.BaetotiPrice,
                               Unit = u.UnitEnglishName,
                               Rating = _dbContext.ItemReviews
                                        .Where(ir => ir.ItemID == i.ID)
                                        .Select(ir => (decimal?)ir.Rating)
                                        .Average() ?? 0,
                               RatingCount = _dbContext.ItemReviews
                                        .Where(ir => ir.ItemID == i.ID)
                                        .Count(),
                               CategoryImage = c.Picture,
                               Distance = userLocation == null ? 0 : Helper.GetDistance(userLocation.Latitude, userLocation.Longitude, s.Latitude, s.Longitude),
                               AvailableQuantity = i.AvailableQuantity,
                               VisitCount = _dbContext.ItemVisits.Count(iv => iv.ItemID == i.ID),
                               IsItemAlreadyViewed = _dbContext.ItemVisits.Count(iv => iv.ItemID == i.ID && (UserID == null || iv.UserID == UserID)) > 0,
                               Tags = (from t in _dbContext.Tags
                                       join it in _dbContext.ItemTags on t.ID equals it.TagID
                                       where it.ItemID == i.ID
                                       select new ItemTagsList
                                       {
                                           TagArabic = t.TagArabic,
                                           TagEnglish = t.TagEnglish
                                       }).ToList(),
                               IsAddedToCart = ct != null
                           };

            ItemDashboardResponse response = new ItemDashboardResponse();

            response.Available.Items = await itemList.Where(x => x.AvailableQuantity > 0).Take(10).ToListAsync();
            response.NearMe.Items = (await itemList.ToListAsync()).Where(x => x.Distance <= 10.0).Take(10).ToList();
            response.TopRated.Items = await itemList.Where(x => x.Rating >= 4).Take(10)
                                        .OrderByDescending(x => x.RatingCount).ToListAsync();
            response.Featured.Items = await itemList.Take(10).ToListAsync();
            response.Latest.Items = await itemList.OrderByDescending(x => x.CreateDate).Take(10).ToListAsync();

            return response;
        }

        public async Task<List<Item>> GetListofAllItems()
        {
            return _dbContext.Items.AsEnumerable().ToList();
        }


    }
}
