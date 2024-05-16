using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Contract;
using Baetoti.Shared.Response.Contract;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IContractRepository : IAsyncRepository<Contract>
    {
        Task<ContractResponse> Get(ContractRequest request);

    }
}