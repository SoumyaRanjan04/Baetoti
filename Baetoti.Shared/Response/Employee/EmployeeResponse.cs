using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.Employee
{
    public class EmployeeResponse
    {
        public int Online { get; set; }
        public int Offline { get; set; }
        public List<EmployeeList> employeeList { get; set; }
        public EmployeeResponse()
        {
            employeeList = new List<EmployeeList>();
        }
    }

    public class EmployeeList
    {
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string Location { get; set; }
        public int DepartmentID { get; set; }
        public int DesignationID { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public string Shift { get; set; }
        public int RoleId { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
        public DateTime? DOB { get; set; }
        public string Phone { get; set; }
        public int ReportTo { get; set; }
        public string ReportToName { get; set; }
        public string Address { get; set; }
        public string Goals { get; set; }
        public string Skills { get; set; }
        public string RefreshToken { get; set; }
        public int EmployeeStatus { get; set; }
        public string EmployeeStatusValue { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Picture { get; set; }
    }

}
