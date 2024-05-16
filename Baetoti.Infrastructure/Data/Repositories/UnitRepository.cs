using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Response.Unit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class UnitRepository : EFRepository<Unit>, IUnitRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public UnitRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<UnitResponse>> GetAllUnitsAsync()
        {
            return await (from u in _dbContext.Units.Where(u => u.MarkAsDeleted == false)
                          join eCreated in _dbContext.Employee on u.CreatedBy.GetValueOrDefault() equals eCreated.ID
                          //join eUpdated in _dbContext.Employee on u.UpdatedBy.GetValueOrDefault() equals eUpdated.ID into pb


                          select new UnitResponse
                          {
                              ID = u.ID,
                              CreatedAt = u.CreatedAt,
                              CreatedByName = eCreated.FirstName + " " + eCreated.LastName,
                              CreatedBy = u.CreatedBy,
                              LastUpdatedAt = u.LastUpdatedAt,
                              UnitArabicName = u.UnitArabicName,
                              UnitEnglishName = u.UnitEnglishName,
                              UnitStatus = u.UnitStatus,
                              UnitType = u.UnitType,
                              UpdatedBy = u.UpdatedBy,
                              UpdatedByName = _dbContext.Employee.Where(ww => ww.ID == u.UpdatedBy).FirstOrDefault().FirstName + " " + _dbContext.Employee.Where(ww => ww.ID == u.UpdatedBy).FirstOrDefault().LastName,

                          }).ToListAsync();
        }

        public async Task<UnitResponse> GetUnitByID(int ID)
        {
            return await (from u in _dbContext.Units.Where(u => u.MarkAsDeleted == false && u.ID == ID)
                          join eCreated in _dbContext.Employee on u.CreatedBy equals (int?)eCreated.ID
                          join eUpdated in _dbContext.Employee on u.UpdatedBy equals (int?)eUpdated.ID into tempUpdatedBy
                          from eUpdated in tempUpdatedBy.DefaultIfEmpty()
                          select new UnitResponse
                          {
                              ID = u.ID,
                              CreatedAt = u.CreatedAt,
                              CreatedByName = eCreated.FirstName + " " + eCreated.LastName,
                              CreatedBy = u.CreatedBy,
                              LastUpdatedAt = u.LastUpdatedAt,
                              UnitArabicName = u.UnitArabicName,
                              UnitEnglishName = u.UnitEnglishName,
                              UnitStatus = u.UnitStatus,
                              UnitType = u.UnitType,
                              UpdatedBy = u.UpdatedBy,
                              UpdatedByName = eUpdated.FirstName + " " + eUpdated.LastName
                          }).FirstOrDefaultAsync();
        }

    }
}
