using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class EmployeeLoginHistoryRepository : EFRepository<EmployeeLoginHistory>, IEmployeeLoginHistoryRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public EmployeeLoginHistoryRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EmployeeLoginHistory> GetByUserID(long UserID)
        {
            return await _dbContext.EmployeesLoginHistory
                .Where(x => x.EmployeeID == UserID).OrderByDescending(x => x.ID).FirstOrDefaultAsync();
        }

        public async Task<PaginationResponse> GetEmployeesLoginHistory(PaginationRequest request)
        {
            var list = _dbContext.EmployeesLoginHistory.Select(x => x).OrderByDescending(x => x.ID);

            var totalRecords = list.Count();
            var totalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize);

            return new PaginationResponse
            {
                CurrentPage = request.PageNumber,
                TotalPages = totalPages,
                PageSize = request.PageSize,
                TotalCount = totalRecords,
                Data = await list.Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize).ToListAsync()
            };
        }

    }
}
