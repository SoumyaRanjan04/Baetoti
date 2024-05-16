using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Infrastructure.Data.Repositories;
using Baetoti.Shared.Extentions;
using Baetoti.Shared.Request.Shared;
using Baetoti.Shared.Request.User;
using Baetoti.Shared.Response.Dashboard;
using Baetoti.Shared.Response.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Baetoti.API.Controllers
{
    public class UserLocationController : ApiBaseController
    {

        public readonly IUserRepository _userRepository;
        public readonly IUserLocationRepository _userLocationRepository;
        public readonly IMapper _mapper;

        public UserLocationController(
            IUserRepository userRepository,
            IUserLocationRepository userLocationRepository,
            IMapper mapper
            )
        {
            _userRepository = userRepository;
            _userLocationRepository = userLocationRepository;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(long UserID)
        {
            try
            {
                var userLocations = await _userLocationRepository.GetAllLocations(UserID);
                return Ok(new SharedResponse(true, 200, "", userLocations));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] UpdateLocationRequest request)
        {
            try
            {
                if (request.IsSelected)
                {
                    UserLocation defaultLocation = await _userLocationRepository.GetDefaultLocation(long.Parse(UserId));
                    var user = await _userRepository.GetByIdAsync(Convert.ToInt64(UserId));
                    user.Address = request.Address;
                    user.Longitude = request.coordinates.Longitude;
                    user.Latitude = request.coordinates.Latitude;
                    user.DefaultLocationTitle = request.Title;
                    user.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    user.UpdatedBy = Convert.ToInt32(UserId);
                    await _userRepository.UpdateAsync(user);

                    if (defaultLocation != null)
                    {
                        defaultLocation.IsDefault = false;
                        await _userLocationRepository.UpdateAsync(defaultLocation);
                    }
                }
                UserLocation userLocation = new UserLocation
                {
                    UserID = Convert.ToInt64(UserId),
                    Title = request.Title,
                    Address = request.Address,
                    Longitude = request.coordinates.Longitude,
                    Latitude = request.coordinates.Latitude,
                    CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time"),
                    CreatedBy = Convert.ToInt32(UserId),
                    MarkAsDeleted = false,
                    IsDefault = request.IsSelected
                };
                await _userLocationRepository.AddAsync(userLocation);
                return Ok(new SharedResponse(true, 200, "User Location Added Succesfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] UpdateLocationRequest request)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(Convert.ToInt64(UserId));
                if (user != null)
                {
                    UserLocation defaultLocation = await _userLocationRepository.GetDefaultLocation(long.Parse(UserId));
                    if (request.IsSelected)
                    {
                        user.Address = request.Address;
                        user.Longitude = request.coordinates.Longitude;
                        user.Latitude = request.coordinates.Latitude;
                        user.DefaultLocationTitle = request.Title;
                        user.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                        user.UpdatedBy = Convert.ToInt32(UserId);
                        await _userRepository.UpdateAsync(user);



                        if (defaultLocation != null)
                        {
                            defaultLocation.IsDefault = false;
                            //request.ID = defaultLocation.UserID; // for setting up the default userId
                            await _userLocationRepository.UpdateAsync(defaultLocation);
                        }
                    }
                    UserLocation userLocation = await _userLocationRepository.GetByIdAsync(request.ID);
                    userLocation.Title = request.Title;
                    userLocation.Address = request.Address;
                    userLocation.Longitude = request.coordinates.Longitude;
                    userLocation.Latitude = request.coordinates.Latitude;
                    userLocation.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    userLocation.UpdatedBy = Convert.ToInt32(UserId);
                    if (request.IsSelected)
                    {
                        userLocation.IsDefault = request.IsSelected;
                    }
                    await _userLocationRepository.UpdateAsync(userLocation);
                    return Ok(new SharedResponse(true, 200, "User Location Updated Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find User"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("SetDefault")]
        public async Task<IActionResult> SetDefault(long ID)
        {
            try
            {
                var location = await _userLocationRepository.GetByIdAsync(ID);
                if (location != null)
                {
                    UserLocation defaultLocation = await _userLocationRepository.GetDefaultLocation(long.Parse(UserId));
                    if (defaultLocation != null)
                    {
                        defaultLocation.IsDefault = false;
                        await _userLocationRepository.UpdateAsync(defaultLocation);
                    }
                    location.IsDefault = true;
                    location.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    location.UpdatedBy = Convert.ToInt32(UserId);
                    await _userLocationRepository.UpdateAsync(location);

                    var user = await _userRepository.GetByIdAsync(Convert.ToInt64(UserId));
                    user.Address = location.Address;
                    user.Longitude = location.Longitude;
                    user.Latitude = location.Latitude;
                    user.DefaultLocationTitle = location.Title;
                    user.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    user.UpdatedBy = Convert.ToInt32(UserId);
                    await _userRepository.UpdateAsync(user);

                    return Ok(new SharedResponse(true, 200, "Location Deleted Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Location"));
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
                var location = await _userLocationRepository.GetByIdAsync(ID);
                if (location != null)
                {
                    if (location.IsDefault)
                    {
                        return Ok(new SharedResponse(false, 400, "Unable To Delete Default Location"));
                    }
                    location.MarkAsDeleted = true;
                    location.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    location.UpdatedBy = Convert.ToInt32(UserId);
                    await _userLocationRepository.UpdateAsync(location);
                    return Ok(new SharedResponse(true, 200, "Location Deleted Succesfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Location"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("SetLive")]
        public async Task<IActionResult> SetLive([FromBody] UpdateLocationRequest request)
        {
            try
            {
                UserLocation liveLocation = await _userLocationRepository.CheckLiveLocation(long.Parse(UserId));
                if (liveLocation == null)
                {
                    liveLocation = new UserLocation
                    {
                        UserID = Convert.ToInt64(UserId),
                        Title = request.Title,
                        Address = !string.IsNullOrEmpty(request.Address) ? request.Address : "Live Location",
                        Longitude = request.coordinates.Longitude,
                        Latitude = request.coordinates.Latitude,
                        CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time"),
                        CreatedBy = Convert.ToInt32(UserId),
                        MarkAsDeleted = false,
                        IsDefault = false,
                        IsLive = true
                    };
                    await _userLocationRepository.AddAsync(liveLocation);
                }
                else
                {
                    liveLocation.Title = request.Title;
                    liveLocation.Address = request.Address;
                    liveLocation.Latitude = request.coordinates.Latitude;
                    liveLocation.Longitude = request.coordinates.Longitude;
                    await _userLocationRepository.UpdateAsync(liveLocation);
                }
                return Ok(new SharedResponse(true, 200, "User Live Location Updated Succesfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpGet("GetLive")]
        public async Task<IActionResult> GetLive()
        {
            try
            {
                var userLocation = await _userLocationRepository.GetLiveLocation(long.Parse(UserId));
                return Ok(new SharedResponse(true, 200, "", userLocation));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

    }
}
