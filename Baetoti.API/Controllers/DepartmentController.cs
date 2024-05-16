using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Response.Department;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
    public class DepartmentController : ApiBaseController
    {
        public readonly IDepartmentRepository _departmentRepository;
        public readonly IMapper _mapper;

        public DepartmentController(
            IDepartmentRepository departmentRepository,
            IMapper mapper
            )
        {
            _departmentRepository = departmentRepository;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var departmentList = (await _departmentRepository.ListAllAsync())
                    .Where(x => x.MarkAsDeleted == false).ToList();
                return Ok(new SharedResponse(true, 200, "", _mapper.Map<List<DepartmentResponse>>(departmentList)));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }
    }
}
