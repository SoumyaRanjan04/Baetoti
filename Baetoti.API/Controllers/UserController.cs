using Baetoti.API.Controllers.Base;
using Baetoti.API.Helpers;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Helper;
using Baetoti.Shared.Request.Auth;
using Baetoti.Shared.Request.Email;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Request.User;
using Baetoti.Shared.Request.UserLoginHistory;
using Baetoti.Shared.Response.Email;
using Baetoti.Shared.Response.FileUpload;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.SMS;
using Baetoti.Shared.Response.User;
using DeviceDetectorNET;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
    public class UserController : ApiBaseController
    {

        private readonly IUserRepository _userRepository;
        private readonly IOTPRepository _otpRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly ISMSService _smsService;
        private readonly IUserJwtService _userJwtService;
        private readonly IUserLoginHistoryRepository _userLoginHistoryRepository;
        private readonly IHubContext<UserHub> _userHubContext;
        private readonly IEmailTemplateRepository _emailTemplateRepository;
        private readonly IEmailService _emailService;
        private readonly ISMSTemplateRepository _smsTemplateRepository;
        private readonly INotificationService _notificationService;

        public UserController(
            IUserRepository userRepository,
            IOTPRepository otpRepository,
            IProviderRepository providerRepository,
            IDriverRepository driverRepository,
            ISMSService smsService,
            IUserJwtService userJwtService,
            IUserLoginHistoryRepository userLoginHistoryRepository,
            IHubContext<UserHub> userHubContext,
            IEmailTemplateRepository emailTemplateRepository,
            IEmailService emailService,
            ISMSTemplateRepository smsTemplateRepository,
            ISiteConfigService siteConfigService,
            INotificationService notificationService
            ) : base(siteConfigService)
        {
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _providerRepository = providerRepository;
            _driverRepository = driverRepository;
            _smsService = smsService;
            _userJwtService = userJwtService;
            _userLoginHistoryRepository = userLoginHistoryRepository;
            _userHubContext = userHubContext;
            _emailTemplateRepository = emailTemplateRepository;
            _emailService = emailService;
            _notificationService = notificationService;
            _smsTemplateRepository = smsTemplateRepository;
        }

        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SignUpRequest signUpRequest)
        {
            try
            {
                string deviceInfo = string.Empty;
                string userAgent = Request.Headers["User-Agent"].ToString();
                var deviceDetector = new DeviceDetector(userAgent);
                deviceDetector.Parse();
                if (deviceDetector.IsMobile())
                {
                    deviceInfo = deviceDetector.GetOs().Matches[0].Name;
                }
                var existingUser = await _userRepository.GetByMobileNumberAsync(signUpRequest.MobileNumber);
                if (existingUser == null)
                {
                    var user = new User();
                    user.Phone = signUpRequest.MobileNumber;
                    user.CountryCode = signUpRequest.CountryCode;
                    user.PhoneWithCountryCode = signUpRequest.PhoneWithCountryCode;
                    user.UserStatus = (int)UserStatus.Active;
                    user.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    user.SelectedLanguage = (int)UserLanguage.English;
                    user.SelectedUserType = (int)UserType.Buyer;
                    user.UserDevice = deviceInfo;
                    existingUser = await _userRepository.AddAsync(user);
                    if (existingUser == null)
                        return Ok(new SharedResponse(false, 400, "Unable to SignUp. Please try later"));
                }
                else
                {
                    existingUser.CountryCode = signUpRequest.CountryCode;
                    existingUser.PhoneWithCountryCode = signUpRequest.PhoneWithCountryCode;
                    existingUser.UserDevice = deviceInfo;
                    //existingUser.MacAddress = signUpRequest.MacAddress;
                    await _userRepository.UpdateAsync(existingUser);
                }

                var _min = 1000;
                var _max = 9999;
                var _rdm = new Random();
                var number = _rdm.Next(_min, _max);
                var otp = new OTP();
                otp.UserID = existingUser.ID;
                otp.OneTimePassword = number.ToString();
                otp.OTPGeneratedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                otp.OTPStatus = (int)OTPStatus.Active;
                otp.RetryCount = 0;

                // Handle special number for apple testers.
                if (signUpRequest.MobileNumber == "+11231234123")
                {
                    otp.OneTimePassword = "1234";
                    await _otpRepository.AddAsync(otp);
                    return Ok(new SharedResponse(true, 200, "Success", otp));
                }
                await _otpRepository.AddAsync(otp);

                if (_siteConfig.IsSMSEnabled)
                {
                    SMSTemplateResponse smsTemplate = await _smsTemplateRepository.GetTemplateByType((int)SMSType.OTP);
                    string SMSResponse = await _smsService.SendSMS(signUpRequest.MobileNumber, smsTemplate.SMSText.Replace("{OTP}", otp.OneTimePassword));
                    otp.OneTimePassword = "";
                    return Ok(new SharedResponse(true, 200, SMSResponse, otp));
                }
                return Ok(new SharedResponse(true, 200, "Success", otp));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }


        [HttpPost("VerifyOTP")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPRequest otpRequest)
        {
            try
            {
                var varifiedOTP = await _otpRepository.GetByUserIdAsync(otpRequest.UserID);
                if (varifiedOTP == null)
                    return Ok(new SharedResponse(false, 400, "Unable to Verify OTP. Please Try Later."));
                if (varifiedOTP.OTPGeneratedAt < DateTime.Now.ToTimeZoneTime("Arab Standard Time").AddMinutes(-2))
                {
                    varifiedOTP.OTPStatus = (int)OTPStatus.Rejected;
                    varifiedOTP.Description = "Rejected Due to Timeout.";
                    await _otpRepository.UpdateAsync(varifiedOTP);
                    return Ok(new SharedResponse(false, 400, "OTP Expire. Please Try Later."));
                }
                if (varifiedOTP.OneTimePassword != otpRequest.OTP)
                {
                    varifiedOTP.RetryCount += 1;
                    await _otpRepository.UpdateAsync(varifiedOTP);
                    return Ok(new SharedResponse(false, 400, "Invalid OTP. Please Try Again."));
                }
                var user = await _userRepository.GetByIdAsync(otpRequest.UserID);

                //if (!string.IsNullOrEmpty(user.MacAddress))
                //{
                //    if (!user.MacAddress.Equals(otpRequest.MacAddress))
                //    {
                //        // if the user is already signed in somewhere and he is using the app.
                //        // we can sign him out of the app by sending this notification through signalR.
                //        await _notificationService.RelayUserSignout(UserId, HttpContext, _siteConfig);
                //    }
                //}

                user.MacAddress = otpRequest.MacAddress;
                varifiedOTP.OTPStatus = (int)OTPStatus.Approved;
                varifiedOTP.Description = "Approved";
                await _otpRepository.UpdateAsync(varifiedOTP);
                user.LastLogin = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                await _userRepository.UpdateAsync(user);
                var response = await _userJwtService.GenerateTokenAsync(user);
                return Ok(new SharedResponse(true, 200, "Success", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("LogActivity")]
        [AllowAnonymous]
        public async Task<IActionResult> LogActivity([FromBody] UserLoginHistoryRequest request)
        {
            try
            {
                User user = await _userRepository.GetByIdAsync(request.UserID);
                if (request.LoginStatus == (int)LoginStatus.Login)
                {
                    UserLoginHistory userLoginHistory = new UserLoginHistory()
                    {
                        UserID = request.UserID,
                        Name = $"{user.FirstName} {user.LastName}",
                        Date = Convert.ToDateTime(DateTime.Now.ToTimeZoneTime("Arab Standard Time").ToString("MMMM-dd-yyyy")),
                        LoginTime = Convert.ToDateTime(DateTime.Now.ToTimeZoneTime("Arab Standard Time").ToString("hh:mm tt")),
                        IPAddress = Helper.GetIPAddress(HttpContext)
                    };
                    await _userLoginHistoryRepository.AddAsync(userLoginHistory);
                    user.IsOnline = true;
                }
                else
                {
                    user.RefreshToken = string.Empty;
                    user.IsOnline = false;

                    UserLoginHistory userLoginHistory = await _userLoginHistoryRepository.GetByUserID(request.UserID);
                    userLoginHistory.LogoutTime = Convert.ToDateTime(DateTime.Now.ToTimeZoneTime("Arab Standard Time").ToString("hh:mm tt"));
                    await _userLoginHistoryRepository.UpdateAsync(userLoginHistory);
                }
                await _userRepository.UpdateAsync(user);

                return Ok(new SharedResponse(true, 200, "Activity Logged Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var principal = _userJwtService.GetPrincipalFromExpiredToken(request?.Token);
            if (principal == null)
            {
                throw new ArgumentException(nameof(principal));
            }
            var result = int.TryParse(principal.Identity.Name, out int userId);
            if (!result)
            {
                throw new ArgumentException(nameof(userId));
            }
            var user = await _userRepository.GetByIdAsync(userId);
            if (user.RefreshToken != request.RefreshToken)
                return BadRequest(new SharedResponse(false, 400, "Invalid Refresh token", null));
            var response = await _userJwtService.GenerateTokenAsync(user);
            return Ok(new SharedResponse(true, 200, "Success", response));
        }

        [HttpPost("CompleteProfile")]
        public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequest completeProfileRequest)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(completeProfileRequest.UserID);
                //FenceResponse isNotExisit = await _fenceRepository.GetFenceByCityID(completeProfileRequest.CityID);
                //if (isNotExisit == null)
                //    return Ok(new SharedResponse(false, 400, "Services are not available in your city."));

                if (user == null)
                {
                    return Ok(new SharedResponse(false, 400, "Unable to Find User Information. Please Try Later."));
                }
                if (user.MarkAsDeleted == false)
                {
                    user.FirstName = completeProfileRequest.FirstName;
                    user.LastName = completeProfileRequest.LastName;
                    user.DOB = completeProfileRequest.DOB;
                    user.Gender = completeProfileRequest.Gender;
                    user.Email = completeProfileRequest.Email;
                    user.UpdatedBy = Convert.ToInt32(UserId);
                    user.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    user.IsProfileCompleted = true;
                    user.Picture = completeProfileRequest.Picture;
                    user.IsSocialLoginUsed = completeProfileRequest.IsSocialLoginUsed;
                    user.CountryID = completeProfileRequest.CountryID;
                    user.RegionID = completeProfileRequest.RegionID;
                    user.CityID = completeProfileRequest.CityID;
                    await _userRepository.UpdateAsync(user);
                    return Ok(new SharedResponse(true, 200, "Profile Updated Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(true, 200, "User account is deleted"));
                }
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
                    FileUploadResponse _RESPONSE = await obj.UploadImageFile(file, "User");
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

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var usersData = await _userRepository.GetAllUsersDataAsync();
                return Ok(new SharedResponse(true, 200, "", usersData));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("ViewProfile")]
        public async Task<IActionResult> ViewProfile(long UserID)
        {
            try
            {
                var userData = await _userRepository.GetUserProfile(UserID);
                return Ok(new SharedResponse(true, 200, "", userData));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetFilteredData")]
        public async Task<IActionResult> GetFilteredData([FromBody] UserFilterRequest filterRequest)
        {
            try
            {
                var usersData = await _userRepository.GetFilteredUsersDataAsync(filterRequest);
                return Ok(new SharedResponse(true, 200, "", usersData));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("SearchUser")]
        public async Task<IActionResult> SearchUser([FromBody] UserSearchRequest request)
        {
            try
            {
                var usersData = await _userRepository.SearchUser(request);
                return Ok(new SharedResponse(true, 200, "", usersData));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("SearchFilterUser")]
        public async Task<IActionResult> SearchFilterUser([FromBody] UserFliterSearchRequest request)
        {
            try
            {
                var usersData = await _userRepository.SearchFilterUser(request);
                return Ok(new SharedResponse(true, 200, "", usersData));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("GetAllOnBoardingRequest")]
        public async Task<IActionResult> GetAllOnBoardingRequest([FromBody] OnBoardingRequest onBoardingRequest)
        {
            try
            {
                var onBoardingResponse = await _userRepository.GetOnBoardingDataAsync(onBoardingRequest);
                return Ok(new SharedResponse(true, 200, "", onBoardingResponse));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.ID);
                if (user != null)
                {
                    user.UserStatus = request.Value;
                    user.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    if (!string.IsNullOrEmpty(UserRole))
                        user.UpdatedByAdminId = Convert.ToInt32(UserId);
                    else
                        user.UpdatedBy = Convert.ToInt32(UserId);
                    await _userRepository.UpdateAsync(user);
                    return Ok(new SharedResponse(true, 200, "User Status Updated Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find User"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("UpdateOnboardingStatus")]
        public async Task<IActionResult> UpdateOnboardingStatus([FromBody] UpdateStatusRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.ID);
                if (user != null)
                {
                    user.OnBoardingStatus = request.Value;
                    user.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    user.UpdatedByAdminId = Convert.ToInt32(UserId);

                    await _userRepository.UpdateAsync(user);
                    return Ok(new SharedResponse(true, 200, "User Status Updated Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find User"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("SetLanguage")]
        public async Task<IActionResult> SetLanguage([FromBody] LanguageAndUserTypeRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserID);
                if (user != null)
                {
                    user.SelectedLanguage = request.Value;
                    user.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    user.UpdatedBy = Convert.ToInt32(UserId);
                    await _userRepository.UpdateAsync(user);
                    return Ok(new SharedResponse(true, 200, "User Language Updated Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find User"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("SetUserType")]
        public async Task<IActionResult> SetUserType([FromBody] LanguageAndUserTypeRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserID);
                if (user != null)
                {
                    user.SelectedUserType = request.Value;
                    user.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    user.UpdatedBy = Convert.ToInt32(UserId);
                    await _userRepository.UpdateAsync(user);
                    return Ok(new SharedResponse(true, 200, "User Type Updated Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find User"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("ConnectUser")]
        public async Task<IActionResult> ConnectUser(int userID)
        {
            try
            {
                Task sendTask = _userHubContext.Clients.All.SendAsync("UserConnected", userID);
                await sendTask;

                if (sendTask.IsCompletedSuccessfully)
                {
                    User user = await _userRepository.GetByIdAsync(userID);
                    user.IsOnline = true;
                    await _userRepository.UpdateAsync(user);
                    return Ok(new { UserID = userID, IsOnline = true });
                }
                else
                {
                    return StatusCode(500, new { Message = "Failed to send message to clients." });
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("DisconnectUser")]
        public async Task<IActionResult> DisconnectUser(int userID)
        {
            try
            {
                Task sendTask = _userHubContext.Clients.All.SendAsync("DisconnectUser", userID);
                await sendTask;

                if (sendTask.IsCompletedSuccessfully)
                {
                    User user = await _userRepository.GetByIdAsync(userID);
                    user.IsOnline = false;
                    await _userRepository.UpdateAsync(user);
                    return Ok(new { UserID = userID, IsOnline = false });
                }
                else
                {
                    return StatusCode(500, new { Message = "Failed to send message to clients." });
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("VerifyDeleteUserAccount")]
        public async Task<IActionResult> VerifyDeleteUserAccount(int userID)
        {
            try
            {
                User user = await _userRepository.GetByIdAsync(userID);
                if (user != null)
                {
                    var _min = 1000;
                    var _max = 9999;
                    var _rdm = new Random();
                    var number = _rdm.Next(_min, _max);
                    var otp = new OTP();
                    otp.UserID = userID;
                    otp.OneTimePassword = number.ToString();
                    otp.OTPGeneratedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    otp.OTPStatus = (int)OTPStatus.Active;
                    otp.RetryCount = 0;
                    await _otpRepository.AddAsync(otp);

                    user = await _userRepository.GetByIdAsync(long.Parse(UserId));
                    user.Email = user.Email;

                    EmailTemplateResponse emailMessage = await _emailTemplateRepository.GetTemplateByType((int)EmailType.DeleteAccount);
                    EmailRequest email = new EmailRequest()
                    {
                        Receiver = user.Email,
                        Subject = emailMessage.Subject,
                        UserID = userID.ToString(),
                        API = "/api/Email/Verify"
                    };
                    email.Body = emailMessage.HtmlBody.Replace("{OTP}", otp.OneTimePassword);
                    bool result = await _emailService.SendEmailAsync(email);
                    if (result)
                        return Ok(new SharedResponse(true, 200, "Email Sent Successfully.", otp));
                    else
                        return Ok(new SharedResponse(false, 400, "Something went wrong. Please try later."));
                }
                else
                {
                    return StatusCode(400, new { Message = "unable to delete the user." });
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 500, "unable to delete the user." + ex.Message));
            }
        }

        [HttpPost("DeleteUserAccount")]
        public async Task<IActionResult> DeleteUserAccount([FromBody] OTPRequest otpRequest)
        {
            try
            {
                var varifiedOTP = await _otpRepository.GetByUserIdAsync(otpRequest.UserID);
                if (varifiedOTP == null)
                    return Ok(new SharedResponse(false, 400, "Unable to Verify OTP. Please Try Later."));
                if (varifiedOTP.OTPGeneratedAt < DateTime.Now.ToTimeZoneTime("Arab Standard Time").AddMinutes(-2))
                {
                    varifiedOTP.OTPStatus = (int)OTPStatus.Rejected;
                    varifiedOTP.Description = "Rejected Due to Timeout.";
                    await _otpRepository.UpdateAsync(varifiedOTP);
                    return Ok(new SharedResponse(false, 400, "OTP Expire. Please Try Later."));
                }
                if (varifiedOTP.OneTimePassword != otpRequest.OTP)
                {
                    varifiedOTP.RetryCount += 1;
                    await _otpRepository.UpdateAsync(varifiedOTP);
                    return Ok(new SharedResponse(false, 400, "Invalid OTP. Please Try Again."));
                }
                varifiedOTP.OTPStatus = (int)OTPStatus.Approved;
                varifiedOTP.Description = "Approved";
                await _otpRepository.UpdateAsync(varifiedOTP);
                User user = await _userRepository.GetByIdAsync(long.Parse(UserId));
                user.MarkAsDeleted = true;
                await _userRepository.UpdateAsync(user);
                Provider provider = await _providerRepository.GetByUserID(long.Parse(UserId));
                if (provider != null)
                {
                    provider.MarkAsDeleted = true;
                    await _providerRepository.UpdateAsync(provider);
                }
                Driver driver = await _driverRepository.GetDriverByUserID(long.Parse(UserId));
                if (driver != null)
                {
                    driver.MarkAsDeleted = true;
                    await _driverRepository.UpdateAsync(driver);
                }
                return Ok(new SharedResponse(true, 200, "OTP Verified Successfully & User Account Deleted Successfully."));

            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> logout([FromBody] RequestID request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.ID);
                if (user != null)
                {
                    user.MacAddress = string.Empty;
                    await _userRepository.UpdateAsync(user);
                    return Ok(new SharedResponse(true, 200, "Logout Successfully."));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "No user found", null));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
