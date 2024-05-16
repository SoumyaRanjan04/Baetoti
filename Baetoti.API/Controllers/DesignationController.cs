using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Response.Designation;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
    public class DesignationController : ApiBaseController
    {
        public readonly IDesignationRepository _designationRepository;
        public readonly IMapper _mapper;

        public DesignationController(
            IDesignationRepository designationRepository,
            IMapper mapper
            )
        {
            _designationRepository = designationRepository;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var designationList = (await _designationRepository.ListAllAsync())
                    .Where(x => x.MarkAsDeleted == false).ToList();
                return Ok(new SharedResponse(true, 200, "", _mapper.Map<List<DesignationResponse>>(designationList)));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }
    }
}
