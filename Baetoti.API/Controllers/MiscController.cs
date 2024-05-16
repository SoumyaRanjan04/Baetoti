using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
    public class MiscController : ApiBaseController
    {
        public readonly IUserRepository _userRepository;

        public MiscController(
            IUserRepository userRepository
            )
        {
            _userRepository = userRepository;
        }

        [HttpGet("GetUserTypes")]
        public async Task<IActionResult> GetUserTypes()
        {
            try
            {
                var list = new List<string>();
                list.Add("Buyer");
                list.Add("Provider");
                list.Add("Driver");
                return Ok(new SharedResponse(true, 200, "", list));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetUserStatus")]
        public async Task<IActionResult> GetUserStatus()
        {
            try
            {
                var userStatus = (await _userRepository.ListAllAsync()).
                    Select(x => new { UserStatus = ((UserStatus)x.UserStatus).ToString() }).Distinct().ToList();
                return Ok(new SharedResponse(true, 200, "", userStatus));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetLocations")]
        public async Task<IActionResult> GetLocations()
        {
            try
            {
                var list = new List<string>();
                list.Add("Pakistan");
                list.Add("India");
                list.Add("UAE");
                return Ok(new SharedResponse(true, 200, "", list));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetCountries")]
        public async Task<IActionResult> GetCountries()
        {
            try
            {
                var list = new List<string>();
                list.Add("Pakistan");
                list.Add("India");
                list.Add("UAE");
                return Ok(new SharedResponse(true, 200, "", list));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetCity")]
        public async Task<IActionResult> GetCity(string countryName)
        {
            try
            {
                var list = new List<string>();
                list.Add("Pakistan");
                list.Add("India");
                list.Add("UAE");
                return Ok(new SharedResponse(true, 200, "", list));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetGender")]
        public async Task<IActionResult> GetGender()
        {
            try
            {
                var list = new List<string>();
                list.Add("Not Specified");
                list.Add("Male");
                list.Add("Female");
                return Ok(new SharedResponse(true, 200, "", list));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetStatus")]
        public async Task<IActionResult> GetStatus()
        {
            try
            {
                var list = new List<string>();
                list.Add("Online");
                list.Add("Offline");
                return Ok(new SharedResponse(true, 200, "", list));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetFence")]
        public async Task<IActionResult> GetFence()
        {
            try
            {
                var list = new List<string>();
                list.Add("Pakistan");
                list.Add("UAE");
                return Ok(new SharedResponse(true, 200, "", list));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetAllOrderStatus")]
        public async Task<IActionResult> GetAllOrderStatus()
        {
            try
            {
                var list = new List<KeyValuePair<string, int>>();
                foreach (var e in Enum.GetValues(typeof(OrderStatus)))
                {
                    list.Add(new KeyValuePair<string, int>(e.ToString(), (int)e));
                }
                return Ok(new SharedResponse(true, 200, "", list));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
