using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.API.Helpers;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Request.Notification;
using Baetoti.Shared.Request.Provider;
using Baetoti.Shared.Response.Fence;
using Baetoti.Shared.Response.FileUpload;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using NotificationType = Baetoti.Shared.Enum.NotificationType;
using Provider = Baetoti.Core.Entites.Provider;
using System.Linq;
using Baetoti.Shared.Request.Driver;
using Baetoti.Shared.Response.Dashboard;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;

namespace Baetoti.API.Controllers
{
    public class ProviderController : ApiBaseController
    {

        public readonly IProviderRepository _providerRepository;
        public readonly IProviderBusinessRepository _providerBusinessRepository;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPushNotificationRepository _pushNotificationRepository;
        private readonly IFenceRepository _fenceRepository;
        private readonly IStoreRepository _storeRepository;
        public readonly IMapper _mapper;

        public ProviderController(
            IProviderRepository providerRepository,
            IProviderBusinessRepository providerBusinessRepository,
            ISiteConfigService siteConfigService,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,
            IPushNotificationRepository pushNotificationRepository,
            IFenceRepository fenceRepository,
            IStoreRepository storeRepository,
            IMapper mapper
            ) : base(siteConfigService)
        {
            _providerRepository = providerRepository;
            _providerBusinessRepository = providerBusinessRepository;
            _httpContextAccessor = httpContextAccessor;
            _notificationService = notificationService;
            _pushNotificationRepository = pushNotificationRepository;
            _fenceRepository = fenceRepository;
            _storeRepository = storeRepository;
            _mapper = mapper;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] ProviderRequest providerRequest)
        {
            try
            {
                var checkIfProviderAlreadyExist = await _providerRepository.GetByUserID(providerRequest.UserID);
                if (checkIfProviderAlreadyExist != null)
                    return Ok(new SharedResponse(false, 400, "User already exist in provider."));
                FenceResponse isNotExisit = await _fenceRepository.GetFenceByCityID(providerRequest.CityId);
                if (isNotExisit == null)
                    return Ok(new SharedResponse(false, 400, "Services are not available in your city."));

                if (!string.IsNullOrEmpty(providerRequest.GovernmentID))
                {
                    Provider alreadyExist = await _providerRepository.GetByGovtID(providerRequest.GovernmentID);
                    if (alreadyExist != null)
                        return Ok(new SharedResponse(false, 400, "Another provider with same govt id already exists."));
                }

                var provider = _mapper.Map<Provider>(providerRequest);
                if (!string.IsNullOrEmpty(providerRequest.Gender))
                    provider.Title = providerRequest.Gender == "Male" ? "Mr" : "Ms";
                provider.ProviderStatus = (int)ProviderStatus.Pending;
                provider.OnBoardingStatus = (int)OnBoardingStatus.Pending;
                provider.MarkAsDeleted = false;
                provider.IsOnline = true;
                provider.IsPublic = false;
                provider.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                provider.CreatedBy = Convert.ToInt32(UserId);

                var result = await _providerRepository.AddAsync(provider);
                if (result == null)
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Submit Provider Request"));
                }
                else
                {
                    ProviderBusiness providerBusiness = new ProviderBusiness();
                    providerBusiness.ProviderID = result.ID;
                    providerBusiness.FirstName = providerRequest.FirstName;
                    providerBusiness.MiddleName = providerRequest.MiddleName;
                    providerBusiness.LastName = providerRequest.LastName;
                    providerBusiness.PhoneNumber = providerRequest.PhoneNumber;
                    providerBusiness.IsCorpoarate = providerRequest.IsCorpoarate;
                    providerBusiness.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    providerBusiness.LastUpdatedAt = providerRequest.LastUpdatedAt;
                    providerBusiness.Signature = providerRequest.Signature;
                    providerBusiness.IsCivilID = providerRequest.IsCivilID;
                    providerBusiness.CopyofIDOrPassport = providerRequest.CopyofIDOrPassport;
                    providerBusiness.OwnerAdrress = providerRequest.OwnerAdrress;
                    providerBusiness.CommercialRegistrationLicense = providerRequest.CommercialRegistrationLicense;
                    providerBusiness.CompanyAddress = providerRequest.CompanyAddress;
                    providerBusiness.VATRegistrationCertificate = providerRequest.VATRegistrationCertificate;
                    providerBusiness.FreelanceCertificate = providerRequest.FreelanceCertificate;
                    providerBusiness.IBANNumber = providerRequest.IBANNumber;
                    providerBusiness.BankAccountName = providerRequest.BankAccountName;
                    providerBusiness.BankAccountNumber = providerRequest.BankAccountNumber;
                    providerBusiness.BeneficiaryAddress = providerRequest.BeneficiaryAddress;
                    providerBusiness.SwiftCode = providerRequest.SwiftCode;
                    providerBusiness.CommercialExpiry = providerRequest.CommercialExpiry;
                    providerBusiness.CommercialNumber = providerRequest.CommercialNumber;
                    providerBusiness.VATRegistrationNumber = providerRequest.VATRegistrationNumber;
                    providerBusiness.BankAccountCertificate = providerRequest.BankAccountCertificate;
                    providerBusiness.CompanyName = providerRequest.CompanyName;
                    providerBusiness.Position = providerRequest.Position;
                    providerBusiness.BusinessCategory = providerRequest.BusinessCategory;
                    providerBusiness.TapCompanyStoreName = $"{providerRequest.FirstName}{providerRequest.LastName}{providerRequest.UserID}";
                    providerBusiness.CommercialStartDate = providerRequest.CommercialStartDate;
                    var resultPB = await _providerBusinessRepository.AddAsync(providerBusiness);
                    if (resultPB == null)
                    {
                        return Ok(new SharedResponse(true, 200, "Provider Request Submitted Successfully but ProviderBussiness couldnot save", result));
                    }
                    else
                    {
                        var providerDetail = await _providerRepository.GetProviderBusinessDetailByUserID(result.UserID);
                        return Ok(new SharedResponse(true, 200, "Provider and ProviderBussiness Request Submitted Successfully", providerDetail));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] ProviderRequest providerRequest)
        {
            try
            {
                Provider alreadyExist = await _providerRepository.GetByGovtID(providerRequest.GovernmentID);
                if (alreadyExist != null && alreadyExist.UserID != providerRequest.UserID)
                    return Ok(new SharedResponse(false, 400, "Another provider with same govt id already exists."));

                Provider provider = await _providerRepository.GetByUserID(providerRequest.UserID);
                provider.GovernmentID = providerRequest.GovernmentID;
                provider.CountryId = providerRequest.CountryId;
                provider.RegionId = providerRequest.RegionId;
                provider.CityId = providerRequest.CityId;
                provider.GovernmentIDPicture = providerRequest.GovernmentIDPicture;
                provider.GovernmentIDPictureBack = providerRequest.GovernmentIDPictureBack;
                provider.ExpirationDate = providerRequest.ExpirationDate;
                provider.IssueDate = providerRequest.IssueDate;
                provider.ProviderStatus = providerRequest.ProviderStatus;
                if (!string.IsNullOrEmpty(providerRequest.Gender))
                    provider.Title = providerRequest.Gender == "Male" ? "Mr" : "Ms";
                provider.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                provider.UpdatedBy = Convert.ToInt32(UserId);
                await _providerRepository.UpdateAsync(provider);
                ProviderBusiness providerBusiness = await _providerBusinessRepository.GetByProviderID(providerRequest.ProviderID);
                if (providerBusiness != null)
                {
                    providerBusiness.ProviderID = providerRequest.ProviderID;
                    providerBusiness.FirstName = providerRequest.FirstName;
                    providerBusiness.MiddleName = providerRequest.MiddleName;
                    providerBusiness.LastName = providerRequest.LastName;
                    providerBusiness.PhoneNumber = providerRequest.PhoneNumber;
                    providerBusiness.IsCorpoarate = providerRequest.IsCorpoarate;
                    providerBusiness.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    providerBusiness.Signature = providerRequest.Signature;
                    providerBusiness.IsCivilID = providerRequest.IsCivilID;
                    providerBusiness.CopyofIDOrPassport = providerRequest.CopyofIDOrPassport;
                    providerBusiness.OwnerAdrress = providerRequest.OwnerAdrress;
                    providerBusiness.CommercialRegistrationLicense = providerRequest.CommercialRegistrationLicense;
                    providerBusiness.CompanyAddress = providerRequest.CompanyAddress;
                    providerBusiness.VATRegistrationCertificate = providerRequest.VATRegistrationCertificate;
                    providerBusiness.FreelanceCertificate = providerRequest.FreelanceCertificate;
                    providerBusiness.IBANNumber = providerRequest.IBANNumber;
                    providerBusiness.BankAccountName = providerRequest.BankAccountName;
                    providerBusiness.BankAccountNumber = providerRequest.BankAccountNumber;
                    providerBusiness.BeneficiaryAddress = providerRequest.BeneficiaryAddress;
                    providerBusiness.SwiftCode = providerRequest.SwiftCode;
                    providerBusiness.CommercialNumber = providerRequest.CommercialNumber;
                    providerBusiness.CommercialExpiry = providerRequest.CommercialExpiry;
                    providerBusiness.VATRegistrationNumber = providerRequest.VATRegistrationNumber;
                    providerBusiness.BankAccountCertificate = providerRequest.BankAccountCertificate;
                    providerBusiness.CompanyName = providerRequest.CompanyName;
                    providerBusiness.Position = providerRequest.Position;
                    providerBusiness.BusinessCategory = providerRequest.BusinessCategory;
                    providerBusiness.CommercialStartDate = providerRequest.CommercialStartDate;
                    return Ok(new SharedResponse(true, 200, "Provider and ProviderBusiness Information Updated Successfully", provider));
                }
                else
                {
                    providerBusiness = new ProviderBusiness();
                    providerBusiness.ProviderID = providerRequest.ProviderID;
                    providerBusiness.FirstName = providerRequest.FirstName;
                    providerBusiness.MiddleName = providerRequest.MiddleName;
                    providerBusiness.LastName = providerRequest.LastName;
                    providerBusiness.PhoneNumber = providerRequest.PhoneNumber;
                    providerBusiness.IsCorpoarate = providerRequest.IsCorpoarate;
                    providerBusiness.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    providerBusiness.Signature = providerRequest.Signature;
                    providerBusiness.IsCivilID = providerRequest.IsCivilID;
                    providerBusiness.CopyofIDOrPassport = providerRequest.CopyofIDOrPassport;
                    providerBusiness.OwnerAdrress = providerRequest.OwnerAdrress;
                    providerBusiness.CommercialRegistrationLicense = providerRequest.CommercialRegistrationLicense;
                    providerBusiness.CompanyAddress = providerRequest.CompanyAddress;
                    providerBusiness.VATRegistrationCertificate = providerRequest.VATRegistrationCertificate;
                    providerBusiness.FreelanceCertificate = providerRequest.FreelanceCertificate;
                    providerBusiness.IBANNumber = providerRequest.IBANNumber;
                    providerBusiness.BankAccountName = providerRequest.BankAccountName;
                    providerBusiness.BankAccountNumber = providerRequest.BankAccountNumber;
                    providerBusiness.BeneficiaryAddress = providerRequest.BeneficiaryAddress;
                    providerBusiness.SwiftCode = providerRequest.SwiftCode;
                    providerBusiness.CommercialNumber = providerRequest.CommercialNumber;
                    providerBusiness.CommercialExpiry = providerRequest.CommercialExpiry;
                    providerBusiness.VATRegistrationNumber = providerRequest.VATRegistrationNumber;
                    providerBusiness.BankAccountCertificate = providerRequest.BankAccountCertificate;
                    providerBusiness.CompanyName = providerRequest.CompanyName;
                    providerBusiness.Position = providerRequest.Position;
                    providerBusiness.CommercialStartDate = providerRequest.CommercialStartDate;
                    var resultPB = await _providerBusinessRepository.AddAsync(providerBusiness);
                    if (resultPB == null)
                    {
                        return Ok(new SharedResponse(true, 200, "Provider Request Update Successfully but ProviderBussiness couldnot save", resultPB));
                    }
                    else
                    {
                        return Ok(new SharedResponse(true, 200, "Provider Request Update Successfully and Add ProviderBussiness  Successfully", resultPB));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpGet("GetByProviderID")]
        public async Task<IActionResult> GetByProviderID(long Id)
        {
            try
            {
                var provider = await _providerRepository.GetByUserID(Id);
                return Ok(new SharedResponse(true, 200, "", provider));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetByUserID")]
        public async Task<IActionResult> GetByUserID(long Id)
        {
            try
            {
                var provider = await _providerRepository.GetDetailByUserID(Id);
                return Ok(new SharedResponse(true, 200, "", provider));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetProviderBusinessDetailByUserID")]
        public async Task<IActionResult> GetProviderBusinessDetailByUserID(long Id)
        {
            try
            {
                var provider = await _providerRepository.GetProviderBusinessDetailByUserID(Id);
                return Ok(new SharedResponse(true, 200, "", provider));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetProviderBusinessDetailByProviderID")]
        public async Task<IActionResult> GetProviderBusinessDetailByProviderID(long Id)
        {
            try
            {
                var provider = await _providerBusinessRepository.GetByProviderID(Id);
                return Ok(new SharedResponse(true, 200, "", provider));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetDashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var providerDashboard = await _providerRepository.GetDashboardData(Convert.ToInt64(UserId));
                return Ok(new SharedResponse(true, 200, "", providerDashboard));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var providers = await _providerRepository.GetAllAsync();
                return Ok(new SharedResponse(true, 200, "", providers));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetOnline")]
        public async Task<IActionResult> GetOnline(int PageNumber = 1, int PageSize = 10)
        {
            try
            {
                var providers = await _providerRepository.GetAllOnlineAsync(PageNumber, PageSize);
                return Ok(new SharedResponse(true, 200, "", providers));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetSalesGraphData")]
        public async Task<IActionResult> GetSalesGraphData(int SaleGraphFilter)
        {
            try
            {
                var response = await _providerRepository.GetSalesGraphData(SaleGraphFilter, long.Parse(UserId));
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpGet("GetEarnGraphData")]
        public async Task<IActionResult> GetEarnGraphData(int EarnGraphFilter)
        {
            try
            {
                var response = await _providerRepository.GetEarnGraphData(EarnGraphFilter, long.Parse(UserId));
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] ProviderApprovalRequest providerRequest)
        {
            try
            {
                var provider = await _providerRepository.GetByUserID(providerRequest.UserID);
                provider.ProviderStatus = providerRequest.StatusValue;
                provider.Comments = providerRequest.Comments;
                provider.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                provider.UpdatedBy = Convert.ToInt32(UserId);
                await _providerRepository.UpdateAsync(provider);

                // Notification

                string DestinationUserID = "";
                int DestinationUserType = 0;
                int NotificationTypeID = 0;
                string SenderUser = "";
                int SenderUserType = 0;
                string SubjectID = "";

                if (provider.ProviderStatus == (int)ProviderStatus.Approved)
                {
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.ProviderOnboardingApproved;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Admin;
                    SubjectID = "";
                }
                else if (provider.ProviderStatus == (int)ProviderStatus.Rejected)
                {
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.ProviderOnboardingRejected;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Admin;
                    SubjectID = "";
                }

                if (NotificationTypeID > 0)
                {
                    var notification = await _pushNotificationRepository.GetByNotificationType(NotificationTypeID);
                    CreateNotificationRequest createNotificationRequest = new CreateNotificationRequest()
                    {
                        DestUserID = DestinationUserID,
                        Type = notification.NotificationTypeID,
                        DestUserType = DestinationUserType,
                        NotificationTitle = notification.Title,
                        NotificationBody = notification.Text,
                        SenderUser = SenderUser,
                        SenderUserType = SenderUserType,
                        SubjectID = SubjectID
                    };
                    await _notificationService.CreatePushNotification(createNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);
                }

                return Ok(new SharedResponse(true, 200, "Provider Request Processed Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("UpdateOnBoardingStatus")]
        public async Task<IActionResult> UpdateOnBoardingStatus([FromBody] ProviderApprovalRequest providerRequest)
        {
            try
            {
                var provider = await _providerRepository.GetByUserID(providerRequest.UserID);
                provider.OnBoardingStatus = providerRequest.StatusValue;
                provider.Comments = providerRequest.Comments;
                if (providerRequest.StatusValue == (int)OnBoardingStatus.Approved)
                    provider.ProviderStatus = (int)ProviderStatus.Active;
                provider.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                provider.UpdatedByAdminId = Convert.ToInt32(UserId);
                await _providerRepository.UpdateAsync(provider);

                // Notification

                string DestinationUserID = "";
                int DestinationUserType = 0;
                int NotificationTypeID = 0;
                string SenderUser = "";
                int SenderUserType = 0;
                string SubjectID = "";

                if (provider.OnBoardingStatus == (int)OnBoardingStatus.Approved)
                {
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.StoreApproved;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Admin;
                    SubjectID = "";

                    var storelatlngData = await _storeRepository.GetByProviderId(provider.UserID);
                    if (storelatlngData != null)
                    {
                        var sLat = storelatlngData.Latitude;
                        var slng = storelatlngData.Longitude;
                        var getListOfUserWithIn10km = await _providerRepository.GetUserlatlng(sLat, slng);

                        var notification = await _pushNotificationRepository.GetByNotificationType(NotificationTypeID);
                        ServiceNotificationRequest serviceNotificationRequest = new ServiceNotificationRequest
                        {
                            NotificationTitle = notification.Title,
                            NotificationBody = notification.Text,
                            UserIDs = getListOfUserWithIn10km.Select(x => x.UserID).ToList(),
                            UserTypes = getListOfUserWithIn10km.Select(x => x.UserType).ToList(),

                        };
                        await _notificationService.CreateServiceNotification(serviceNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);
                    }
                    else
                    {

                    }
                }
                else if (provider.OnBoardingStatus == (int)OnBoardingStatus.Rejected)
                {
                    DestinationUserID = provider.UserID.ToString();
                    DestinationUserType = (int)UserType.Provider;
                    NotificationTypeID = (int)NotificationType.ProviderOnboardingRejected;
                    SenderUser = UserId;
                    SenderUserType = (int)UserType.Admin;
                    SubjectID = "";
                }

                if (NotificationTypeID > 0)
                {
                    var notification = await _pushNotificationRepository.GetByNotificationType(NotificationTypeID);
                    CreateNotificationRequest createNotificationRequest = new CreateNotificationRequest()
                    {
                        DestUserID = DestinationUserID,
                        Type = notification.NotificationTypeID,
                        DestUserType = DestinationUserType,
                        NotificationTitle = notification.Title,
                        NotificationBody = notification.Text,
                        SenderUser = SenderUser,
                        SenderUserType = SenderUserType,
                        SubjectID = SubjectID
                    };
                    await _notificationService.CreatePushNotification(createNotificationRequest, _httpContextAccessor.HttpContext, _siteConfig);
                }

                return Ok(new SharedResponse(true, 200, "Provider Request Processed Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("SetOnline")]
        public async Task<IActionResult> SetOnline([FromBody] StatusChangeRequest request)
        {
            try
            {
                var provider = await _providerRepository.GetByUserID(request.UserID);
                provider.IsOnline = request.StatusValue;
                provider.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                provider.UpdatedBy = Convert.ToInt32(UserId);
                await _providerRepository.UpdateAsync(provider);
                return Ok(new SharedResponse(true, 200, "Provider Request Processed Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("SetPublic")]
        public async Task<IActionResult> SetPublic([FromBody] StatusChangeRequest request)
        {
            try
            {
                var provider = await _providerRepository.GetByUserID(request.UserID);
                provider.IsPublic = request.StatusValue;
                provider.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                provider.UpdatedBy = Convert.ToInt32(UserId);
                await _providerRepository.UpdateAsync(provider);
                return Ok(new SharedResponse(true, 200, "Provider Request Processed Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    UploadImage obj = new UploadImage();
                    FileUploadResponse _RESPONSE = await obj.UploadImageFile(file, "Provider");
                    if (string.IsNullOrEmpty(_RESPONSE.Message))
                    {
                        return Ok(new SharedResponse(true, 200, "File uploaded successfully!", _RESPONSE));
                    }
                    else
                    {
                        return Ok(new SharedResponse(true, 400, _RESPONSE.Message));
                    }
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "File is required!"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

    }
}
