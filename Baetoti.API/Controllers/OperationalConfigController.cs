using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Shared.Request.OperationalConfig;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Extentions;

namespace Baetoti.API.Controllers
{
    public class OperationalConfigController : ApiBaseController
    {
        public readonly IMapper _mapper;
        public readonly IOperationalConfigRepository _operationalConfigRepository;
        public readonly IFenceRepository _fenceRepositry;

        public OperationalConfigController(
            IMapper mapper,
            IOperationalConfigRepository operationalConfigRepository,
            IFenceRepository fenceRepository
            )
        {
            _mapper = mapper;
            _operationalConfigRepository = operationalConfigRepository;
            _fenceRepositry = fenceRepository;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _fenceRepositry.GetAllFences();
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int Id)
        {
            try
            {
                var response = await _fenceRepositry.GetFenceByID(Id);
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] OperationalConfigRequest request)
        {
            try
            {
                Fence fence = new Fence
                {
                    Title = request.Title,
                    Notes = request.Notes,
                    CountryID = request.CountryID,
                    RegionID = request.RegionID,
                    Region = request.Region,
                    City = request.City,
                    CityID = request.CityID,
                    FenceStatus = request.FenceStatus,
                    CreatedBy = Convert.ToInt32(UserId),
                    CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time")
                };
                await _fenceRepositry.AddAsync(fence);

                return Ok(new SharedResponse(true, 200, "Operational Config Created Succesfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] OperationalConfigRequest request)
        {
            try
            {
                Fence fence = await _fenceRepositry.GetByIdAsync(request.ID);
                if (fence != null)
                {
                    fence.Title = request.Title;
                    fence.Notes = request.Notes;
                    fence.CountryID = request.CountryID;
                    fence.RegionID = request.RegionID;
                    fence.Region = request.Region;
                    fence.City = request.City;
                    fence.CityID = request.CityID;
                    fence.FenceStatus = request.FenceStatus;
                    fence.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    fence.UpdatedBy = Convert.ToInt32(UserId);
                    await _fenceRepositry.UpdateAsync(fence);

                    return Ok(new SharedResponse(true, 200, "Operational Config Updated Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "unable to find Operational Config"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusRequest request)
        {
            try
            {
                Fence fence = await _fenceRepositry.GetByIdAsync(request.ID);
                if (fence != null)
                {
                    fence.FenceStatus = request.Value;
                    fence.StopedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    fence.StopedBy = Convert.ToInt32(UserId);
                    await _fenceRepositry.UpdateAsync(fence);
                    return Ok(new SharedResponse(true, 200, "Operational Config Updated Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "unable to find Operational Config"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpGet("GetFenceList")]
        public async Task<IActionResult> GetFenceList()
        {
            try
            {
                var response = await _fenceRepositry.GetFenceList();
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
