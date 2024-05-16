using Baetoti.API.Controllers.Base;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Enum;
using Baetoti.Core.Entites;
using Baetoti.Shared.Request.Email;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Request.User;

namespace Baetoti.API.Controllers
{
    public class EmailController : ApiBaseController
    {

        public readonly IEmailService _emailService;
        public readonly IEmailTemplateRepository _emailTemplateRepository;
        public readonly IOTPRepository _otpRepository;
        public readonly IUserRepository _userRepository;

        public EmailController(
            IEmailService emailService,
            IEmailTemplateRepository emailTemplateRepository,
            IOTPRepository oTPRepository,
            IUserRepository userRepository
            )
        {
            _emailService = emailService;
            _emailTemplateRepository = emailTemplateRepository;
            _otpRepository = oTPRepository;
            _userRepository = userRepository;
        }

        [HttpPost("Verify")]
        public async Task<IActionResult> Verify([FromBody] EmailVerificationRequest request)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email);
                if (user != null && user.ID != long.Parse(UserId))
                {
                    return Ok(new SharedResponse(false, 400, "Email already exists."));
                }
                var _min = 1000;
                var _max = 9999;
                var _rdm = new Random();
                var number = _rdm.Next(_min, _max);
                var otp = new OTP();
                otp.UserID = long.Parse(UserId);
                otp.OneTimePassword = number.ToString();
                otp.OTPGeneratedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                otp.OTPStatus = (int)OTPStatus.Active;
                otp.RetryCount = 0;
                await _otpRepository.AddAsync(otp);

                user = await _userRepository.GetByIdAsync(long.Parse(UserId));
                user.Email = request.Email;
                await _userRepository.UpdateAsync(user);

                EmailTemplate emailMessage = await _emailTemplateRepository.GetByIdAsync((int)EmailType.EmailVerification);
                EmailRequest email = new EmailRequest()
                {
                    Receiver = request.Email,
                    Subject = emailMessage.Subject,
                    UserID = UserId,
                    API = "/api/Email/Verify"
                };
                email.Body = emailMessage.HtmlBody.Replace("{OTP}", otp.OneTimePassword);
                bool result = await _emailService.SendEmailAsync(email);
                if (result)
                    return Ok(new SharedResponse(true, 200, "Email Sent Successfully."));
                else
                    return Ok(new SharedResponse(false, 400, "Something went wrong. Please try later."));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("VerifyOTP")]
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
                varifiedOTP.OTPStatus = (int)OTPStatus.Approved;
                varifiedOTP.Description = "Approved";
                await _otpRepository.UpdateAsync(varifiedOTP);
                User user = await _userRepository.GetByIdAsync(long.Parse(UserId));
                user.IsEmailVerified = true;
                await _userRepository.UpdateAsync(user);
                return Ok(new SharedResponse(true, 200, "OTP Verified Successfully."));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("GetAllTemplate")]
        public async Task<IActionResult> GetAllTemplate()
        {
            try
            {
                var emailTemplates = await _emailTemplateRepository.GetAllTemplate();
                return Ok(new SharedResponse(true, 200, "", emailTemplates));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetTemplateByID")]
        public async Task<IActionResult> GetTemplateByID(long ID)
        {
            try
            {
                var emailTemplate = await _emailTemplateRepository.GetTemplateByID(ID);
                return Ok(new SharedResponse(true, 200, "", emailTemplate));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("UpdateEmailTemplate")]
        public async Task<IActionResult> UpdateEmailTemplate([FromBody] EmailTemplateRequest request)
        {
            try
            {
                var emailTemplate = await _emailTemplateRepository.GetByIdAsync(request.ID);
                emailTemplate.Subject = request.Subject;
                emailTemplate.HtmlBody = request.HtmlBody;
                await _emailTemplateRepository.UpdateAsync(emailTemplate);
                return Ok(new SharedResponse(true, 200, "Email Template Updated Successfully.", emailTemplate));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
