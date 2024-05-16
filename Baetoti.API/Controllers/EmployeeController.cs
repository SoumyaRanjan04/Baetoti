using AutoMapper;
using Baetoti.API.Controllers.Base;
using Baetoti.API.Helpers;
using Baetoti.Core.Entites;
using Baetoti.Core.Interface.Repositories;
using Baetoti.Core.Interface.Services;
using Baetoti.Shared.Request.Employee;
using Baetoti.Shared.Response.FileUpload;
using Baetoti.Shared.Response.Shared;
using Baetoti.Shared.Response.Employee;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Baetoti.Shared.Request.EmployeeRole;
using Baetoti.Shared.Enum;
using Baetoti.Shared.Extentions;

namespace Baetoti.API.Controllers
{
    public class EmployeeController : ApiBaseController
    {
        public readonly IEmployeeRepository _employeeRepository;
        public readonly IEmployeeRoleRepository _employeeroleRepository;
        private readonly IRijndaelEncryptionService _hashingService;
        public readonly IMapper _mapper;

        public EmployeeController(
         IEmployeeRepository employeeRepository,
         IEmployeeRoleRepository employeeroleRepository,
         IRijndaelEncryptionService hashingService,
         IMapper mapper
         )
        {
            _employeeRepository = employeeRepository;
            _employeeroleRepository = employeeroleRepository;
            _hashingService = hashingService;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _employeeRepository.GetAll();
                return Ok(new SharedResponse(true, 200, "", response));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long ID)
        {
            try
            {
                var employee = await _employeeRepository.GetById(ID);
                if (employee != null)
                {
                    return Ok(new SharedResponse(true, 200, "", employee));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Employee"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message, null));
            }
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] EmployeeRequest employeeRequest)
        {
            try
            {
                var employee = _mapper.Map<Employee>(employeeRequest);
                var salt = _hashingService.GenerateSalt(8, 10);
                employee.Salt = salt;
                employee.Password = _hashingService.EncryptPassword(employeeRequest.Password, salt);
                employee.MarkAsDeleted = false;
                employee.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                employee.CreatedBy = Convert.ToInt32(UserId);
                employee.EmployeeStatus = 1; //Change it after Email or mobile verifications
                var result = await _employeeRepository.AddAsync(employee);
                if (result == null)
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Create Employee"));
                }
                else
                {
                    AssignRoleRequest assignRole = new AssignRoleRequest();
                    assignRole.RoleId = employeeRequest.RoleID;
                    assignRole.EmployeeId = Convert.ToInt32(result.ID);
                    var role = _mapper.Map<EmployeeRole>(assignRole);
                    role.MarkAsDeleted = false;
                    role.CreatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    role.CreatedBy = Convert.ToInt32(UserId);
                    var res = await _employeeroleRepository.AddAsync(role);
                }
                return Ok(new SharedResponse(true, 200, "Employee Created Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update([FromBody] EmployeeRequest employeeRequest)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(employeeRequest.ID);
                if (employee != null)
                {
                    employee.FirstName = employeeRequest.FirstName;
                    employee.LastName = employeeRequest.LastName;
                    employee.JoiningDate = employeeRequest.JoiningDate;
                    employee.Location = employeeRequest.Location;
                    employee.DepartmentID = employeeRequest.DepartmentID;
                    employee.DesignationID = employeeRequest.DesignationID;
                    employee.Username = employeeRequest.Username;
                    employee.Gender = employeeRequest.Gender;
                    employee.Shift = employeeRequest.Shift;
                    employee.Email = employeeRequest.Email;
                    employee.DOB = employeeRequest.DOB;
                    employee.Phone = employeeRequest.Phone;
                    employee.ReportTo = employeeRequest.ReportTo;
                    employee.Address = employeeRequest.Address;
                    employee.Goals = employeeRequest.Goals;
                    employee.Skills = employeeRequest.Skills;
                    employee.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    employee.LastUpdatedBy = Convert.ToInt32(UserId);
                    if (!string.IsNullOrEmpty(employeeRequest.Picture))
                        employee.Picture = employeeRequest.Picture;
                    await _employeeRepository.UpdateAsync(employee);

                    //Updating Roles
                    var employeeRole = await _employeeroleRepository.GetByUserId(employeeRequest.ID);
                    if (employeeRole != null)
                    {
                        employeeRole.RoleId = employeeRequest.RoleID;
                        employeeRole.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                        employeeRole.UpdatedBy = Convert.ToInt32(UserId);
                        await _employeeroleRepository.UpdateAsync(employeeRole);
                    }
                    return Ok(new SharedResponse(true, 200, "Employee Updated Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Employee"));
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
                var un = await _employeeRepository.GetByIdAsync(ID);
                if (un != null)
                {
                    un.MarkAsDeleted = true;
                    un.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                    un.LastUpdatedBy = Convert.ToInt32(UserId);
                    await _employeeRepository.DeleteAsync(un);
                    return Ok(new SharedResponse(true, 200, "Employee Deleted Successfully"));
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "Unable To Find Employee"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost("BlockUnBlock")]
        public async Task<IActionResult> BlockUnBlock([FromBody] BlockUnBlockRequest blockUnBlockRequest)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(blockUnBlockRequest.ID);
                employee.EmployeeStatus = blockUnBlockRequest.IsBlocked == true ? (int)EmployementStatus.Blocked : (int)EmployementStatus.Active;
                employee.Comments = blockUnBlockRequest.Comments;
                employee.LastUpdatedAt = DateTime.Now.ToTimeZoneTime("Arab Standard Time");
                employee.UpdatedBy = Convert.ToInt32(UserId);
                await _employeeRepository.UpdateAsync(employee);
                return Ok(new SharedResponse(true, 200, "Employee Request Processed Successfully"));
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    UploadImage obj = new UploadImage();
                    FileUploadResponse _RESPONSE = await obj.UploadImageFile(file, "Employee");
                    if (string.IsNullOrEmpty(_RESPONSE.Message))
                    {
                        return Ok(new SharedResponse(true, 200, "File uploaded successfully!", _RESPONSE));
                    }
                    else
                    {
                        return Ok(new SharedResponse(true, 400, _RESPONSE.Message));
                    }
                }
                else
                {
                    return Ok(new SharedResponse(false, 400, "File is required!"));
                }
            }
            catch (Exception ex)
            {
                return Ok(new SharedResponse(false, 400, ex.Message));
            }
        }

        //Save Sase64 Image In Directory
        //private FileUploadOutput WriteBase64ImageFile(byte[] bytess, string extension)
        //{
        //    FileUploadOutput _result = new FileUploadOutput();
        //    bool WriteFileWithRoot = true;
        //    string fileName = null;
        //    try
        //    {
        //        if (WriteFileWithRoot == true)
        //        {
        //            fileName = DateTime.Now.ToTimeZoneTime("Arab Standard Time").Ticks + extension; //Create a new Name for the file due to security reasons.
        //            var pathBuilt = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads\\Visitors");
        //            if (!Directory.Exists(pathBuilt))
        //            {
        //                Directory.CreateDirectory(pathBuilt);
        //            }
        //            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Uploads\\Visitors", fileName);
        //            using (var imageFile = new FileStream(path, FileMode.Create))
        //            {
        //                imageFile.Write(bytess, 0, bytess.Length);
        //                imageFile.Flush();
        //            }
        //        }
        //        _result.Message = "File uploaded successfully!";
        //        _result.FileName = fileName;
        //        _result.Path = "Uploads/Visitors";
        //        _result.PathwithFileName = "Uploads/Visitors/" + fileName;
        //        _result.StatusCode = "200";
        //    }
        //    catch (Exception e)
        //    {
        //        _result.Message = "failed to Upload File!";
        //        _result.StatusCode = "500";
        //        //isSaveSuccess = false;
        //    }
        //    return _result;
        //}

        //private string GetExtenstionFromBase64(string base64String)
        //{
        //    String[] strings = base64String.Split(",");
        //    string extension;
        //    switch (strings[0])
        //    {
        //        //check image's extension
        //        case "data:image/jpg;base64":
        //            extension = ".jpg";
        //            break;
        //        case "data:image/png;base64":
        //            extension = ".png";
        //            break;
        //        default:
        //            //should write cases for more images types
        //            extension = ".jpeg";
        //            break;
        //    }
        //    return extension;
        //}
    }
}
