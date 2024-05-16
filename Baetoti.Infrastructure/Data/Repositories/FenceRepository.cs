using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Response.Fence;
using Baetoti.Shared.Response.OperationalConfig;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class FenceRepository : EFRepository<Fence>, IFenceRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public FenceRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<OperationalConfigResponse>> GetAllFences()
        {
            return await (from f in _dbContext.Fences
                          join c in _dbContext.Countries on f.CountryID equals c.ID
                          join ec in _dbContext.Employee on
                          new { ID = f.CreatedBy } equals new { ID = (int?)ec.ID } into tempCreatedBy
                          from ec in tempCreatedBy.DefaultIfEmpty()
                          join eu in _dbContext.Employee on
                          new { ID = f.UpdatedBy } equals new { ID = (int?)eu.ID } into tempUpdatedBy
                          from eu in tempUpdatedBy.DefaultIfEmpty()
                          select new OperationalConfigResponse
                          {
                              ID = f.ID,
                              Title = f.Title,
                              Notes = f.Notes,
                              Country = c.CountryName,
                              Region = f.Region,
                              City = f.City,
                              Status = ((FenceStatus)f.FenceStatus).ToString(),
                              CreatedAt = f.CreatedAt,
                              LastUpdatedAt = f.LastUpdatedAt,
                              StopedAt = f.StopedAt,
                              CreatedBy = $"{ec.FirstName} {ec.LastName}",
                              UpdatedBy = eu != null ? $"{eu.FirstName} {eu.LastName}" : "",
                              TotalUser = _dbContext.Users.Where(u => u.CityID == f.CityID).Count(),
                              TotalDriver = _dbContext.Drivers.Where(u => u.CityId == f.CityID).Count(),
                              TotalProvider = _dbContext.Providers.Where(u => u.CityId == f.CityID).Count()
                          }).ToListAsync();
        }

        public async Task<OperationalConfigResponseByID> GetFenceByID(long ID)
        {
            return await (from f in _dbContext.Fences.Where(f => f.ID == ID)
                          join c in _dbContext.Countries on f.CountryID equals c.ID
                          join ec in _dbContext.Employee on
                          new { ID = f.CreatedBy } equals new { ID = (int?)ec.ID } into tempCreatedBy
                          from ec in tempCreatedBy.DefaultIfEmpty()
                          join eu in _dbContext.Employee on
                          new { ID = f.UpdatedBy } equals new { ID = (int?)eu.ID } into tempUpdatedBy
                          from eu in tempUpdatedBy.DefaultIfEmpty()
                          select new OperationalConfigResponseByID
                          {
                              ID = f.ID,
                              Title = f.Title,
                              Notes = f.Notes,
                              Country = c.CountryName,
                              CountryID = f.CountryID,
                              RegionID = f.RegionID,
                              CityID = f.CityID,
                              Region = f.Region,
                              City = f.City,
                              FenceStatus = f.FenceStatus,
                              CreatedAt = f.CreatedAt,
                              StopedAt = f.StopedAt,
                              LastUpdatedAt = f.LastUpdatedAt,
                              CreatedBy = $"{ec.FirstName} {ec.LastName}",
                              UpdatedBy = eu != null ? $"{eu.FirstName} {eu.LastName}" : "",
                              TotalUser = _dbContext.Users.Where(u => u.CityID == f.CityID).Count(),
                              TotalDriver = _dbContext.Drivers.Where(u => u.CityId == f.CityID).Count(),
                              TotalProvider = _dbContext.Providers.Where(u => u.CityId == f.CityID).Count()
                          }).FirstOrDefaultAsync();
        }

        public async Task<List<FenceResponse>> GetFenceList()
        {
            return await (from f in _dbContext.Fences
                          where f.FenceStatus == 1
                          select new FenceResponse
                          {
                              ID = f.ID,
                              Name = f.Title
                          }).ToListAsync();
        }

        public async Task<FenceResponse> GetFenceByCityID(string CityID)
        {
            return await (from f in _dbContext.Fences
                          where f.CityID == CityID && f.FenceStatus == 1
                          select new FenceResponse
                          {
                              ID = f.ID,
                              Name = f.Title
                          }).FirstOrDefaultAsync();
        }

    }
}
