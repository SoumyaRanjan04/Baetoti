using AutoMapper;
using Baetoti.API.Controllers.Base;
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
using Baetoti.Shared.Response.Email;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
    public class AuthController : ApiBaseController
    {

        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmployeeLoginHistoryRepository _employeeLoginHistoryRepository;
        private readonly IJwtService _jwtService;
        private readonly IMapper _mapper;
        private readonly IRijndaelEncryptionService _hashingService;
        private readonly IRolePrivilegeRepository _rolePrivilegeRepository;
        private readonly IEmployeeRoleRepository _employeeRoleRepository;
        private readonly IOTPAdminRepository _otpAdminRepository;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateRepository _emailTemplateRepository;

        public AuthController(
            IEmployeeRepository employeeRepository,
            IEmployeeLoginHistoryRepository employeeLoginHistoryRepository,
            IJwtService jwtService,
            IMapper mapper,
            IRijndaelEncryptionService hashingService,
            IRolePrivilegeRepository rolePrivilegeRepository,
            IEmployeeRoleRepository employeeRoleRepository,
            IOTPAdminRepository oTPAdminRepository,
            IEmailService emailService,
            IEmailTemplateRepository emailTemplateRepository
            )
        {
            _employeeRepository = employeeRepository;
            _employeeLoginHistoryRepository = employeeLoginHistoryRepository;
            _jwtService = jwtService;
            _mapper = mapper;
            _hashingService = hashingService;
            _rolePrivilegeRepository = rolePrivilegeRepository;
            _employeeRoleRepository = employeeRoleRepository;
            _otpAdminRepository = oTPAdminRepository;
            _emailService = emailService;
            _emailTemplateRepository = emailTemplateRepository;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthRequest request)
        {
            try
            {
                var employee = await _employeeRepository.AuthenticateUser(_mapper.Map<Employee>(request));
                if (employee != null)
                {
                    var decryptedPassword = _hashingService.DecryptPassword(employee.Password, employee.Salt);
                    if (decryptedPassword == request.Password)
                    {
                        var _min = 1000;
                        var _max = 9999;
                        var _rdm = new Random();
                        var number = _rdm.Next(_min, _max);
                        var otp = new OTPAdmin();
                        otp.EmployeeID = employee.ID;
                        otp.OneTimePassword = number.ToString();
                        otp.OTPGeneratedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                        otp.OTPStatus = (int)OTPStatus.Active;
                        otp.RetryCount = 0;
                        await _otpAdminRepository.AddAsync(otp);

                        EmailTemplateResponse emailMessage = await _emailTemplateRepository.GetTemplateByType((int)EmailType.OTP);
                        EmailRequest email = new EmailRequest()
                        {
                            Receiver = employee.Email,
                            Subject = emailMessage.Subject,
                            UserID = employee.ID.ToString(),
                            API = "/api/Email/Verify"
                        };
                        email.Body = emailMessage.HtmlBody.Replace("{OTP}", otp.OneTimePassword);
                        bool result = await _emailService.SendEmailAsync(email);
                        if (result)
                        {
                            otp.OneTimePassword = "";
                            return Ok(new SharedResponse(true, 200, "Email Sent Successfully.", otp));
                        }
                        else
                            return Ok(new SharedResponse(false, 400, "Something went wrong. Please try later."));
                    }
                    return Ok(new SharedResponse(false, 400, "Invalid Password."));
                }
                return Ok(new SharedResponse(false, 400, "Invalid Username."));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("VerifyOTP")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOTP([FromBody] OTPRequest otpRequest)
        {
            try
            {
                var varifiedOTP = await _otpAdminRepository.GetByEmployeeIdAsync(otpRequest.UserID);
                if (varifiedOTP == null)
                    return Ok(new SharedResponse(false, 400, "Unable to Verify OTP. Please Try Later."));
                if (varifiedOTP.OTPGeneratedAt < DateTime.Now.ToTimeZoneTime("Arab Standard Time").AddMinutes(-2))
                {
                    varifiedOTP.OTPStatus = (int)OTPStatus.Rejected;
                    varifiedOTP.Description = "Rejected Due to Timeout.";
                    await _otpAdminRepository.UpdateAsync(varifiedOTP);
                    return Ok(new SharedResponse(false, 400, "OTP Expire. Please Try Later."));
                }
                if (varifiedOTP.OneTimePassword != otpRequest.OTP)
                {
                    varifiedOTP.RetryCount += 1;
                    await _otpAdminRepository.UpdateAsync(varifiedOTP);
                    return Ok(new SharedResponse(false, 400, "Invalid OTP. Please Try Again."));
                }
                varifiedOTP.OTPStatus = (int)OTPStatus.Approved;
                varifiedOTP.Description = "Approved";
                await _otpAdminRepository.UpdateAsync(varifiedOTP);

                var employee = await _employeeRepository.GetByIdAsync(otpRequest.UserID);
                employee.LastLogin = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                await _employeeRepository.UpdateAsync(employee);
                var response = await _jwtService.GenerateTokenAsync(employee);

                EmployeeLoginHistory employeeLoginHistory = new EmployeeLoginHistory()
                {
                    EmployeeID = employee.ID,
                    Employee = $"{employee.FirstName} {employee.LastName}",
                    Date = DateTime.Now.ToTimeZoneTime("Arab Standard Time").ToString("MMMM-dd-yyyy"),
                    LoginTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time").ToString("hh:mm tt"),
                    IPAddress = Helper.GetIPAddress(HttpContext)
                };
                await _employeeLoginHistoryRepository.AddAsync(employeeLoginHistory);
                //Task.Run(async () => await _employeeLoginHistoryRepository.AddAsync(employeeLoginHistory));

                var role = await _employeeRoleRepository.GetByUserId(employee.ID);
                if (role != null)
                {
                    var menu = await _rolePrivilegeRepository.GetRoleWithPrivileges(role.RoleId);
                    if (menu != null)
                    {
                        return Ok(new SharedResponse(true, 200, "Success", response, menu));
                    }
                }
                return Ok(new SharedResponse(true, 200, "Success", response));
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
                var employee = await _employeeRepository.GetByIdAsync(request.ID);
                employee.RefreshToken = string.Empty;
                await _employeeRepository.UpdateAsync(employee);

                EmployeeLoginHistory employeeLoginHistory = await _employeeLoginHistoryRepository.GetByUserID(request.ID);
                employeeLoginHistory.LogoutTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time").ToString("hh:mm tt");
                await _employeeLoginHistoryRepository.UpdateAsync(employeeLoginHistory);
                //Task.Run(async () => await _employeeLoginHistoryRepository.UpdateAsync(employeeLoginHistory));

                return Ok(new SharedResponse(true, 200, "Logout Successfully."));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(request?.Token);
            if (principal == null)
            {
                throw new ArgumentException(nameof(principal));
            }
            var result = int.TryParse(principal.Identity.Name, out int userId);
            if (!result)
            {
                throw new ArgumentException(nameof(userId));
            }
            var user = await _employeeRepository.GetByIdAsync(userId);
            if (user.RefreshToken != request.RefreshToken)
                return BadRequest(new SharedResponse(false, 400, "Invalid Refresh token", null));
            var response = await _jwtService.GenerateTokenAsync(user);
            return Ok(new SharedResponse(true, 200, "Success", response));
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var user = await _employeeRepository.GetByIdAsync(Convert.ToInt32(UserId));
            if (user != null)
            {
                var decruptedPassword = _hashingService.DecryptPassword(user.Password, user.Salt);
                if (decruptedPassword == request.CurrentPassword)
                {
                    var salt = _hashingService.GenerateSalt(8, 10);
                    var newHash = _hashingService.EncryptPassword(request.NewPassword, salt);
                    user.Password = newHash;
                    user.Salt = salt;
                    user.LastPasswordChangedById = Convert.ToInt32(UserId);
                    user.LastPasswordChangedDate = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    await _employeeRepository.UpdateAsync(user);
                    return Ok(new SharedResponse(true, 200, "Success"));
                }
                return Ok(new SharedResponse(false, 400, "Invalid Current Password."));
            }
            return Ok(new SharedResponse(false, 400, "Please try later."));
        }

        [HttpPost("ChangeEmployeePassword")]
        public async Task<IActionResult> ChangeEmployeePassword([FromBody] ChangeEmployeePasswordRequest request)
        {
            var user = await _employeeRepository.GetByIdAsync(request.ID);
            if (user != null)
            {
                var salt = _hashingService.GenerateSalt(8, 10);
                var newHash = _hashingService.EncryptPassword(request.Password, salt);
                user.Password = newHash;
                user.Salt = salt;
                user.LastPasswordChangedById = Convert.ToInt32(UserId);
                user.LastPasswordChangedDate = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                await _employeeRepository.UpdateAsync(user);
                return Ok(new SharedResponse(true, 200, "Success"));
            }
            return Ok(new SharedResponse(false, 400, "Please try later."));
        }

        [HttpPost("GetEmployeesHistory")]
        public async Task<IActionResult> GetEmployeesHistory([FromBody] PaginationRequest request)
        {
            try
            {
                var response = await _employeeLoginHistoryRepository.GetEmployeesLoginHistory(request);
                return Ok(new SharedResponse(true, 200, "Success", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
