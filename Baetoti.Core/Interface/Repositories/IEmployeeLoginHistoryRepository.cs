using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Response.Shared;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IEmployeeLoginHistoryRepository : IAsyncRepository<EmployeeLoginHistory>
    {
        Task<EmployeeLoginHistory> GetByUserID(long UserID);

        Task<PaginationResponse> GetEmployeesLoginHistory(PaginationRequest request);

    }
}
