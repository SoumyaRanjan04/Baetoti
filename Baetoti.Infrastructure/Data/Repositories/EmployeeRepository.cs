using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Context;
using Baetoti.Infrastructure.Data.Repositories.Base;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Response.Employee;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.Infrastructure.Data.Repositories
{
    public class EmployeeRepository : EFRepository<Employee>, IEmployeeRepository
    {

        private readonly BaetotiDbContext _dbContext;

        public EmployeeRepository(BaetotiDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<EmployeeResponse> GetAll()
        {
            var list = await (from e in _dbContext.Employee
                              join er in _dbContext.EmployeeRoles on e.ID equals er.EmployeeId
                              join r in _dbContext.Roles on er.RoleId equals r.ID
                              join rt in _dbContext.Employee on e.ReportTo equals rt.ID
                              where e.MarkAsDeleted == false
                              select new EmployeeList
                              {
                                  ID = e.ID,
                                  FirstName = e.FirstName,
                                  LastName = e.LastName,
                                  JoiningDate = e.JoiningDate,
                                  Location = e.Location,
                                  DepartmentID = e.DepartmentID,
                                  DesignationID = e.DesignationID,
                                  Username = e.Username,
                                  Gender = e.Gender,
                                  Shift = e.Shift,
                                  RoleId = er.RoleId,
                                  Role = r.Name,
                                  Email = e.Email,
                                  DOB = e.DOB,
                                  Phone = e.Phone,
                                  ReportTo = e.ReportTo,
                                  ReportToName = $"{rt.FirstName} {rt.LastName}",
                                  Address = e.Address,
                                  Goals = e.Goals,
                                  Skills = e.Skills,
                                  RefreshToken = e.RefreshToken,
                                  EmployeeStatus = e.EmployeeStatus,
                                  EmployeeStatusValue = ((EmployementStatus)e.EmployeeStatus).ToString(),
                                  CreatedBy = e.CreatedBy,
                                  UpdatedBy = e.UpdatedBy,
                                  Picture = e.Picture
                              }).ToListAsync();
            return new EmployeeResponse
            {
                employeeList = list,
                Online = list.Count(x => x.EmployeeStatus == 1),
                Offline = list.Count(x => x.EmployeeStatus != 1)
            };
        }

        public async Task<List<string>> GetRolesAsync(Employee user)
        {
            return await (from ur in _dbContext.EmployeeRoles
                          join
                          r in _dbContext.Roles on ur.RoleId equals r.ID
                          where ur.EmployeeId == user.ID
                          select r.Name).ToListAsync();
        }

        public async Task<Employee> AuthenticateUser(Employee user)
        {
            return await _dbContext.Employee.Where(x => x.Username.ToLower() == user.Username.ToLower() && x.EmployeeStatus == (int)EmployementStatus.Active).FirstOrDefaultAsync();
        }

        public async Task<EmployeeList> GetById(long ID)
        {
            return await (from e in _dbContext.Employee
                          join er in _dbContext.EmployeeRoles on e.ID equals er.EmployeeId
                          join r in _dbContext.Roles on er.RoleId equals r.ID
                          join rt in _dbContext.Employee on e.ReportTo equals rt.ID
                          where e.ID == ID
                          select new EmployeeList
                          {
                              ID = e.ID,
                              FirstName = e.FirstName,
                              LastName = e.LastName,
                              JoiningDate = e.JoiningDate,
                              Location = e.Location,
                              DepartmentID = e.DepartmentID,
                              DesignationID = e.DesignationID,
                              Username = e.Username,
                              Gender = e.Gender,
                              Shift = e.Shift,
                              RoleId = er.RoleId,
                              Role = r.Name,
                              Email = e.Email,
                              DOB = e.DOB,
                              Phone = e.Phone,
                              ReportTo = e.ReportTo,
                              ReportToName = $"{rt.FirstName} {rt.LastName}",
                              Address = e.Address,
                              Goals = e.Goals,
                              Skills = e.Skills,
                              RefreshToken = e.RefreshToken,
                              EmployeeStatus = e.EmployeeStatus,
                              EmployeeStatusValue = ((EmployementStatus)e.EmployeeStatus).ToString(),
                              CreatedBy = e.CreatedBy,
                              UpdatedBy = e.UpdatedBy,
                              Picture = e.Picture
                          }).FirstOrDefaultAsync();
        }

    }
}
