using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Helper;
using Baetoti.Shared.Response.Cart;
using Baetoti.Shared.Response.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class CartRepository : EFRepository<Cart>, ICartRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public CartRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Cart>> GetByUserID(long UserID)
        {
            return await _dbContext.Carts.Where(x => x.UserID == UserID).ToListAsync();
        }

        public async Task<Cart> GetByItemID(long ItemID, long UserID)
        {
            return await _dbContext.Carts.Where(x => x.ItemID == ItemID && x.UserID == UserID).FirstOrDefaultAsync();
        }

        public async Task<GetCartResponse> GetCartDetail(long UserID)
        {
            List<CartItem> cartItems = await (from c in _dbContext.Carts
                                              join i in _dbContext.Items on c.ItemID equals i.ID
                                              join cat in _dbContext.Categories on i.CategoryID equals cat.ID
                                              join p in _dbContext.Providers on i.ProviderID equals p.ID
                                              join s in _dbContext.Stores on p.ID equals s.ProviderID
                                              where c.UserID == UserID
                                              select new CartItem
                                              {
                                                  ItemID = i.ID,
                                                  ItemName = i.Name,
                                                  ItemComments = c.Comments,
                                                  Quantity = c.Quantity,
                                                  StoreLogo = s.BusinessLogo,
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
                                                  StoreID = s.ID,
                                                  StoreName = s.Name,
                                                  Price = i.Price,
                                                  BaetotiPrice = i.BaetotiPrice,
                                                  StoreMinimumItemPrice = (from it in _dbContext.Items
                                                                           join pr in _dbContext.Providers on it.ProviderID equals pr.ID
                                                                           join st in _dbContext.Stores on pr.ID equals st.ProviderID
                                                                           where st.ID == s.ID
                                                                           orderby i.Price descending
                                                                           select i.Price).FirstOrDefault(),
                                                  CategoryID = cat.ID,
                                                  Category = cat.CategoryName,
                                                  CategoryArabic = cat.CategoryArabicName
                                              }).ToListAsync();

            if (cartItems == null)
                throw new Exception($"No item found in Cart for UserID:{UserID}");

            UserLocation userLocation = await _dbContext.UserLocations
                .Where(x => x.UserID == UserID && x.IsDefault == true).FirstOrDefaultAsync();

            if (userLocation == null)
                throw new Exception($"Default location is not set for UserID:{UserID}");

            Store store = await (from c in _dbContext.Carts.Where(c => c.UserID == UserID)
                                 join i in _dbContext.Items on c.ItemID equals i.ID
                                 join p in _dbContext.Providers on i.ProviderID equals p.ID
                                 join s in _dbContext.Stores on p.ID equals s.ProviderID
                                 select s).FirstOrDefaultAsync();

            decimal distance = Convert.ToDecimal(Helper.GetDistance(userLocation.Latitude, userLocation.Longitude, store.Latitude, store.Longitude));

            var charge = await _dbContext.DriverConfigs
                .Where(c => distance <= c.MaximumDistance && !c.MarkAsDeleted).FirstOrDefaultAsync();
            if (charge == null)
                throw new Exception($"Delivery config is not defined for Distance:{distance} KM");

            decimal extraDistance = 0, normalRate = charge.RatePerKM, extraRate = 0;
            if (distance > charge.ToKM)
            {
                extraDistance = distance - charge.ToKM;
                extraRate = (extraDistance / charge.AdditionalKM) * charge.AdditionalRatePerKM;
            }
            decimal totalDeliveryCharge = normalRate + extraRate;

            decimal totalCharges = cartItems.Sum(x => x.BaetotiPrice * x.Quantity);
            GetCartResponse cartResponse = new GetCartResponse
            {
                CartItems = cartItems,
                DeliveryCharges = totalDeliveryCharge,
                TotalCharges = totalCharges,
                ProviderCommision = totalCharges - cartItems.Sum(x => x.Price * x.Quantity),
                DriverCommision = charge.DriverComission == 0 ? 0 : charge.DriverComission / 100 * totalDeliveryCharge
            };
            cartResponse.ServiceFee = (charge.ServiceFee / 100 * cartResponse.TotalCharges) + charge.ServiceFeeFixed;
            return cartResponse;
        }

        public async Task<bool> CheckForStore(long ItemID, long UserID)
        {
            List<CartItem> cartItems = await (from c in _dbContext.Carts
                                              join i in _dbContext.Items on c.ItemID equals i.ID
                                              join p in _dbContext.Providers on i.ProviderID equals p.ID
                                              join s in _dbContext.Stores on p.ID equals s.ProviderID
                                              where c.UserID == UserID
                                              select new CartItem
                                              {
                                                  ItemID = i.ID,
                                                  StoreID = s.ID
                                              }).ToListAsync();

            if (cartItems.Any())
            {
                Store store = await (from i in _dbContext.Items
                                     join p in _dbContext.Providers on i.ProviderID equals p.ID
                                     join s in _dbContext.Stores on p.ID equals s.ProviderID
                                     where i.ID == ItemID
                                     select s).FirstOrDefaultAsync();
                if (store.ID == cartItems.FirstOrDefault().StoreID)
                    return true;
                else
                    return false;
            }
            else
            {
                return true;
            }
        }

    }
}
