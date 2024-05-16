using Baetoti.Shared.Response.Item;
using System;
using System.Collections.Generic;
using System.Text;

namespace Baetoti.Shared.Response.User
{
    public class UserProfile
    {
        public BuyerResponse buyer { get; set; }
        public ProviderResponse provider { get; set; }
        public DriverResponse driver { get; set; }
        public WalletResponse wallet { get; set; }
        public AnalyticsResponse analytics { get; set; }
        public bool isViewUserProfile { get; set; }
        public bool isProvider { get; set; }
        public bool isDriver { get; set; }
        public bool isStoreCreated { get; set; }

        public UserProfile()
        {
            analytics = new AnalyticsResponse();
            wallet = new WalletResponse();
        }
    }

    public class BuyerResponse
    {
        public long UserID { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public string DOB { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public decimal Rating { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; }
        public int UserStatus { get; set; }
        public string Phone { get; set; }
        public string CountryCode { get; set; }
        public string PhoneWithCountryCode { get; set; }
        public string Email { get; set; }
        public string WalletAmount { get; set; }
        public int SelectedLanguage { get; set; }
        public int SelectedUserType { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsProfileCompleted { get; set; }
        public bool IsSocialLoginUsed { get; set; }
        public bool IsEmailVerified { get; set; }
        public List<BuyerHistory> buyerHistory { get; set; }
    }

    public class BuyerHistory
    {
        public int SrNo { get; set; }
        public long OrderID { get; set; }
        public long ProviderID { get; set; }
        public long ProviderUserID { get; set; }
        public long DriverID { get; set; }
        public long DriverUserID { get; set; }
        public int OrderAmount { get; set; }
        public string Provider { get; set; }
        public string Driver { get; set; }
        public string PaymentType { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
        public string DeliveryPickUp { get; set; }
        public DateTime Date { get; set; }
    }

    public class ProviderResponse
    {
        public long ProviderID { get; set; }
        public long UserID { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MaroofID { get; set; }
        public string GovernmentID { get; set; }
        public string ExpirationDate { get; set; }
        public string GovernmentIDPicture { get; set; }
        public string GovernmentIDPictureBack { get; set; }
        public string Title { get; set; }
        public string Gender { get; set; }
        public string IssueDate { get; set; }
        public string Picture { get; set; }
        public string Status { get; set; }
        public int ProviderStatus { get; set; }
        public string Phone { get; set; }
        public string CountryCode { get; set; }
        public string PhoneWithCountryCode { get; set; }
        public string Email { get; set; }
        public string Instagram { get; set; }
        public string WalletData { get; set; }
        public string Comments { get; set; }
        public string StoreName { get; set; }
        public long? StoreID { get; set; }
        public double StoreLatitude { get; set; }
        public double StoreLongitude { get; set; }
        public double OrderAcceptanceRate { get; set; }
        public string Location { get; set; }
        public string Rating { get; set; }
        public int Reviews { get; set; }
        public string Description { get; set; }
        public bool IsPublic { get; set; }
        public bool IsOnline { get; set; }
        public string FacebookURL { get; set; }
        public string InstagramURL { get; set; }
        public string TikTokURL { get; set; }
        public string TwitterURL { get; set; }
        public string RegionId { get; set; }
        public string CityId { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public List<WeekDays> weekDays { get; set; }
        public List<ItemListResponse> Items { get; set; }
        public List<ProviderOrders> Orders { get; set; }
        public List<ProviderOrders2> Orders2 { get; set; }
        public int OnBoardingStatus { get; set; }
        public string BusinessFirstName { get; set; }
        public string BussinessMiddleName { get; set; }
        public string BussinessLastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsCorpoarate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string Signature { get; set; }
        public bool IsCivilID { get; set; }
        public string CopyofIDOrPassport { get; set; }
        public string OwnerAdrress { get; set; }
        public string CommercialRegistrationLicense { get; set; }
        public string CompanyAddress { get; set; }
        public string VATRegistrationCertificate { get; set; }
        public string FreelanceCertificate { get; set; }
        public string IBANNumber { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public string BeneficiaryAddress { get; set; }
        public string SwiftCode { get; set; }
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public DateTime CommercialStartDate { get; set; }
        public string BusinessCategory { get; set; }
    }

    public class ProviderOrders
    {
        public long OrderID { get; set; }
        public string Driver { get; set; }
        public DateTime? PickUp { get; set; }
        public DateTime? Delivery { get; set; }
        public string Rating { get; set; }
        public string Review { get; set; }
    }

    public class ProviderOrders2
    {
        public long OrderID { get; set; }
        public string Buyer { get; set; }
        public string Driver { get; set; }
        public decimal OrderAmount { get; set; }
        public string PaymentType { get; set; }
        public DateTime? Date { get; set; }
        public string DeliverOrPickup { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class WeekDays
    {
        public string Day { get; set; }
        public string OpeningHours { get; set; }
        public string ClosingHours { get; set; }
    }

    public class DriverResponse
    {
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Description { get; set; }
        public string Picture { get; set; }
        public string Rating { get; set; }
        public string Status { get; set; }
        public int DriverStatus { get; set; }
        public string Comments { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string WalletData { get; set; }
        public string Gender { get; set; }
        public string SmartPhone { get; set; }
        public string DriverID { get; set; }
        public long UserID { get; set; }
        public string DrivingLiscence { get; set; }
        public string VehicleAuthorization { get; set; }
        public string MedicalCheckup { get; set; }
        public string VehicleInsurance { get; set; }
        public string GovernmentIDFrontPic { get; set; }
        public string DrivingLiscensePic { get; set; }
        public string VehicleRegistrationPic { get; set; }
        public string VehicleAuthorizationPic { get; set; }
        public string MedicalReportPic { get; set; }
        public string VehicleInsurancePic { get; set; }
        public string IdentityTypeId { get; set; }
        public string CarTypeId { get; set; }
        public string RegionId { get; set; }
        public string CityId { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string MobileNumber { get; set; }
        public string CarNumber { get; set; }
        public string VehicleSequenceNumber { get; set; }
        public bool IsOnline { get; set; }
        public bool IsAcceptJob { get; set; }
        public double LiveLatitude { get; set; }
        public double LiveLongitude { get; set; }
        public int PendingDeliveries { get; set; }
        public List<DeliveryDetail> deliveryDetails { get; set; }
        public int OnBoardingStatus { get; set; }
        public string Nationality { get; set; }
        public string DOB { get; set; }
        public string IDNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public string CarType { get; set; }
        public double OrderAcceptanceRate { get; set; }
        public string IDIssueDate { get; set; }
        public string BackSideofIDPic { get; set; }
        public string IBAN { get; set; }
        public string Title { get; set; }
    }

    public class DeliveryDetail
    {
        public long OrderID { get; set; }
        public string Price { get; set; }
        public string DeliveryFee { get; set; }
        public DateTime Date { get; set; }
        public string PickUpTime { get; set; }
        public string DropTime { get; set; }
        public string Rating { get; set; }

    }
    public class WalletResponse
    {
        public List<TransactionsHistory> transactionsHistory { get; set; }
        public WalletResponse()
        {
            transactionsHistory = new List<TransactionsHistory>();
        }
    }

    public class TransactionsHistory
    {
        public long TransactionID { get; set; }
        public long OrderID { get; set; }
        public long FromUserID { get; set; }
        public long ToUserID { get; set; }
        public int UserType { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionFrom { get; set; }
        public string TransactionTo { get; set; }
        public string TransactionTime { get; set; }
        public string PaymentType { get; set; }
        public string TransactionFor { get; set; }
        public string TransactionStatus { get; set; }
    }

    public class AnalyticsResponse
    {
        public AnalyticalData analyticalData { get; set; }
        public AnalyticalData providerAnalyticalData { get; set; }
        public AnalyticalData driverAnalyticalData { get; set; }
        public UserCancelledOrder2 userCancelledOrder { get; set; }
        public ProviderAnalytic provider { get; set; }
        public DriverAnalytic driver { get; set; }
        public OrderAnalytic order { get; set; }
        public AnalyticsResponse()
        {
            provider = new ProviderAnalytic();
            driver = new DriverAnalytic();
            order = new OrderAnalytic();
        }
    }

    public class AnalyticalData
    {
        public decimal Satisfaction { get; set; }
        public int Complaints { get; set; }
        public decimal TotalKMs { get; set; }
        public decimal HourOnRoad { get; set; }
        public string AverageCartSize { get; set; }
        public int AverageAbandantCart { get; set; }
    }

    public class UserCancelledOrder
    {
        public string TotalOrder { get; set; }
        public string CancelledOrder { get; set; }
        public string OrderDate { get; set; }
    }

    public class UserCancelledOrder2
    {
        public List<string> TotalOrder { get; set; }
        public List<string> CancelledOrder { get; set; }
        public List<string> OrderDate { get; set; }

        public UserCancelledOrder2()
        {
            TotalOrder = new List<string>();
            CancelledOrder = new List<string>();
            OrderDate = new List<string>();
        }
    }

    public class ProviderAnalytic
    {
        public OrderPrice orderPrice { get; set; }
        public CancelledOrder2 cancelledOrder { get; set; }
        public ProviderAnalytic()
        {
            orderPrice = new OrderPrice();
            cancelledOrder = new CancelledOrder2();
        }
    }

    public class OrderPrice
    {
        public List<string> TotalPrice { get; set; }
        public List<string> TotalOrder { get; set; }
        public List<string> OrderDate { get; set; }

        public OrderPrice()
        {
            TotalPrice = new List<string>();
            TotalOrder = new List<string>();
            OrderDate = new List<string>();
        }
    }

    public class OrderPrice2
    {
        public string TotalPrice { get; set; }
        public string TotalOrder { get; set; }
        public string OrderDate { get; set; }
    }

    public class CancelledOrder
    {
        public string TotalOrder { get; set; }
        public string Cancelled { get; set; }
        public string OrderDate { get; set; }
    }

    public class CancelledOrder2
    {
        public List<string> TotalOrder { get; set; }
        public List<string> Cancelled { get; set; }
        public List<string> OrderDate { get; set; }
        public CancelledOrder2()
        {
            TotalOrder = new List<string>();
            Cancelled = new List<string>();
            OrderDate = new List<string>();
        }
    }

    public class DriverAnalytic
    {
        public DeliveryTimeAccuracy deliveryTimeAccuracy { get; set; }
        public DriverCancelledOrder driverCancelledOrder { get; set; }
        public DriverAnalytic()
        {
            deliveryTimeAccuracy = new DeliveryTimeAccuracy();
            driverCancelledOrder = new DriverCancelledOrder();
        }
    }

    public class DeliveryTimeAccuracy
    {
        public List<string> Schedule { get; set; }
        public List<string> Actual { get; set; }
        public List<string> Date { get; set; }
        public DeliveryTimeAccuracy()
        {
            Schedule = new List<string>();
            Actual = new List<string>();
            Date = new List<string>();
        }
    }

    public class DeliveryTimeAccuracy2
    {
        public string Schedule { get; set; }
        public string Actual { get; set; }
        public string Date { get; set; }
    }

    public class DriverCancelledOrder
    {
        public List<string> TotalOrder { get; set; }
        public List<string> Cancelled { get; set; }
        public List<string> OrderDate { get; set; }
        public DriverCancelledOrder()
        {
            TotalOrder = new List<string>();
            Cancelled = new List<string>();
            OrderDate = new List<string>();
        }
    }

    public class DriverCancelledOrder2
    {
        public string TotalOrder { get; set; }
        public string Cancelled { get; set; }
        public string OrderDate { get; set; }
    }

    public class OrderAnalytic
    {
        public TotalAcceptedOrder totalAcceptedOrder { get; set; }
        public AverageOrderPrice averageOrderPrice { get; set; }
        public OrderTimeAccuracy orderTimeAccuracy { get; set; }
        public OrderAnalytic()
        {
            totalAcceptedOrder = new TotalAcceptedOrder();
            averageOrderPrice = new AverageOrderPrice();
            orderTimeAccuracy = new OrderTimeAccuracy();
        }
    }
    public class TotalAcceptedOrder
    {
        public List<string> TotalOrder { get; set; }
        public List<string> AcceptedOrder { get; set; }
        public List<string> OrderDate { get; set; }
        public TotalAcceptedOrder()
        {
            TotalOrder = new List<string>();
            AcceptedOrder = new List<string>();
            OrderDate = new List<string>();
        }
    }

    public class TotalAcceptedOrder2
    {
        public string TotalOrder { get; set; }
        public string AcceptedOrder { get; set; }
        public string OrderDate { get; set; }
    }

    public class AverageOrderPrice
    {
        public List<string> TotalOrder { get; set; }
        public List<string> OrderDate { get; set; }
        public AverageOrderPrice()
        {
            TotalOrder = new List<string>();
            OrderDate = new List<string>();
        }
    }

    public class AverageOrderPrice2
    {
        public string TotalOrder { get; set; }
        public string OrderDate { get; set; }
    }

    public class OrderTimeAccuracy
    {
        public List<string> Schedule { get; set; }
        public List<string> Actual { get; set; }
        public List<string> Date { get; set; }
        public OrderTimeAccuracy()
        {
            Schedule = new List<string>();
            Actual = new List<string>();
            Date = new List<string>();
        }
    }

    public class OrderTimeAccuracy2
    {
        public string Schedule { get; set; }
        public string Actual { get; set; }
        public string Date { get; set; }
    }
}
