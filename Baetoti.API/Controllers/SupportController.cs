using Baetoti.API.Controllers.Base;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Shared.Request.SupportRequest;
using Baetoti.Shared.Response.SupportRequest;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Request.Dashboard;

namespace Baetoti.API.Controllers
{
    public class SupportController : ApiBaseController
    {

        public readonly ISupportRequestRepository _supportRequestRepository;

        public SupportController(
            ISupportRequestRepository supportRequestRepository
            )
        {
            _supportRequestRepository = supportRequestRepository;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] SubmitSupportRequest request)
        {
            try
            {
                SupportRequest supportRequest = new SupportRequest
                {
                    UserID = long.Parse(UserId),
                    SupportRequestType = request.SupportRequestType,
                    Title = request.Title,
                    Comments = request.Comments,
                    Picture = request.Picture,
                    RecordDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                };
                await _supportRequestRepository.AddAsync(supportRequest);
                return Ok(new SharedResponse(true, 200, "Request Submitted Successfully", supportRequest));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Feedback")]
        public async Task<IActionResult> Feedback([FromBody] FeedbackRequest request)
        {
            try
            {
                SupportRequest supportRequest = await _supportRequestRepository.GetByIdAsync(request.ID);
                supportRequest.UserRating = request.Rating;
                supportRequest.UserFeedback = request.FeedbackComments;
                await _supportRequestRepository.UpdateAsync(supportRequest);
                return Ok(new SharedResponse(true, 200, "Feedback Submitted Successfully", supportRequest));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("GetAll")]
        public async Task<IActionResult> GetAll([FromBody] SupportRequestFilter request)
        {
            try
            {
                var supportRequests = await _supportRequestRepository.GetAll(request);
                return Ok(new SharedResponse(true, 200, "", supportRequests));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("GetByID")]
        public async Task<IActionResult> GetByID(RequestID request)
        {
            try
            {
                SupportRequestByIDResponse supportRequest = await _supportRequestRepository.GetById(request.ID, long.Parse(UserId), UserRole);
                return Ok(new SharedResponse(true, 200, "", supportRequest));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("OpenRequest")]
        public async Task<IActionResult> OpenRequest(RequestID request)
        {
            try
            {
                SupportRequest supportRequest = await _supportRequestRepository.GetByIdAsync(request.ID);
                supportRequest.SupportRequestStatus = (int)SupportRequestStatus.Open;
                supportRequest.OpenDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                await _supportRequestRepository.UpdateAsync(supportRequest);
                return Ok(new SharedResponse(true, 200, "Request Opened Successfully", supportRequest));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("CloseRequest")]
        public async Task<IActionResult> CloseRequest(CloseSupportRequest request)
        {
            try
            {
                SupportRequest supportRequest = await _supportRequestRepository.GetByIdAsync(request.ID);
                supportRequest.SupportRequestStatus = (int)SupportRequestStatus.Close;
                supportRequest.ResolveDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                supportRequest.SupervisorID = long.Parse(UserId);
                supportRequest.SupervisorComments = request.SupervisorComments;
                supportRequest.SupervisorCommentsForUser = request.SupervisorCommentsForUser;
                supportRequest.ActionTaken = request.ActionTaken;
                supportRequest.RootCause = request.RootCause;
                await _supportRequestRepository.UpdateAsync(supportRequest);
                return Ok(new SharedResponse(true, 200, "Request Closed Successfully", supportRequest));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("GetDashboardData")]
        public async Task<IActionResult> GetDashboardData([FromBody] DashboardRequest request)
        {
            try
            {
                var response = await _supportRequestRepository.GetDashboardData(request);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

    }
}
