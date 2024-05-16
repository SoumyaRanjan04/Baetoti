using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Repositories;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Request.Category;
using Baetoti.Shared.Response.Category;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Baetoti.Shared.Request.Country;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Extentions;

namespace Baetoti.API.Controllers
{
	public class CountryController : ApiBaseController
	{

		public readonly ICountryRepositroy _countryRepository;
		public readonly IMapper _mapper;

		public CountryController(
			ICountryRepositroy countryRepositroy,
			IMapper mapper
			)
		{
			_countryRepository = countryRepositroy;
			_mapper = mapper;
		}

		[HttpGet("GetAll")]
		public async Task<IActionResult> GetAll()
		{
			try
			{
				var countries = (await _countryRepository.ListAllAsync()).ToList();
				return Ok(new SharedResponse(true, 200, "", countries));
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
				var country = await _countryRepository.GetByIdAsync(Id);
				return Ok(new SharedResponse(true, 200, "", country));
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message, null));
			}
		}

		[HttpPost("Add")]
		public async Task<IActionResult> Add([FromBody] CountryRequest request)
		{
			try
			{
				var country = _mapper.Map<Country>(request);
				country.RecordDateTime = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
				country.CountryStatus = (int)CountryStatus.Active;
				var result = await _countryRepository.AddAsync(country);
				if (result == null)
				{
					return Ok(new SharedResponse(false, 400, "Unable To Create Country"));
				}
				return Ok(new SharedResponse(true, 200, "Country Created Succesfully"));
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message));
			}
		}

		[HttpPost("Update")]
		public async Task<IActionResult> Update([FromBody] CountryRequest request)
		{
			try
			{
				var country = await _countryRepository.GetByIdAsync(request.ID);
				if (country != null)
				{
					country.CountryName = request.CountryName;
					country.CountryCode = request.CountryCode;
					country.PhoneCode = request.PhoneCode;
					await _countryRepository.UpdateAsync(country);
					return Ok(new SharedResponse(true, 200, "Country Updated Succesfully"));
				}
				else
				{
					return Ok(new SharedResponse(false, 400, "unable to find country"));
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
				var country = await _countryRepository.GetByIdAsync(request.ID);
				if (country != null)
				{
					country.CountryStatus = request.Value;
					await _countryRepository.UpdateAsync(country);
					return Ok(new SharedResponse(true, 200, "Country Status Updated Succesfully"));
				}
				else
				{
					return Ok(new SharedResponse(false, 400, "unable to find country"));
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
				var country = await _countryRepository.GetByIdAsync(ID);
				if (country != null)
				{
					await _countryRepository.DeleteAsync(country);
					return Ok(new SharedResponse(true, 200, "Country Deleted Succesfully"));
				}
				else
				{
					return Ok(new SharedResponse(false, 400, "Unable To Find Country"));
				}
			}
			catch (Exception ex)
			{
				return Ok(new SharedResponse(false, 400, ex.Message));
			}
		}

	}
}
