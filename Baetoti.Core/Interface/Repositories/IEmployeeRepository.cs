using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Base;
using Baetoti.Shared.Response.Employee;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baetoti.Core.Interface.Repositories
{
    public interface IEmployeeRepository : IAsyncRepository<Employee>
    {
        Task<Employee> AuthenticateUser(Employee user);

        Task<List<string>> GetRolesAsync(Employee user);

        Task<EmployeeResponse> GetAll();

        Task<EmployeeList> GetById(long ID);

    }
}
