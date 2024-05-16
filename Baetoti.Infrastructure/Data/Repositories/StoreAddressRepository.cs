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

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class StoreAddressRepository : EFRepository<StoreAddress>, IStoreAddressRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public StoreAddressRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<StoreAddress>> GetAllByStoreId(long id)
        {

             var query = await (from st in _dbContext.StoreAddress
                                join t in _dbContext.Stores on st.StoreID equals t.ID
                                where st.StoreID == id
                                select new StoreAddress
                                {
                                    ID = st.ID,
                                    StoreID = st.StoreID,
                                    Location = st.Location,
                                    Latitude =st.Latitude,
                                    Longitude = st.Longitude,
                                    CreatedAt = st.CreatedAt,

                                }).ToListAsync();

            return query;
        }

        
    }
}

