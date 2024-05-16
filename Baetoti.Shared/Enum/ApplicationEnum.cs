namespace Baetoti.Shared.Enum
{
    public enum AppRole
    {
        Admin = 1,
        User = 2
    }

    public enum EmployementStatus
    {
        Inactive = 0,
        Active = 1,
        Blocked = 2
    }

    public enum UserStatus
    {
        Inactive = 0,
        Active = 1
    }

    public enum OnBoardingStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public enum UserType
    {
        All = 0,
        Buyer = 1,
        Provider = 2,
        Driver = 3,
        Admin = 4
    }


    public enum ProviderBusinessType
    {
        Corporate = 0,
        Individual = 1,
    }
    public enum ProviderBusinessCardType
    {
        CivilID = 0,
        Passport = 1,
    }

    public enum ProviderIDType
    {
        GovtID = 0,
        Passport = 1
    }

    public enum ProviderStatus
    {
        Inactive = 0,
        Active = 1,
        Pending = 2,
        Approved = 3,
        Rejected = 4
    }

    public enum DriverStatus
    {
        Inactive = 0,
        Active = 1,
        Pending = 2,
        Approved = 3,
        Rejected = 4
    }

    public enum OTPStatus
    {
        Inactive = 0,
        Active = 1,
        Approved = 2,
        Rejected = 3
    }

    public enum ItemStatus
    {
        Inactive = 0,
        Active = 1,
        Pending = 2,
        Approved = 3,
        Rejected = 4
    }

    public enum ItemRequestType
    {
        Onboarding = 1,
        Change = 2
    }

    public enum OrderStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        InProgress = 3,
        Ready = 4,
        PickedUp = 5,
        Delivered = 6,
        Complaint = 7,
        CancelledByCustomer = 8,
        CancelledByDriver = 9,
        CancelledByProvider = 10,
        Completed = 11,
        Late = 12,
        CancelledBySystem = 13
    }

    public enum TrackOrderStatus
    {
        Active = 1, // Include OrderStatus 1,3,4,5,6
        Completed = 2, // Include OrderStatus 11
        WaitingForRating = 3, // Include OrderStatus 11, 6
        Cancelled = 4 // Include OrderStatus 8,9,10
    }

    public enum ProviderOrderStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Cancelled = 3
    }

    public enum SortOrder
    {
        DeliveryDate = 1,
        OrderDate = 2
    }

    public enum ProviderOnlineStatus
    {
        All = 0,
        Online = 1,
        Offline = 2
    }

    public enum DriverOnlineStatus
    {
        All = 0,
        Online = 1,
        Offline = 2
    }

    public enum DriverOrderStatus
    {
        Pending = 0,
        PickedUp = 1,
        Delivered = 2,
        Cancelled = 4
    }

    public enum TransactionStatus
    {
        UnPaid = 0,
        Paid = 1,
        Declined = 2
    }

    public enum TransactionType
    {
        Cash = 1,
        Online = 2
    }

    public enum StoreStatus
    {
        Private = 0,
        Public = 1
    }

    public enum RatingFilter
    {
        All = 0,
        Positive = 1,
        Negative = 2,
        Recent = 4
    }

    public enum FenceStatus
    {
        Inactive = 0,
        Active = 1,
        Stopped = 2
    }

    public enum DBSchema
    {
        baetoti,
        audit
    }

    public enum CategoryStatus
    {
        Active = 1,
        Inactive = 2
    }

    public enum SubCategoryStatus
    {
        Active = 1,
        Inactive = 2
    }

    public enum UnitStatus
    {
        Active = 1,
        Inactive = 2
    }

    public enum CountryStatus
    {
        Active = 1,
        Inactive = 2
    }

    public enum TagStatus
    {
        Active = 1,
        Inactive = 2
    }

    public enum InvoiceType
    {
        User = 1,
        Driver = 2,
        Provider = 3,
        PickUp = 4,
        Self = 5,
        Baetoti = 6
    }

    public enum PromotionType
    {
        ForAllUsers = 1,
        FirstTimeUsers = 2
    }

    public enum PromoCodeDiscountType
    {
        Percentage = 1,
        Value = 2
    }

    public enum CommisionType
    {
        Percentage = 1,
        Value = 2
    }

    public enum PromotionStatus
    {
        Active = 1, // Esisting , ReOpen
        Inactive = 2 // Expire
    }

    public enum ItemFilter
    {
        AvailableNow = 1,
        Featured = 2,
        NearMe = 3,
        TopRated = 4,
        Latest = 5
    }

    public enum TagType
    {
        Item = 1,
        Store = 2
    }

    public enum OrderRequestStatus
    {
        NotInitiated = 0,
        Pending = 1,
        Accepted = 2,
        Rejected = 3,
        PickedUp = 4,
        Cancelled = 5,
        RideStarted = 6,
        ArrivedAtStore = 7,
        ArrivedAtDestination = 8,
        Completed = 9
    }

    public enum SupportRequestType
    {
        Suggestion = 1,
        SafetyConcern = 2,
        ReportedOrder = 3
    }

    public enum SupportRequestStatus
    {
        Pending = 0,
        Open = 1,
        Close = 2
    }

    public enum ReportedOrderStatus
    {
        Current = 1,
        Past = 2
    }

    public enum UserLanguage
    {
        English = 1,
        Arabic = 2
    }

    public enum NotificationType
    {
        NewChat = 1,
        NewMessage = 2,
        NewOrderForProvider = 3,
        AcceptOrderByProvider = 4,
        RejectOrderByProvider = 5,
        CancelOrderByBuyer = 6,
        CancelOrderByProvider = 7,
        SetOrderInprogress = 8,
        SetOrderReady = 9,
        SetOrderDelivered = 10,
        CancelledByBuyer = 11,
        CancelledByProvider = 12,
        CompleteOrderByBuyer = 13,
        DriverRatedByBuyer = 14,
        ProviderRatedByBuyer = 15,
        BuyerRatedByProvider = 16,
        DriverRatedByProvider = 17,
        ProviderRatedByDriver = 18,
        BuyerRatedByDriver = 19,
        DriverDeliveryRequest = 20,
        DriverAcceptDeliveryRequest = 21,
        DriverIgnoreDeliveryRequest = 22,
        DriverCancelDeliveryRequest = 23,
        DriverArrivePickup = 24,
        DriverPickup = 25,
        DriverArivedDeliveryPoint = 26,
        DriverOrderCompleted = 27,
        CaptainOnboardingApproved = 28,
        CaptainOnboardingRejected = 29,
        ProviderOnboardingApproved = 30,
        ProviderOnboardingRejected = 31,
        ItemChangeRequestApproved = 32,
        ItemChangeRequestRejected = 33,
        StoreApproved = 34,
    }

    public enum SaleGraphFilter
    {
        ByWeek = 1,
        ByMonth = 2,
        ByYear = 3,
    }

    public enum EarnGraphFilter
    {
        ByMonth = 1,
        All = 2,
    }

    public enum CSSGraphFilter
    {
        All = 1,
        AverageCustomerSatisfaction = 2,
        AverageResponseTime = 3,
        AverageResolutionTime = 4,
    }

    public enum LoginStatus
    {
        Login = 1,
        Logout = 2
    }

    public enum RevenuPerDayFilter
    {
        Daily = 1,
        Monthly = 2,
        Yearly = 3,
    }

    public enum CohortFilterType
    {
        Weekly = 1,
        Yearly = 2
    }

    public enum EmailType
    {
        OTP = 1,
        EmailVerification = 2,
        DeleteAccount = 3
    }

    public enum SMSType
    {
        OTP = 1
    }

    public enum ContractType
    {
        Driver = 1,
        ProviderIndividual = 2,
        ProviderCorporate = 3
    }

}
