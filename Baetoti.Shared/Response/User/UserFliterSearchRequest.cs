using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.User
{

    public class UserFliterSearchRequest
    {
        public int? CountryID { get; set; }
        public string CityID { get; set; }
        public string RegionID { get; set; }
        public int UserType { get; set; } = 0;
        public string Name { get; set; }
        public string Gender { get; set; }
    }
}
