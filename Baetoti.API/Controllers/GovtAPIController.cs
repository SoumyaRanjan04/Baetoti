using Baetoti.API.Controllers.Base;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Baetoti.Shared.Response.GovtAPI;
using Baetoti.Core.Interface.Repositories;
using AutoMapper;

namespace Baetoti.API.Controllers
{
    public class GovtAPIController : ApiBaseController
    {

        private readonly IGovtAPIService _govtAPIService;
        private readonly IRegionRepository _regionRepository;
        private readonly ICityRepository _cityRepository;
        private readonly ICarTypeRepository _carTypeRepository;
        public readonly IMapper _mapper;

        public GovtAPIController(
            ISiteConfigService siteConfigService,
            IGovtAPIService govtAPIService,
            IRegionRepository regionRepository,
            ICityRepository cityRepository,
            ICarTypeRepository carTypeRepository,
            IMapper mapper
            ) : base(siteConfigService)
        {
            _govtAPIService = govtAPIService;
            _regionRepository = regionRepository;
            _cityRepository = cityRepository;
            _mapper = mapper;
            _carTypeRepository = carTypeRepository;
        }

        [HttpGet("GetAuthorities")]
        public async Task<IActionResult> GetAuthorities()
        {
            try
            {
                var regions = await _govtAPIService.GetAuthorities(_siteConfig);
                return Ok(new SharedResponse(true, 200, "", regions));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetCancellationReasons")]
        public async Task<IActionResult> GetCancellationReasons()
        {
            try
            {
                var regions = await _govtAPIService.GetCancellationReasons(_siteConfig);
                return Ok(new SharedResponse(true, 200, "", regions));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetRegions")]
        public async Task<IActionResult> GetRegions()
        {
            try
            {
                SharedLookupResponse[] regions;
                if (_siteConfig.UseLocalDataForGovtAPIs == "1")
                    regions = _mapper.Map<SharedLookupResponse[]>(await _regionRepository.ListAllAsync());
                else
                    regions = await _govtAPIService.GetRegions(_siteConfig);
                return Ok(new SharedResponse(true, 200, "", regions));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var regions = await _govtAPIService.GetCategories(_siteConfig);
                return Ok(new SharedResponse(true, 200, "", regions));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetIdentityTypes")]
        public async Task<IActionResult> GetIdentityTypes()
        {
            try
            {
                var regions = await _govtAPIService.GetIdentityTypes(_siteConfig);
                return Ok(new SharedResponse(true, 200, "", regions));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetPaymentMethods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            try
            {
                var regions = await _govtAPIService.GetPaymentMethods(_siteConfig);
                return Ok(new SharedResponse(true, 200, "", regions));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetCarTypes")]
        public async Task<IActionResult> GetCarTypes()
        {
            try
            {
                SharedLookupResponse[] carTypes;
                if (_siteConfig.UseLocalDataForGovtAPIs == "1")
                    carTypes = _mapper.Map<SharedLookupResponse[]>(await _carTypeRepository.ListAllAsync());
                else
                    carTypes = await _govtAPIService.GetCarTypes(_siteConfig);
                return Ok(new SharedResponse(true, 200, "", carTypes));
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
                var regions = await _govtAPIService.GetCountries(_siteConfig);
                return Ok(new SharedResponse(true, 200, "", regions));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetCity")]
        public async Task<IActionResult> GetCity(string RegionID)
        {
            try
            {
                SharedLookupResponse[] cities;
                if (_siteConfig.UseLocalDataForGovtAPIs == "1")
                    cities = _mapper.Map<SharedLookupResponse[]>(await _cityRepository.GetAll(RegionID));
                else
                    cities = await _govtAPIService.GetCities(_siteConfig, RegionID);
                return Ok(new SharedResponse(true, 200, "", cities));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
