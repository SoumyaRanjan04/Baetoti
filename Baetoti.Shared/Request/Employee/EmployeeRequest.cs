using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Request.Employee
{
    public class EmployeeRequest
    {
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Picture { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string Location { get; set; }
        public int DepartmentID { get; set; }
        public int DesignationID { get; set; }
        public string Username { get; set; }
        public string Gender { get; set; }
        public string Shift { get; set; }
        public string Email { get; set; }
        public DateTime? DOB { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int RoleID { get; set; }
        public int ReportTo { get; set; }
        public string Address { get; set; }
        public string Goals { get; set; }
        public string Skills { get; set; }
    }
}
