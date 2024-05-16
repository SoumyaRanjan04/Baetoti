using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Request.Contract;
using Baetoti.Shared.Response.Contract;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class ContractRepository : EFRepository<Contract>, IContractRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public ContractRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ContractResponse> Get(ContractRequest request)
        {
            return await _dbContext.Contracts.Where(c => c.ContractType == request.ContractType)
                .Select(c => new ContractResponse
                {
                    Content = c.Content
                }).FirstOrDefaultAsync();
        }

    }
}
