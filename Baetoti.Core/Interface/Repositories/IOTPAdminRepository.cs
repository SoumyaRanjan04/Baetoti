using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IOTPAdminRepository : IAsyncRepository<OTPAdmin>
    {
        Task<OTPAdmin> GetByEmployeeIdAsync(long EmployeeID);

    }
}
