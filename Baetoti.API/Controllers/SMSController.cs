using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Request.SMS;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
    public class SMSController : ApiBaseController
    {

        public readonly ISMSService _smsService;
        public readonly ISMSTemplateRepository _smsTemplateRepository;

        public SMSController(
            ISMSService smsService,
            ISMSTemplateRepository smsTemplateRepository
            )
        {
            _smsService = smsService;
            _smsTemplateRepository = smsTemplateRepository;
        }

        [HttpPost("GetAllTemplate")]
        public async Task<IActionResult> GetAllTemplate()
        {
            try
            {
                var smsTemplates = await _smsTemplateRepository.GetAllTemplate();
                return Ok(new SharedResponse(true, 200, "", smsTemplates));
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
                var smsTemplate = await _smsTemplateRepository.GetTemplateByID(ID);
                return Ok(new SharedResponse(true, 200, "", smsTemplate));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("UpdateTemplate")]
        public async Task<IActionResult> UpdateTemplate([FromBody] SMSTemplateRequest request)
        {
            try
            {
                var smsTemplate = await _smsTemplateRepository.GetByIdAsync(request.ID);
                smsTemplate.SMSText = request.SMSText;
                await _smsTemplateRepository.UpdateAsync(smsTemplate);
                return Ok(new SharedResponse(true, 200, "SMS Template Updated Successfully.", smsTemplate));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
