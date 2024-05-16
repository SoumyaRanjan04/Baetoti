using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Request.UnitRequest;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
    public class UnitController : ApiBaseController
    {

        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;

        public UnitController(
         IUnitRepository unitRepository,
         IMapper mapper
         )
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var unitList = await _unitRepository.GetAllUnitsAsync();

                return Ok(new SharedResponse(true, 200, "", unitList));
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
                var unit = await _unitRepository.GetUnitByID(Id);
                return Ok(new SharedResponse(true, 200, "", unit));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] UnitRequest unitRequest)
        {
            try
            {
                var unit = _mapper.Map<Unit>(unitRequest);
                unit.MarkAsDeleted = false;
                unit.UnitStatus = (int)UnitStatus.Active;
                unit.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                unit.CreatedBy = Convert.ToInt32(UserId);
                var result = await _unitRepository.AddAsync(unit);
                if (result == null)
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Create Unit"));
                }
                return Ok(new SharedResponse(true, 200, "Unit Created Succesfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] UnitRequest unitRequest)
        {
            try
            {
                var unit = await _unitRepository.GetByIdAsync(unitRequest.ID);
                if (unit != null)
                {
                    unit.UnitType = unitRequest.UnitType;
                    unit.UnitEnglishName = unitRequest.UnitEnglishName;
                    unit.UnitArabicName = unitRequest.UnitArabicName;
                    unit.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    unit.UpdatedBy = Convert.ToInt32(UserId);
                    await _unitRepository.UpdateAsync(unit);
                    return Ok(new SharedResponse(true, 200, "Unit Updated Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Unit"));
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
                var unit = await _unitRepository.GetByIdAsync(request.ID);
                if (unit != null)
                {
                    unit.UnitStatus = request.Value;
                    unit.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    unit.UpdatedBy = Convert.ToInt32(UserId);
                    await _unitRepository.UpdateAsync(unit);
                    return Ok(new SharedResponse(true, 200, "Unit UnSuspended Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Unit"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpDelete("Delete/{ID}")]
        public async Task<IActionResult> Delete(long ID)
        {
            try
            {
                var un = await _unitRepository.GetByIdAsync(ID);
                if (un != null)
                {
                    un.MarkAsDeleted = true;
                    un.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    un.UpdatedBy = Convert.ToInt32(UserId);
                    await _unitRepository.UpdateAsync(un);
                    return Ok(new SharedResponse(true, 200, "Unit Deleted Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Unit"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

    }
}
