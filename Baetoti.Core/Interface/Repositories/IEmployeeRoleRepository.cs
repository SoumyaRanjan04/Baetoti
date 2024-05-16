using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IEmployeeRoleRepository : IAsyncRepository<EmployeeRole>
    {
        Task<EmployeeRole> GetByUserId(long UserId);
    }
}
