using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.SubMenu;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
    public class SubMenuController : ApiBaseController
    {
        public readonly ISubMenuRepository _submenuRepository;
        public readonly IMapper _mapper;

        public SubMenuController(
            ISubMenuRepository submenuRepository,
            IMapper mapper
            )
        {
            _submenuRepository = submenuRepository;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var submenuList = (await _submenuRepository.GetAll()).ToList();

                return Ok(new SharedResponse(true, 200, "", _mapper.Map<List<SubMenuResponse>>(submenuList)));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }
    }
}
