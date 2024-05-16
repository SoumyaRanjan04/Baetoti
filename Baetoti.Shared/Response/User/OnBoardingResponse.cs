using System;
using System.Collections.Generic;

namespace Baetoti.Shared.Response.User
{
    public class OnBoardingResponse
    {
        public ProviderAndDriverOnBoardingRequest providersAndDrivers { get; set; }
    }

    public class OnBoardingUserList
    {
        public long UserID { get; set; }
        public long EmployeeId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime? RequestCloseDate { get; set; }
        public bool IsProvider { get; set; }
        public bool IsDriver { get; set; }
        public bool IsRequestClosed { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        public string OnBoardingStatus { get; set; }
        public long CountryID { get; set; }
        public string RegionID { get; set; }
        public string CityID { get; set; }

        public string CountryName { get; set; }
        public string RegionName { get; set; }
        public string CityName { get; set; }
    }

    public class UserStates
    {
        public double Pending { get; set; }
        public double Approved { get; set; }
        public double Rejected { get; set; }
    }

    public class DriverOnBoardingRequest
    {
        public UserStates userStates { get; set; }
        public List<OnBoardingUserList> pendingUserList { get; set; }
        public List<OnBoardingUserList> closeUserList { get; set; }
        public List<GraphData> graphData { get; set; }
        public DriverOnBoardingRequest()
        {
            pendingUserList = new List<OnBoardingUserList>();
            closeUserList = new List<OnBoardingUserList>();
            graphData = new List<GraphData>();
        }
    }

    public class ProviderOnBoardingRequest
    {
        public UserStates userStates { get; set; }
        public List<OnBoardingUserList> pendingUserList { get; set; }
        public List<OnBoardingUserList> closeUserList { get; set; }
        public List<GraphData> graphData { get; set; }
        public ProviderOnBoardingRequest()
        {
            pendingUserList = new List<OnBoardingUserList>();
            closeUserList = new List<OnBoardingUserList>();
            graphData = new List<GraphData>();
        }
    }
    public class ProviderAndDriverOnBoardingRequest
    {
        public UserStates userStates { get; set; }
        public List<OnBoardingUserList> pendingUserList { get; set; }
        public List<OnBoardingUserList> closeUserList { get; set; }
        public List<GraphData> graphData { get; set; }
        public ProviderAndDriverOnBoardingRequest()
        {
            pendingUserList = new List<OnBoardingUserList>();
            closeUserList = new List<OnBoardingUserList>();
            graphData = new List<GraphData>();
        }
    }

    public class GraphData
    {
        public string Date { get; set; }
        public int Drivers { get; set; }
        public int Providers { get; set; }
    }

}
